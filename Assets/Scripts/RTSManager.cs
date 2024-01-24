using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public float maxTouchLengthBeforeHold = 0.25f;

    private bool touchInProgress;
    private float lastTouchDuration;
    private float touchStartTime;

    // Start is called before the first frame update
    void Start()
    {
        SetupCMDCButtons();
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
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && lastTouchDuration < maxTouchLengthBeforeHold) // && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            pos.z = 0;
            selectedUnit.MoveTo(pos);
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
}
