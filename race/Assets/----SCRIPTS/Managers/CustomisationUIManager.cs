using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomisationUIManager : MonoBehaviour
{
    public GameManager gM;
    public CustomisationManager cM;

    public Animator MenuAnimator;

    float timeToRandomAnimation;

    private void Start()
    {
        gM = FindObjectOfType<GameManager>();
        cM = FindObjectOfType<CustomisationManager>();  

        RefreshDisplayCharacter();

        timeToRandomAnimation = 10f;
    }

    private void Update()
    {
        CameraControl();

        RandomPlayerAnimation();
    }

    void RandomPlayerAnimation()
    {
        timeToRandomAnimation -= Time.deltaTime;
        if (timeToRandomAnimation < 0)
        {
            timeToRandomAnimation = Random.Range(30f, 90f);
            DisplayCharacter.GetComponent<DisplayCharacterScript>().PlayAnimation("STAND_IDLE_1");
        }
    }

    public void ReturnToMenu()
    {
        FindObjectOfType<GameManager>().LoadScene("MAINMENU");
    }

    public GameObject CameraSpin;

    Vector3 mp1;
    Vector3 mp2;

    [SerializeField] float camSpinSpeed;

    void CameraControl()
    {
        if (Input.GetMouseButton(1))
        {
            if (mp1 == new Vector3(0, 0, 0))
            {
                mp1 = Input.mousePosition;
            }
            mp2 = Input.mousePosition;
            float xdifference = mp1.x - mp2.x;
            float ydifference = mp1.y - mp2.y;
            CameraSpin.transform.Rotate(new Vector3(ydifference * Time.deltaTime * camSpinSpeed, -xdifference * Time.deltaTime * camSpinSpeed, 0));
            mp1 = mp2;
            Vector3 rot = new Vector3(0, CameraSpin.transform.localEulerAngles.y, 0);
            CameraSpin.transform.rotation = Quaternion.Euler(rot);
        }

        if (Input.GetMouseButtonUp(1))
        {
            mp1 = new Vector3(0, 0, 0);
        }
    }

    public GameObject DisplayCharacter;

    public void RefreshDisplayCharacter()
    {
        DisplayCharacter.GetComponent<DisplayCharacterScript>().HeadChosen = cM.HeadSelected;
        DisplayCharacter.GetComponent<DisplayCharacterScript>().BodyChosen = cM.BodySelected;
        DisplayCharacter.GetComponent<DisplayCharacterScript>().LegsChosen = cM.LegsSelected;
        DisplayCharacter.GetComponent<DisplayCharacterScript>().AnimationStyleChosen = cM.AnimationStyleSelected;

        DisplayCharacter.GetComponent<DisplayCharacterScript>().RefreshCharacter();
        DisplayCharacter.GetComponent<DisplayCharacterScript>().PlayAnimation("STAND_IDLE");
    }

    public void CustomiseButtonSelected(string buttonType)
    {
        switch (buttonType)
        {
            case "body":
                ShowBodyInventory(); break;

            case "legs":
                ShowLegsInventory(); break;

            case "head":
                ShowHeadInventory(); break;
        }

        MenuAnimator.SetBool("InventoryOpen", true);
    }


    #region Show Inventories
    List<GameObject> createdButtons = new List<GameObject>();
    void clearCreatedButtons()
    {
        for (int i = 0; i < createdButtons.Count; i++)
        {
            Destroy(createdButtons[i]);
        }
        createdButtons.Clear();
    }

    

    [SerializeField] GameObject InventoryPanel;

    [SerializeField] GameObject InventoryButtonPrefab;

   
    void ShowBodyInventory()
    {
        clearCreatedButtons();

        List<int> buttonsShow = new List<int>();
        for (int i = 0; i < cM.BodyOwned.Length; i++)
        {
            if (cM.BodyOwned[i]) buttonsShow.Add(i);
        }

        for (int i = 0; i < buttonsShow.Count; i++)
        {
            Customisation custom = cM.Bodies[buttonsShow[i]];

            GameObject newButton = Instantiate(InventoryButtonPrefab);
            newButton.transform.SetParent(InventoryPanel.transform);
            newButton.transform.localScale = Vector3.one;

            Color thisRarityColour = cM.GetColorFromRarity(custom.Rarity);

            newButton.transform.Find("itemImage").GetComponent<Image>().sprite = custom.DisplayImage;
            newButton.transform.Find("Tag").GetComponent<Image>().color = thisRarityColour;
            newButton.transform.Find("fade").GetComponent<Image>().color = thisRarityColour;

            newButton.transform.Find("nameTextLabel").transform.Find("nameText").GetComponent<Text>().text = custom.DisplayName;
            newButton.transform.Find("nameTextLabel").transform.Find("nameText").GetComponent<Text>().color = thisRarityColour;

            newButton.GetComponent<InventoryItemSlot>().ID = buttonsShow[i];
            newButton.GetComponent<InventoryItemSlot>().ButtonType = "body";
            createdButtons.Add(newButton);
        }
    }

    void ShowLegsInventory()
    {
        clearCreatedButtons();

        List<int> buttonsShow = new List<int>();
        for (int i = 0; i < cM.LegsOwned.Length; i++)
        {
            if (cM.LegsOwned[i]) buttonsShow.Add(i);
        }

        for (int i = 0; i < buttonsShow.Count; i++)
        {
            Customisation custom = cM.Legs[buttonsShow[i]];

            GameObject newButton = Instantiate(InventoryButtonPrefab);
            newButton.transform.SetParent(InventoryPanel.transform);
            newButton.transform.localScale = Vector3.one;

            Color thisRarityColour = cM.GetColorFromRarity(custom.Rarity);

            newButton.transform.Find("itemImage").GetComponent<Image>().sprite = custom.DisplayImage;
            newButton.transform.Find("Tag").GetComponent<Image>().color = thisRarityColour;
            newButton.transform.Find("fade").GetComponent<Image>().color = thisRarityColour;

            newButton.transform.Find("nameTextLabel").transform.Find("nameText").GetComponent<Text>().text = custom.DisplayName;
            newButton.transform.Find("nameTextLabel").transform.Find("nameText").GetComponent<Text>().color = thisRarityColour;

            newButton.GetComponent<InventoryItemSlot>().ID = buttonsShow[i];
            newButton.GetComponent<InventoryItemSlot>().ButtonType = "legs";
            createdButtons.Add(newButton);
        }
    }

    void ShowHeadInventory()
    {
        clearCreatedButtons();

        List<int> buttonsShow = new List<int>();
        for (int i = 0; i < cM.HeadOwned.Length; i++)
        {
            if (cM.HeadOwned[i]) buttonsShow.Add(i);
        }

        for (int i = 0; i < buttonsShow.Count; i++)
        {
            Customisation custom = cM.Heads[buttonsShow[i]];

            GameObject newButton = Instantiate(InventoryButtonPrefab);
            newButton.transform.SetParent(InventoryPanel.transform);
            newButton.transform.localScale = Vector3.one;

            Color thisRarityColour = cM.GetColorFromRarity(custom.Rarity);

            newButton.transform.Find("itemImage").GetComponent<Image>().sprite = custom.DisplayImage;
            newButton.transform.Find("Tag").GetComponent<Image>().color = thisRarityColour;
            newButton.transform.Find("fade").GetComponent<Image>().color = thisRarityColour;

            newButton.transform.Find("nameTextLabel").transform.Find("nameText").GetComponent<Text>().text = custom.DisplayName;
            newButton.transform.Find("nameTextLabel").transform.Find("nameText").GetComponent<Text>().color = thisRarityColour;

            newButton.GetComponent<InventoryItemSlot>().ID = buttonsShow[i];
            newButton.GetComponent<InventoryItemSlot>().ButtonType = "head";
            createdButtons.Add(newButton);
        }
    }

    #endregion

    #region Equip Item
    public void EquipItem(string type, int id)
    {
        switch (type)
        {
            case "body":
                cM.SelectBody(id); break;

            case "legs":
                cM.SelectLegs(id); break;

            case "head":
                cM.SelectHead(id); break;

            case "animStyle":
                cM.SelectAnimationStyle(id); break;

        }

        MenuAnimator.SetBool("InventoryOpen", false);
        FindObjectOfType<GameManager>().PlaySoundEffect(3);
        FindObjectOfType<GameManager>().PlaySoundEffect(0);
        RefreshDisplayCharacter();
    }
    #endregion
}
