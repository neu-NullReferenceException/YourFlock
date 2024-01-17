using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FollowerListButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI rationText;
    [SerializeField] private Image profilePic;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider mentalBar;
    [SerializeField] private Slider foodBar;
    public Follower myFollower;

    public void Setup(Follower f)
    {
        myFollower = f;
        nameText.text = f.name;
        if (profilePic) { profilePic.sprite = f.profilePic; }
        if (healthBar) { healthBar.value = f.health; }
        if (rationText) { rationText.text = f.dayliRation + " Kcal"; }
        if (mentalBar) { mentalBar.value = f.mentalState; }
        if (foodBar) { foodBar.value = f.food; }
    }

    public void Refresh()
    {
        if (profilePic) { profilePic.sprite = myFollower.profilePic; }
        if (healthBar) { healthBar.value = myFollower.health; }
        if (rationText) { rationText.text = myFollower.dayliRation + " Kcal"; }
        if (mentalBar) { mentalBar.value = myFollower.mentalState; }
        if (foodBar) { foodBar.value = myFollower.food; }
    }

    public void SetSelectedFollower()
    {
        GameObject.Find("MANAGER").GetComponent<GameManager>().SetSelectedFollower(myFollower);
    }

    public void RefreshPeoplePanelInfo()
    {
        GameObject.Find("MANAGER").GetComponent<GameManager>().RefreshPeoplePanelInfo();
    }

    public void RefreshRationsPanelInfo()
    {
        GameObject.Find("MANAGER").GetComponent<GameManager>().RefreshRationsPanelInfo();
    }

    public void RefreshTasksPanelInfo()
    {
        //GameObject.Find("MANAGER").GetComponent<GameManager>().SetSelectedFollower(myFollower);
    }
}
