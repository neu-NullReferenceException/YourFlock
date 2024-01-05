using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CMDCButton : MonoBehaviour
{
    public Slider healthbar;
    public Image portraitImage;
    public Image weaponImage;
    public TextMeshProUGUI nameText;

    public Follower myFollower;
    public RTSUnit unit;

    private void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 1f;
    }

    public void Setup()
    {
        healthbar.value = myFollower.health;
        portraitImage.sprite = myFollower.profilePic;
        nameText.text = myFollower.name;
        weaponImage.sprite = myFollower.weapon.itemIcon;
    }
}
