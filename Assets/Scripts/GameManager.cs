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

    public List<InventoryItem> allCraftablesInGame = new List<InventoryItem>();
    public InventoryItem[] startingItems;

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

    public GameObject errorWindow;
    public TextMeshProUGUI errorText;

    public StaticDataExtender staticDataExtender;
    public Animator menuAnimator;

    [Space(5)]
    [Header("Main Screen References")]
    public GameObject peopleCanvas;
    public GameObject rationsCanvas;
    public GameObject measuresCanvas;
    public GameObject inventoryCanvas;
    public GameObject tasksCanvas;
    [Header("Canvas Item Prefabs")] // minden ami listázáshoz kell prefab
    public GameObject peopleCanvasItemPrefab;
    public GameObject rationsCanvasItemPrefab;
    public GameObject inventoryCanvasItemPrefab;
    public GameObject tasksCanvasItemPrefab;
    [Header("Canvas Contents")] // ezek alá kell a lista elemeket parentelni példányosítás után
    public Transform peopleCanvasContent;
    public Transform rationsCanvasContent;
    public Transform inventoryCanvasContent;
    //public Transform tasksCanvasContent;

    private Follower currentFollower;
    private InventoryItem currentItem;
    private Map currentMap;
    private Measure currentMesure;
    private bool nextDayFlag = false;

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
    [SerializeField] private GameObject mapDetails;
    [Header("Resource Display Fields")]
    [SerializeField] private TextMeshProUGUI materialText;
    [SerializeField] private TextMeshProUGUI popText;
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI dayText;
    [Header("WeaponSelect Panel Fields")]
    [SerializeField] private Transform weaponListContent;
    [SerializeField] private GameObject weaponSelectPanelExtraOptions;
    [SerializeField] private GameObject weaponSelectPanel;
    [SerializeField] private TextMeshProUGUI weaponSelectText;
    [SerializeField] private TextMeshProUGUI weaponDescText;
    [Header("Measure Detail Panel Fields")]
    [SerializeField] private TextMeshProUGUI measureNameText;
    [SerializeField] private TextMeshProUGUI measureDescText;
    [SerializeField] private TextMeshProUGUI conflictNameText;
    [SerializeField] private TextMeshProUGUI requirementNameText;
    [SerializeField] private GameObject measureDetails;

    [Header("Other Stuff")]
    public GameObject[] measureRepresenterButtons;
    public InventoryItem sawdustMeal;
    public AudioSource musicAudio;

    private void Start()
    {
        Application.targetFrameRate = 60;
        
        // DEV ONLY
        if (StaticDataProvider.isFirstTime)
        {
            staticDataExtender.InjectData();

            StaticDataProvider.isFirstTime = false;
            if (StaticDataProvider.followers.Count == 0)
            {
                StaticDataProvider.AddRandomFollower(3);
            }

            foreach (InventoryItem item in startingItems)
            {
                StaticDataProvider.inventoryItems.Add(item);
            }
        }
        if (StaticDataProvider.foodRecipe)
        {
            allCraftablesInGame.Add(StaticDataProvider.foodRecipe);
        }
        //rationSlider.stepSize
        if (SaveLoader.isSaveFile(new DinamicData(),"gameState.nre"))
        {
            StaticDataProvider.loadGame();
        }
        UpdateResourceMetrics();
        SetStaticAudioLevel();
    }

    public void BeginGame()
    {
        menuAnimator.Play("ToWhite");
        StartCoroutine(DelayedSceneLoad());
    }
    IEnumerator DelayedSceneLoad()
    {
        yield return new WaitForSeconds(4);
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
        int l = 0;
        while (l < dialogeElement.dialogText.Length)
        {
            if (Input.touchCount > 0)
            {
                if(Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    break;
                }
                
            }
            dialogText.text += dialogeElement.dialogText[l];
            l++;
            yield return null;
        }
        /*for (int i = 0; i < dialogeElement.dialogText.Length; i++)
        {
            dialogText.text += dialogeElement.dialogText[i];
            yield return null;
        }*/
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
    public void StartSacrificeCurrent(Requirement requirement)
    {
        if (!requirement.IsMet())
        {
            ShowErrorWindow("You need to introduce the " + requirement.requiredLaw.name + " measure to do this.");
            return;
        }
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
    public void StartBanishCurrent(Requirement requirement)
    {
        if (!requirement.IsMet())
        {
            ShowErrorWindow("You need to introduce the " + requirement.requiredLaw.name + " measure to do this.");
            return;
        }
        banishCanvas.SetActive(true);
        banishButton.currentFollower = currentFollower;
        banishedFollowerNameText.text = currentFollower.name;
    }

    public void Sacrifice(Follower f)
    {
        sacrificeCanvasEnd.SetActive(true);
        sacrificedFollowerNameTextEnd.text = f.name;
        StaticDataProvider.followers.Remove(f);
        UpdateResourceMetrics();
    }

    public void Banish(Follower f)
    {
        banishCanvasEnd.SetActive(true);
        banishedFollowerNameTextEnd.text = f.name;
        StaticDataProvider.followers.Remove(f);
        StaticDataProvider.banishedPeople.Add(f);
        UpdateResourceMetrics();
    }

    // ha kiválasztottuk a random eventet ezzel életrehívjuk
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
        pplStoryText.text = "Stats:\nStrength: " + currentFollower.strength + "\nPerception: " + currentFollower.perception + "";
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

    public void RefreshWeaponSelectPanelInfo()
    {
        weaponDescText.text = currentItem.description;
        itemNameText.text = currentItem.name; //+ " " + StaticDataProvider.CountInventoryItem(currentItem) + "x";
        weaponSelectPanelExtraOptions.SetActive(true);
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

    public void RefreshWeaponSelect()
    {
        weaponSelectPanel.SetActive(true);

        Dictionary<string, int> stock = new Dictionary<string, int>();
        foreach (InventoryItem item in StaticDataProvider.inventoryItems)
        {
            if(item.type == ItemType.Weapon)
            {
                if (stock.ContainsKey(item.name))
                {
                    stock[item.name]++;
                }
                else
                {
                    stock.Add(item.name, 1);
                }
            }
        }
        foreach(Follower f in StaticDataProvider.followers)
        {
            if(f.weapon != null)
            {
                if (stock.ContainsKey(f.weapon.name)) { stock[f.weapon.name]--; }  
            }
        }

        //cleanup
        for (int i = 0; i < weaponListContent.childCount; i++)
        {
            Destroy(weaponListContent.GetChild(i).gameObject);
        }

        foreach (KeyValuePair<string,int> item in stock)
        {
            if(item.Value > 0)
            {
                //GameObject o = Instantiate(weaponListUIPrefab, weaponListContent);
                GameObject o = Instantiate(inventoryCanvasItemPrefab, weaponListContent);
                o.GetComponent<InventoryItemHolder>().Setup(GetFirstInventoryItemFromName(item.Key), item.Value);
            }
        }
    }

    public void SetSelectedWeaponForSelectedFollower()
    {
        currentFollower.weapon = currentItem;
        ClearSelectedInventoryItem();
        RefreshPeoplePanelInfo();
    }

    private InventoryItem GetFirstInventoryItemFromName(string name)
    {
        foreach (InventoryItem item in StaticDataProvider.inventoryItems)
        {
            if(item.name == name)
            {
                return item;
            }
        }
        return null;
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
                if (StaticDataProvider.strikeTeam.Count > 4 || currentFollower.weapon == null)
                {
                    // valami error windowt ide
                    ShowErrorWindow("The exploration team can consist of up to 4 people!\nEach member must be armed!");
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
                //extraOptionsRations.SetActive(true);
                rationSlider.value = f.dayliRation;
                rationSliderText.text = f.dayliRation + " Kcal";
            }
        }
    }

    public void SetSelectedInventoryItem(InventoryItem i)
    {
        currentItem = i;
        if (inventoryCanvas.activeInHierarchy)
        {
            RefreshInventoryPanelInfo();
        }
        if (weaponSelectPanel.activeInHierarchy)
        {
            RefreshWeaponSelectPanelInfo();
        }
    }

    public void ClearSelectedInventoryItem()
    {
        currentItem = null;
        extraOptionsInventory.SetActive(false);
        weaponSelectPanelExtraOptions.SetActive(false);
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
        mapDetails.SetActive(true);
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
            ShowErrorWindow("The expolarition team is empty!\nYou should assign some 'volunteers'!");
            return;
        }

        SceneManager.LoadScene(currentMap.sceneBuildIndex);
    }

    public void NextDay()
    {
        if (nextDayFlag) { return; }
        nextDayFlag = true;
        menuAnimator.Play("NextDay");
        StaticDataProvider.FeedPopulation();
        StaticDataProvider.daysPassed++;
        StaticDataProvider.isLawAwailable = true;
        List<Follower> ppl = StaticDataProvider.CheckStarveToDeath();
        if (ppl.Count > 0)
        {
            string outp = "";
            foreach (Follower item in ppl)
            {
                outp += item.name + ", ";
            }
            ShowErrorWindow(outp + " starved to death!");
        }

        if(StaticDataProvider.followers.Count == 0)
        {
            Debug.Log("YOU LOST");
            SceneManager.LoadScene(7);
        }
        CraftAll();
        StartCoroutine(NextDayDelay());
        StaticDataProvider.saveGame();
    }

    IEnumerator NextDayDelay()
    {
        yield return new WaitForSeconds(3);
        UpdateResourceMetrics();
        yield return new WaitForSeconds(1);
        nextDayFlag = false;
    }

    public void UpdateResourceMetrics()
    {
        if (!materialText) { return; }
        materialText.text = StaticDataProvider.material + "";
        foodText.text = StaticDataProvider.food + " Kcal";
        popText.text = StaticDataProvider.followers.Count + "";
        dayText.text = StaticDataProvider.daysPassed + "";
    }

    public void AddToCraftingQeue()
    {
        if(StaticDataProvider.craftingQueue.Count >= CountOfCrafters())
        {
            // ide valami error windowt
            ShowErrorWindow("Not enough assigned workers!\nYou can only carft one item / worker / day.\nYou can assign workers on the task panel.");
            Debug.Log(CountOfCrafters() + "/" + StaticDataProvider.craftingQueue.Count);
            Debug.Log("Additional supplydepots required!");
            return;
        }
        if (currentItem.cost > StaticDataProvider.material)
        {
            // ide valami error windowt
            ShowErrorWindow("Not enugh materials!");
            Debug.Log("Not enugh minerals!");
            return;
        }

        StaticDataProvider.craftingQueue.Add(currentItem);
    }

    public void CraftAll()
    {
        /*for (int i = 0; i < StaticDataProvider.craftingQueue.Count; i++)    // azért nem foreach mer duplikált elemek elõfordulhatnak
        {
            StaticDataProvider.material -= StaticDataProvider.craftingQueue[i].cost;

            StaticDataProvider.inventoryItems.Add(StaticDataProvider.craftingQueue[i]);
            StaticDataProvider.craftingQueue.RemoveAt(i);
        }*/
        int c = StaticDataProvider.craftingQueue.Count;
        if(c == 0)
        {
            return;
        }
        if(c > CountOfCrafters())
        {
            c = CountOfCrafters();
        }

        for (int i = 0; i < CountOfCrafters(); i++)    // ha közben valakit kivettünk
        {
            StaticDataProvider.material -= StaticDataProvider.craftingQueue[i].cost;
            if(StaticDataProvider.craftingQueue[i] == sawdustMeal)
            {
                StaticDataProvider.AddFood(1000);
            }
            else
            {
                StaticDataProvider.inventoryItems.Add(StaticDataProvider.craftingQueue[i]);
            }
            StaticDataProvider.craftingQueue.RemoveAt(i);
        }
        StaticDataProvider.craftingQueue.Clear();
    }
    

    public void ShowErrorWindow(string text)
    {
        errorText.text = text;
        errorWindow.SetActive(true);
    }

    public void ShowMeasureDetails(Measure m)
    {
        currentMesure = m;
        measureDetails.SetActive(true);
        measureNameText.text = m.name;
        measureDescText.text = m.description;
        if (m.requiredToUnlock)
        {
            requirementNameText.text = m.requiredToUnlock.name;
        }
        else
        {
            requirementNameText.text = "None";
        }
        if (m.conflictsWith)
        {
            conflictNameText.text = m.conflictsWith.name;
        }
        else
        {
            conflictNameText.text = "None";
        }
    }

    public void IntroduceMeasure()
    {
        //!! VALAMI VIZUÁLIS CUCC KÉNE ENNEK A SZEMLÉLTÉSÉRE HOGY EZT MÁR AKTIVÁLTUK
        if (StaticDataProvider.passedLaws.Contains(currentMesure))
        {
            ShowErrorWindow("This measure or ideology has already been activated!");
            return;
        }
        if (!StaticDataProvider.isLawAwailable)
        {
            ShowErrorWindow("Only one measure per day can be introduced!");
            return;
        }
        if (StaticDataProvider.MeasureMeetsRequirement(currentMesure))
        {
            StaticDataProvider.passedLaws.Add(currentMesure);
            foreach (GameObject item in measureRepresenterButtons)
            {
                if(item.GetComponent<MeasureHolder>().measure == currentMesure)
                {
                    item.GetComponent<Outline>().enabled = true;
                }
            }
            StaticDataProvider.isLawAwailable = false;
            // kiértékel
            StaticDataProvider.defaultDeathMentalChange += currentMesure.mentalChangeModifyer;
            StaticDataProvider.defaultFoodConsumption += currentMesure.consumptionChange;
            if (currentMesure.providedRecipe)
            {
                StaticDataProvider.foodRecipe = currentMesure.providedRecipe;
                allCraftablesInGame.Add(currentMesure.providedRecipe);
            }
            if (currentMesure.lastStandAbilityUnlocked)
            {
                StaticDataProvider.lastStandAbilityUnlocked = true;
            }
            if (currentMesure.emergencyRationsUnlocked)
            {
                StaticDataProvider.emergencyRationsUnlocked = true;
            }
            currentMesure = null;
        }
        else
        {
            ShowErrorWindow("Requirements do not match");
        }
    }

    public void RefreshMeasurePanel()
    {
        foreach (Measure item in StaticDataProvider.passedLaws) 
        {
            foreach (GameObject g in measureRepresenterButtons)
            {
                if (g.GetComponent<MeasureHolder>().measure == item)
                {
                    g.GetComponent<Outline>().enabled = true;
                }
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetAudioLevel(Slider f)
    {
        StaticDataProvider.musicVolume = f.value;
        if (musicAudio)
        {
            musicAudio.volume = f.value;
        }
    }

    public void SetStaticAudioLevel()
    {
        if (musicAudio)
        {
            musicAudio.volume = StaticDataProvider.musicVolume;
        }
    }
}
