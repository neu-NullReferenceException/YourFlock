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


    private void Start()
    {
        Application.targetFrameRate = 30;
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

    public void StartBanish(Follower f)
    {
        banishCanvasEnd.SetActive(true);
        banishButton.currentFollower = f;
        banishedFollowerNameTextEnd.text = f.name;
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

}
