using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtractionZone : MonoBehaviour
{
    public RTSManager manager;
    public GameObject leaveCanvas;
    public GameObject leaveWarning;

    [SerializeField] private List<Follower> inZoneFollowers = new List<Follower>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<RTSUnit>(out RTSUnit unit))
        {
            if (!inZoneFollowers.Contains(unit.myFollower))
            {
                inZoneFollowers.Add(unit.myFollower);
                leaveCanvas.SetActive(true);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<RTSUnit>(out RTSUnit unit))
        {
            if (inZoneFollowers.Contains(unit.myFollower))
            {
                inZoneFollowers.Remove(unit.myFollower);
                if(inZoneFollowers.Count < 1)
                {
                    leaveCanvas.SetActive(false);
                }
            }
        }
    }

    private List<Follower> GetLeftBehindFollowers()
    {
        List<Follower> left = new List<Follower>();
        foreach (Follower f in StaticDataProvider.strikeTeam)
        {
            if (!inZoneFollowers.Contains(f))
            {
                left.Add(f);
            }
        }

        return left;
    }

    public void ShowLeaveWarning()
    {
        leaveWarning.SetActive(true);
    }

    public void Leave()
    {
        List<Follower> left = GetLeftBehindFollowers();
        if (left.Count > 0)
        {
            foreach (Follower f in left)
            {
                StaticDataProvider.DieInConbat(f);
            }
        }
        SceneManager.LoadScene(1);
    }
}
