using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemSlot : MonoBehaviour
{
    public string ButtonType;
    public int ID;

    public void ButtonPressed()
    {
        FindObjectOfType<CustomisationUIManager>().EquipItem(ButtonType, ID);
    }

    public GameObject select;
    public void HoverEnter()
    {
        select.SetActive(true);
    }

    public void HoverExit()
    {
        select.SetActive(false);
    }
}
