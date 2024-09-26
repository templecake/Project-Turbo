using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMainMenuUI : MonoBehaviour
{
    public int MenuSelected;
    //0 - PLAY, 1 - GARAGE, 2 - STORE, 3 - NEWS, 4 - OPTIONS
    public GameObject[] Menus;
    GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        SelectMenu(gm.LastMenuOpened);
    }

    public void SelectMenu(int selected)
    {
        MenuSelected = selected;
        UpdateUI();
        gm.LastMenuOpened = selected;
    }

    public void UpdateUI()
    {
        CloseAllMenus();
        Menus[MenuSelected].SetActive(true);  
    }

    public void CloseAllMenus()
    {
        foreach (GameObject menu in Menus) {
            menu.SetActive(false);
        }
    }
}
