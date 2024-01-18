using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public DialogeElement startElement;
    public GameObject dialogCanvasLeft;
    public TextMeshProUGUI dialogText;
    public GameObject[] answerButtons;

    public InventoryItem[] allCraftablesInGame;

    private RandomEvent currentEvent;
    private OutcomeEvent eventToTriggerAfterDialogEnd = new OutcomeEvent();

    public GameObject sacrificeCanvas;
    public GameObject sacrificeCanvasEnd;
    public SpecialButton sacrificeButton;
    public TextMeshProUGUI sacrificedFollowerNameText;
    public TextMeshProUGUI sacrificedFollowerNameTextEnd;

    public GameObject banishCanvas;
    public SpecialButton banishButton;
    public GameObject banishCanvasEnd;
    public TextMeshProUGUI banishedFollowerNameText;
    public TextMeshProUGUI banishedFollowerNameTextEnd;
    [Space(5)]
    [Header("Main Screen References")]
    public GameObject peopleCanvas;
    public GameObject rationsCanvas;
    public GameObject measuresCanvas;
    public GameObject inventoryCanvas;
    public GameObject tasksCanvas;
    [Header("Canvas Item Prefabs")] // minden ami list�z�shoz kell prefab
    public GameObject peopleCanvasItemPrefab;
    public GameObject rationsCanvasItemPrefab;
    public GameObject inventoryCanvasItemPrefab;
    public GameObject tasksCanvasItemPrefab;
    [Header("Canvas Contents")] // ezek al� kell a lista elemeket parentelni p�ld�nyos�t�s ut�n
    public Transform peopleCanvasContent;
    public Transform rationsCanvasContent;
    public Transform inventoryCanvasContent;
    //public Transform tasksCanvasContent;

    private Follower currentFollower;
    private InventoryItem currentItem;
    private Map currentMap;

    [Header("People Panel Fields")]
    [SerializeField] private TextMeshProUGUI pplNameText;
    [SerializeField] private TextMeshProUGUI pplStoryText;
    [SerializeField] private Image weaponImage;
    [SerializeField] private GameObject extraOptions;
    [Header("Ration Panel Fields")]
    [SerializeField] private TextMeshProUGUI totalConsumptionText;
    [SerializeField] private Slider rationSlider;
    [SerializeField] private TextMeshProUGUI rationSliderText;
    [SerializeField] private GameObject extraOptionsRations;
    [Header("Inventory Panel Fields")]
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private GameObject extraOptionsInventory;
    [Header("Task Panel Fields")]
    [SerializeField] private Transform idleContent;
    [SerializeField] private Transform workContent;
    [SerializeField] private Transform scoutContent;
    //[SerializeField] private GameObject TaskPanelPrefab;
    [Header("Map Detail Panel Fields")]
    [SerializeField] private TextMeshProUGUI areaNameText;
    [SerializeField] private TextMeshProUGUI areaDescText;
    [SerializeField] private Image mapImage;




    private void Start()
    {
        Application.targetFrameRate = 60;
        // DEV ONLY
        if(StaticDataProvider.followers.Count == 0)
        {
            StaticDataProvider.AddRandomFollower(3);
        }
    }

    public void BeginGame()
    {
        StartCoroutine(DelayedSceneLoad());
    }
    IEnumerator DelayedSceneLoad()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(1);
    }

    public void Play()
    {
        DisplayDialog(startElement);
    }

    public void DisplayDialog(DialogeElement dialogeElement)
    {
        StartCoroutine(AnimatedDialogDisplay(dialogeElement));
        /*dialogCanvasLeft.SetActive(true);
        ClearAnswerButtons();
        dialogText.text = dialogeElement.dialogText;
        for (int i = 0; i < dialogeElement.followUps.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = dialogeElement.followUps[i].answer;
            answerButtons[i].GetComponent<DialogButton>().myDialogFollowup = dialogeElement.followUps[i].nextDialogElement;
            answerButtons[i].SetActive(true);
        }*/
    }

    public void ClearAnswerButtons()
    {
        foreach (GameObject item in answerButtons)
        {
            item.SetActive(false);
            item.GetComponent<DialogButton>().ResetButton();
            /*if (EventSystem.current.currentSelectedGameObject == item)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }*/
        }
    }

    public void NextDialog(DialogeElement dialogeElement, GameObject sender, DialogOptionModifyer modifyer)
    {
        Debug.Log(dialogeElement);
        ApplyDialogModifyer(modifyer);
        foreach (GameObject item in answerButtons)
        {
            if(item != sender)
            {
                item.SetActive(false);
            }
        }
        StartCoroutine(WaitBeforeNextDialog(dialogeElement));
    }

    public void ApplyDialogModifyer(DialogOptionModifyer modifyer)
    {
        StaticDataProvider.AddRadicallity(modifyer.radicalityChange);
        StaticDataProvider.OneTimeCollectiveMentalChange(modifyer.oneTimeMentalChange);
        eventToTriggerAfterDialogEnd.type = modifyer.outcomeEvent.type;
    }

    IEnumerator WaitBeforeNextDialog(DialogeElement dialogeElement)
    {
        // esetleg audio vagy valami
        yield return new WaitForSeconds(2);
        if (dialogeElement!=null)
        {
            DisplayDialog(dialogeElement);
        }
        else
        {
            Debug.Log("Dialog Ended!");
            FinishDialog();
        }
        
    }

    IEnumerator AnimatedDialogDisplay(DialogeElement dialogeElement)
    {
        dialogCanvasLeft.SetActive(true);
        ClearAnswerButtons();

        dialogText.text = "";
        for (int i = 0; i < dialogeElement.dialogText.Length; i++)
        {
            dialogText.text += dialogeElement.dialogText[i];
            yield return null;
        }
        dialogText.text = dialogeElement.dialogText;


        for (int i = 0; i < dialogeElement.followUps.Length; i++)
        {
            if (dialogeElement.followUps[i].requirement.IsMet())
            {
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = dialogeElement.followUps[i].answer;
                answerButtons[i].GetComponent<DialogButton>().myDialogFollowup = dialogeElement.followUps[i].nextDialogElement;
                answerButtons[i].GetComponent<DialogButton>().modifyer = dialogeElement.followUps[i].modifyer;
                answerButtons[i].SetActive(true);
                yield return null;
            }
            
        }
    }

    public void FinishDialog()
    {
        dialogCanvasLeft.SetActive(false);
        if (currentEvent)
        {
            switch (eventToTriggerAfterDialogEnd.type)
            {
                case OutcomeEventType.None:
                    break;
                case OutcomeEventType.SacrificeTheWeakest:
                    Sacrifice(StaticDataProvider.GetWeakestFollower());
                    break;
                case OutcomeEventType.SacrificeTheYungest:
                    Sacrifice(StaticDataProvider.GetYungestFollower());
                    break;
                case OutcomeEventType.BanishTheWeakest:
                    Banish(StaticDataProvider.GetWeakestFollower());
                    break;
            }
        }
        currentEvent = null;
    }

    public void StartSacrifice(Follower f)
    {
        sacrificeCanvas.SetActive(true);
        sacrificedFollowerNameText.text = f.name;
        sacrificeButton.currentFollower = f;
        //StaticDataProvider.followers.Remove(f);
    }
    public void StartSacrificeCurrent()
    {
        sacrificeCanvas.SetActive(true);
        sacrificedFollowerNameText.text = currentFollower.name;
        sacrificeButton.currentFollower = currentFollower;
    }

    public void StartBanish(Follower f)
    {
        banishCanvas.SetActive(true);
        banishButton.currentFollower = f;
        banishedFollowerNameText.text = f.name;
    }
    public void StartBanishCurrent()
    {
        banishCanvas.SetActive(true);
        banishButton.currentFollower = currentFollower;
        banishedFollowerNameText.text = currentFollower.name;
    }

    public void Sacrifice(Follower f)
    {
        sacrificeCanvasEnd.SetActive(true);
        sacrificedFollowerNameTextEnd.text = f.name;
        StaticDataProvider.followers.Remove(f);
    }

    public void Banish(Follower f)
    {
        banishCanvasEnd.SetActive(true);
        banishedFollowerNameTextEnd.text = f.name;
        StaticDataProvider.followers.Remove(f);
        StaticDataProvider.banishedPeople.Add(f);
    }

    // ha kiv�lasztottuk a random eventet ezzel �letreh�vjuk
    public void HandleRandomEvent(RandomEvent @event)
    {
        currentEvent = @event;
        DisplayDialog(@event.dialoge);
    }

    public void RefreshPeoplePanel()
    {

        //cleanup
        //int x = peopleCanvasContent.childCount;
        for (int i = 0; i < peopleCanvasContent.childCount; i++)
        {
            Destroy(peopleCanvasContent.GetChild(i).gameObject);
            //Debug.Log("Deleted People list prefab");
        }
        //populate
        foreach (Follower f in StaticDataProvider.followers)
        {
            //Debug.Log(f.name);
            GameObject o = Instantiate(peopleCanvasItemPrefab, peopleCanvasContent);
            o.GetComponent<FollowerListButton>().Setup(f);
        }
        peopleCanvas.SetActive(true);
        
    }

    public void RefreshPeoplePanelInfo()
    {
        pplNameText.text = currentFollower.name;
        pplStoryText.text = currentFollower.story;
        if (currentFollower.weapon != null)
        {
            weaponImage.gameObject.SetActive(true);
            weaponImage.sprite = currentFollower.weapon.itemIcon;
        }
        else
        {
            weaponImage.gameObject.SetActive(false);
        }
        extraOptions.SetActive(true);
    }

    public void RefreshRationsPanel()
    {
        rationsCanvas.SetActive(true);

        //cleanup
        for (int i = 0; i < rationsCanvasContent.childCount; i++)
        {
            Destroy(rationsCanvasContent.GetChild(i).gameObject);
        }

        //populate
        foreach (Follower f in StaticDataProvider.followers)
        {
            GameObject o = Instantiate(rationsCanvasItemPrefab, rationsCanvasContent);
            o.GetComponent<FollowerListButton>().Setup(f);
        }

        totalConsumptionText.text = StaticDataProvider.CalculateFoodConsumption() + "";
    }

    public void RefreshRationsPanelInfo()
    {
        pplNameText.text = currentFollower.name;
        pplStoryText.text = currentFollower.story;
        totalConsumptionText.text = StaticDataProvider.CalculateFoodConsumption() + "";
        extraOptionsRations.SetActive(true);
    }

    public void RefreshInventoryCanvas()
    {
        inventoryCanvas.SetActive(true);

        //cleanup
        for (int i = 0; i < inventoryCanvasContent.childCount; i++)
        {
            Destroy(inventoryCanvasContent.GetChild(i).gameObject);
        }

        //populate
        foreach (InventoryItem f in allCraftablesInGame)
        {
            GameObject o = Instantiate(inventoryCanvasItemPrefab, inventoryCanvasContent);
            o.GetComponent<InventoryItemHolder>().Setup(f);
        }
    }

    public void RefreshInventoryPanelInfo()
    {
        costText.text = "Cost: " + currentItem.cost + " material";
        descText.text = currentItem.description;
        itemNameText.text = currentItem.name + " " + StaticDataProvider.CountInventoryItem(currentItem) + "x";
        extraOptionsInventory.SetActive(true);
    }

    public void RefreshTaskCanvas()
    {
        tasksCanvas.SetActive(true);

        //cleanup
        for (int i = 0; i < idleContent.childCount; i++)
        {
            Destroy(idleContent.GetChild(i).gameObject);
        }
        for (int i = 0; i < workContent.childCount; i++)
        {
            Destroy(workContent.GetChild(i).gameObject);
        }
        for (int i = 0; i < scoutContent.childCount; i++)
        {
            Destroy(scoutContent.GetChild(i).gameObject);
        }

        //populate
        foreach (Follower f in StaticDataProvider.followers)
        {
            if(f.daylyTask != FollowerTask.Idle) { continue; }
            GameObject o = Instantiate(tasksCanvasItemPrefab, idleContent);
            o.GetComponent<FollowerListButton>().Setup(f);
        }
        foreach (Follower f in StaticDataProvider.followers)
        {
            if (f.daylyTask != FollowerTask.Worker) { continue; }
            GameObject o = Instantiate(tasksCanvasItemPrefab, workContent);
            o.GetComponent<FollowerListButton>().Setup(f);
        }
        foreach (Follower f in StaticDataProvider.followers)
        {
            if (f.daylyTask != FollowerTask.Scout) { continue; }
            GameObject o = Instantiate(tasksCanvasItemPrefab, scoutContent);
            o.GetComponent<FollowerListButton>().Setup(f);
        }
    }


    public void AssignCurrentToTask(int taskID)
    {
        if (currentFollower == null) { return; }

        switch ((FollowerTask)taskID)
        {
            case FollowerTask.Idle:
                currentFollower.daylyTask = FollowerTask.Idle;
                break;
            case FollowerTask.Worker:
                currentFollower.daylyTask = FollowerTask.Worker;
                break;
            case FollowerTask.Scout:
                if(StaticDataProvider.strikeTeam.Count > 5)
                {
                    break;
                }
                currentFollower.daylyTask = FollowerTask.Scout;
                UpdateStrikeTeam();
                break;
            default:
                break;
        }

        RefreshTaskCanvas();
    }

    public void SetRationForCurrent()
    {
        currentFollower.dayliRation = (int)rationSlider.value;
        for (int i = 0; i < rationsCanvasContent.childCount; i++)
        {
            rationsCanvasContent.GetChild(i).GetComponent<FollowerListButton>().Refresh();
        }
    }

    public void UpdateRationSliderText()
    {
        rationSliderText.text = rationSlider.value + " Kcal";
    }

    public void SetSelectedFollower(Follower f)
    {
        currentFollower = f;

        if (rationsCanvas)
        {
            if (rationsCanvas.activeSelf)
            {
                rationSlider.value = f.dayliRation;
                rationSliderText.text = f.dayliRation + " Kcal";
            }
        }
    }

    public void SetSelectedInventoryItem(InventoryItem i)
    {
        currentItem = i;
        RefreshInventoryPanelInfo();
    }

    public void ClearSelectedInventoryItem()
    {
        currentItem = null;
        extraOptionsInventory.SetActive(false);
    }

    public void ClearSelectedFollower()
    {
        extraOptions.SetActive(false);
        extraOptionsRations.SetActive(false);
        currentFollower = null;
    }

    public void ShowMapInfo()
    {
        mapImage.sprite = currentMap.image;
        areaNameText.text = currentMap.name;
        areaDescText.text = currentMap.description;
    }

    public void SetSelectedMapInfo(Map m)
    {
        currentMap = m;
    }

    public void UpdateStrikeTeam()
    {
        StaticDataProvider.strikeTeam.Clear();
        foreach (Follower f in StaticDataProvider.followers)
        {
            if(f.daylyTask == FollowerTask.Scout)
            {
                StaticDataProvider.strikeTeam.Add(f);
            }
        }
    }

    public int CountOfCrafters()
    {
        int i = 0;
        foreach (Follower f in StaticDataProvider.followers)
        {
            if (f.daylyTask == FollowerTask.Worker)
            {
                i++;
            }
        }
        return i;
    }

    public void LaunchMission()
    {
        UpdateStrikeTeam();
        if(StaticDataProvider.strikeTeam.Count < 1)
        {
            // ide valami error window
            return;
        }

        SceneManager.LoadScene(currentMap.scene.buildIndex);
    }

    public void NextDay()
    {
        StaticDataProvider.FeedPopulation();
        StaticDataProvider.daysPassed++;

        if(StaticDataProvider.followers.Count == 0)
        {
            Debug.LogError("YOU LOST");
        }
    }

    public void AddToCraftingQeue()
    {
        if(StaticDataProvider.craftingQueue.Count <= CountOfCrafters())
        {
            // ide valami error windowt
            Debug.Log("Additional supplydepots required!");
            return;
        }
        if (currentItem.cost > StaticDataProvider.material)
        {
            // ide valami error windowt
            Debug.Log("Not enugh minerals!");
            return;
        }

        StaticDataProvider.craftingQueue.Add(currentItem);
    }

    public void CraftAll()
    {
        for (int i = 0; i < StaticDataProvider.craftingQueue.Count; i++)    // az�rt nem foreach mer duplik�lt elemek el�fordulhatnak
        {
            StaticDataProvider.material -= StaticDataProvider.craftingQueue[i].cost;

            StaticDataProvider.inventoryItems.Add(StaticDataProvider.craftingQueue[i]);
            StaticDataProvider.craftingQueue.RemoveAt(i);
        }
    }
    
}
