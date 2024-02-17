using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RTSManager : MonoBehaviour
{
    public GameObject[] cmdcButtons; // MAX 4!!!!!!
    public InventoryItem DEFAULTWEAPON;
    public Transform buttonGroupHolder;
    private int selectedButtonSiblingIndex = -1;
    public RTSUnit selectedUnit;

    public GameObject unitPrefab;
    public Transform[] unitSpawns;
    //public LayerMask ignoreLayer;
    public GameObject miniAlertPrefab;
    public Transform alertContent;

    public Transform Chests;
    public float ChestThiccness;
    public Canvas ItemDialog;
    public Image ItemDialogImage;
    public TextMeshProUGUI ItemDialogText;
    public TextMeshProUGUI ItemDialogDescription;
    public InventoryItem[] loot;

    public float maxTouchLengthBeforeHold = 0.25f;

    private bool touchInProgress;
    private float lastTouchDuration;
    private float touchStartTime;
    private float timeOnArrival;

    // Start is called before the first frame update
    void Start()
    {
        SetupCMDCButtons();
        timeOnArrival = Time.time;
    }

    public void SetupCMDCButtons()
    {
        foreach(GameObject o in cmdcButtons)
        {
            o.SetActive(false);
        }

        for (int i = 0; i < StaticDataProvider.strikeTeam.Count; i++)
        {
            if(StaticDataProvider.strikeTeam[i].weapon == null)
            {
                StaticDataProvider.strikeTeam[i].weapon = DEFAULTWEAPON;
            }

            cmdcButtons[i].GetComponentInChildren<CMDCButton>().myFollower = StaticDataProvider.strikeTeam[i];
            cmdcButtons[i].GetComponentInChildren<CMDCButton>().Setup();
            RTSUnit unit = SpawnUnit(i);
            unit.myFollower = StaticDataProvider.strikeTeam[i];
            unit.Setup();
            cmdcButtons[i].GetComponentInChildren<CMDCButton>().unit = unit;
            cmdcButtons[i].SetActive(true);
        }
    }

    public RTSUnit SpawnUnit(int spawnPosID)
    {
        GameObject unit = Instantiate(unitPrefab, unitSpawns[spawnPosID].position, unitSpawns[spawnPosID].rotation);
        return unit.GetComponentInChildren<RTSUnit>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedUnit)
        {
            CalculateTouchLenght();
            HandleUnitInput();
        }
        
    }

    public void CalculateTouchLenght()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchInProgress = true;
                    touchStartTime = Time.time;
                    break;

                case TouchPhase.Ended:
                    touchInProgress = false;
                    lastTouchDuration = Time.time - touchStartTime;
                    break;
            }
        }
    }

    public void HandleUnitInput()
    {
        // ahoz hogy más objektumokkal lehessen interactolni (pl láda kinyitása a pályán) kéne egy raycast a kamerából e helyett
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && lastTouchDuration < maxTouchLengthBeforeHold)
        {
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            touchPosition.z = 0;

            Transform closestChest = null;
            float closestDistance = float.MaxValue;

            foreach (Transform chest in Chests)
            {
                float distance = Vector3.Distance(chest.position, touchPosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestChest = chest;
                }
            }

            if (closestChest != null && closestDistance < ChestThiccness)
            {
                selectedUnit.MoveTo(closestChest.position);
                Debug.Log("Chest");
                // Ki kell nyitni a chestet ha ott can a character!
                StartCoroutine(CheckIfReachedDestination(closestChest,selectedUnit.transform));
            }
            else
            {
                //Debug.Log("No Chest");
                selectedUnit.MoveTo(touchPosition);
            }
        }
    }

    private IEnumerator CheckIfReachedDestination(Transform Chest, Transform unit)
    {
        while (true)
        {
            //Debug.Log("Waiting for chest!!!");
            yield return null;
            
            float distanceToChest = Vector2.Distance(new Vector2(unit.position.x, unit.position.y), new Vector2(Chest.position.x, Chest.position.y));
            //Debug.Log("Player " + unit.position.x + " " + unit.position.y + " " + unit.position.z + " Chest " + Chest.position.x + " " + Chest.position.y + " " + Chest.position.z+" Distance "+distanceToChest);
            //Debug.Log(distanceToChest);
            float stoppingDistance = 0.5f;

            if (distanceToChest < stoppingDistance)
            {
                if (!Chest.GetComponent<ChestController>().isOpenned)
                {
                    Debug.Log("Got to chest!");
                    Chest.GetComponent<ChestController>().open();
                    PickRandomUpItem();
                }
                break;
            }
        }
    }

    public void ShowMiniAlert(string alertText)
    {
        GameObject g = Instantiate(miniAlertPrefab, alertContent);
        g.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = alertText;
    }

    public void SelectUnit(int siblingIndex)
    {
        foreach (GameObject g in cmdcButtons)
        {
            if (!g.activeInHierarchy) { continue; }
            if(g.transform.GetSiblingIndex() != siblingIndex)
            {
                g.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Deselect");
                g.transform.GetChild(0).GetComponent<Animator>().ResetTrigger("Select");
            }
            else if(siblingIndex != selectedButtonSiblingIndex)
            {
                g.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Select");
                g.transform.GetChild(0).GetComponent<Animator>().ResetTrigger("Deselect");
                selectedButtonSiblingIndex = siblingIndex;
                selectedUnit = g.transform.GetComponentInChildren<CMDCButton>().unit;
            }
            else
            {
                g.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Deselect");
                g.transform.GetChild(0).GetComponent<Animator>().ResetTrigger("Select");
                selectedButtonSiblingIndex = -1;
                selectedUnit = null;
            }
        }
    }

    public void LooseUnit(Follower f)
    {
        foreach(GameObject g in cmdcButtons)
        {
            if(g.GetComponentInChildren<CMDCButton>().myFollower == f)
            {
                g.SetActive(false);
            }
        }
        if(selectedUnit.myFollower == f)
        {
            selectedUnit = null;
        }
        ShowMiniAlert(f.name + " died!");
        if (StaticDataProvider.strikeTeam.Contains(f))
        {
            StaticDataProvider.strikeTeam.Remove(f);
        }

        if (!CheckFollowerAlive())
        {
            SceneManager.LoadScene(1);
        }
    }

    public bool CheckFollowerAlive()
    {
        if(StaticDataProvider.strikeTeam.Count > 0)
        {
            return true;
        }

        return false;
    }

    public void UpdateUI()
    {
        foreach (GameObject g in cmdcButtons)
        {
            if (g.activeInHierarchy)
            {
                g.GetComponentInChildren<CMDCButton>().UpdateHealthbar();
            }
            
        }
    }

    private void PickRandomUpItem()
    {
        int index = UnityEngine.Random.Range(0, loot.Length);
        InventoryItem chosenLoot = loot[index];
        if (chosenLoot != null)
        {
            if (chosenLoot.type == ItemType.Food)
            {
                ItemDialogText.text = chosenLoot.name + " " + chosenLoot.cost + "kcal";
                ItemDialogDescription.text = chosenLoot.description;
                StaticDataProvider.food += chosenLoot.cost;
                ItemDialog.gameObject.SetActive(true);
                ItemDialogImage.sprite = chosenLoot.itemIcon;
            }
            else if (chosenLoot.type == ItemType.Material)
            {
                ItemDialogText.text = chosenLoot.name + " " + chosenLoot.cost + "pcs";
                ItemDialogDescription.text = chosenLoot.description;
                StaticDataProvider.material += chosenLoot.cost;
                ItemDialogImage.sprite = chosenLoot.itemIcon;
                ItemDialog.gameObject.SetActive(true);
            }
        }
    }

    public void PrepareLeave()
    {
        if(Time.time - timeOnArrival > 60)
        {
            StaticDataProvider.AddRandomFollower(1);
            StaticDataProvider.hasNewFollower = true;
        }
    }
}
