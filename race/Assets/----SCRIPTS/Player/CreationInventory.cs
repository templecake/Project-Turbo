using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreationInventory : MonoBehaviour
{
    int currentPanel;
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] GameObject ColourButtonPrefab;
    [SerializeField] GameObject colourGrid;

    [SerializeField] GameObject grid;
    [SerializeField] GameObject blocksGrid;
    [SerializeField] GameObject enginesGrid;
    [SerializeField] GameObject wheelsGrid;
    [SerializeField] GameObject boostersGrid;

    private void Start()
    {
        RefreshButtons();
    }

    public void ChangeSelected(int select)
    {
        FindObjectOfType<CreationManager>().UpdateCurrentPlacing(select);
    }

    public void ChangePanel(int panel)
    {
        currentPanel = panel;
        DisableAll();
        if (currentPanel == 0) grid.transform.parent.gameObject.SetActive(true);
        if (currentPanel == 1) blocksGrid.transform.parent.gameObject.SetActive(true);
        if (currentPanel == 2) enginesGrid.transform.parent.gameObject.SetActive(true);
        if (currentPanel == 3) wheelsGrid.transform.parent.gameObject.SetActive(true);
        if (currentPanel == 4) boostersGrid.transform.parent.gameObject.SetActive(true);
    }

    void DisableAll()
    {
        grid.transform.parent.gameObject.SetActive(false);
        blocksGrid.transform.parent.gameObject.SetActive(false);
        enginesGrid.transform.parent.gameObject.SetActive(false);
        wheelsGrid.transform.parent.gameObject.SetActive(false);
        boostersGrid.transform.parent.gameObject.SetActive(false);
    }

    public List<GameObject> ButtonsCreated = new List<GameObject>();

    public void RefreshButtons()
    {

        CreationManager cm = GameObject.Find("CreationManager").GetComponent<CreationManager>();
        BlockStorage bs = GameObject.Find("GameManager").GetComponent<BlockStorage>();

        for (int i = 0; i < ButtonsCreated.Count; i++)
        {
            Destroy(ButtonsCreated[i]);
        }
        ButtonsCreated.Clear();


        for (int i = 0; i < bs.blocks.Count; i++)
        {
            if(bs.amountInInventory[i] > 0)
            {
                GameObject button = Instantiate(buttonPrefab);
                button.transform.SetParent(grid.transform);
                button.transform.localScale = Vector3.one;
                button.GetComponent<InventoryButton>().id = i;
                button.GetComponent<InventoryButton>().Refresh();
                ButtonsCreated.Add(button);
            }
        }

        for (int i = 0; i < bs.materials.Count; i++)
        {
            if (bs.materialUnlocked[i])
            {
                GameObject button = Instantiate(ColourButtonPrefab);
                button.transform.SetParent(colourGrid.transform);
                button.transform.localScale = Vector3.one;
                button.GetComponent<ColourButton>().id = i;
                button.GetComponent<ColourButton>().Refresh();
                ButtonsCreated.Add(button);
            }
        }
    }
}
