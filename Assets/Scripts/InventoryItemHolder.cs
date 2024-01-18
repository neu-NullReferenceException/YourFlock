using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryItemHolder : MonoBehaviour
{
    public InventoryItem myItem;
    public Image itemImage;
    public TextMeshProUGUI itemNameText;

    public void Setup(InventoryItem i)
    {
        myItem = i;
        itemImage.sprite = i.itemIcon;
        itemNameText.text = myItem.name + " " + StaticDataProvider.CountInventoryItem(i) + "x";

    }

    public void SetSelectedInventoryItem()
    {
        GameObject.Find("MANAGER").GetComponent<GameManager>().SetSelectedInventoryItem(myItem);
    }
}
