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
    [Header("Canvas Item Prefabs")] // minden ami listázáshoz kell prefab
    public GameObject peopleCanvasItemPrefab;
    public GameObject rationsCanvasItemPrefab;
    public GameObject inventoryCanvasItemPrefab;
    public GameObject tasksCanvasItemPrefab;
    [Header("Canvas Contents")] // ezek alá kell a lista elemeket parentelni példányosítás után
    public Transform peopleCanvasContent;
    public Transform rationsCanvasContent;
    public Transform inventoryCanvasContent;
    public Transform tasksCanvasContent;

    private Follower currentFollower;
    [Header("People Panel Fields")]
    public TextMeshProUGUI pplNameText;
    public TextMeshProUGUI pplStoryText;
    public Image weaponImage;
    public GameObject extraOptions;
    [Header("Ration Panel Fields")]
    public TextMeshProUGUI totalConsumptionText;
    public Slider rationSlider;
    public TextMeshProUGUI rationSliderText;
    public GameObject extraOptionsRations;



    private void Start()
    {
        Application.targetFrameRate = 30;
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

    // ha kiválasztottuk a random eventet ezzel életrehívjuk
    public void HandleRandomEvent(RandomEvent @event)
    {
        currentEvent = @event;
        DisplayDialog(@event.dialoge);
    }

    public void RefreshPeoplePanel()
    {
        peopleCanvas.SetActive(true);

        //cleanup
        for (int i = 0; i < peopleCanvasContent.childCount; i++)
        {
            Destroy(peopleCanvasContent.GetChild(0).gameObject);
        }

        //populate
        foreach (Follower f in StaticDataProvider.followers)
        {
            GameObject o = Instantiate(peopleCanvasItemPrefab, peopleCanvasContent);
            o.GetComponent<FollowerListButton>().Setup(f);
        }
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
            Destroy(rationsCanvasContent.GetChild(0).gameObject);
        }

        //populate
        foreach (Follower f in StaticDataProvider.followers)
        {
            GameObject o = Instantiate(rationsCanvasItemPrefab, rationsCanvasContent);
            o.GetComponent<FollowerListButton>().Setup(f);
        }
    }

    public void RefreshRationsPanelInfo()
    {
        pplNameText.text = currentFollower.name;
        pplStoryText.text = currentFollower.story;
        extraOptionsRations.SetActive(true);
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
    }

    public void ClearSelectedFollower()
    {
        extraOptions.SetActive(false);
        extraOptionsRations.SetActive(false);
        currentFollower = null;
    }

    
}
