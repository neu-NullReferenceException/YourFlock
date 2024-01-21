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
            cmdcButtons[i].GetComponentInChildren<CMDCButton>().unit = SpawnUnit(i);
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
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && lastTouchDuration < maxTouchLengthBeforeHold) // && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            pos.z = 0;
            selectedUnit.MoveTo(pos);
        }
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
}
