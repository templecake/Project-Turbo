using Formatting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealWorldMainMenu : MonoBehaviour
{
    public int MenuOpen = 0; //0 - none, 1 - race screen
    [SerializeField] GameObject[] Menus;

    KartController3 playerKart;

    GameManager gm;
    Player p;
    CustomisationManager cM;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        qM = FindObjectOfType<QuestManager>();
        p = FindObjectOfType<Player>();
        cM = FindObjectOfType<CustomisationManager>();
        playerKart = player.GetComponent<KartController3>();
        playerKart.StartUpPublic();
        ChangeMenuOpen(0);
        CrateManager cm = FindObjectOfType<CrateManager>();
        cm.OpenAllCrates();
    }

    void Update()
    {
        if (MenuOpen != 0) if (Input.GetKeyDown(KeyCode.Escape)) ChangeMenuOpen(0);
        if (!playerKart.KartBuilt) playerKart.StartUpPublic();
    }

    private void FixedUpdate()
    {
        ShowEnterName();
        UpdatePlayerStats();
        PlayerBlipUpdate();
        if (MenuOpen == 0) CameraFollowPlayer();
    }

    public void ChangeMenuOpen(int menu)
    {
        CloseAllMenus();
        Menus[menu].SetActive(true);
        MenuOpen = menu;
        
        if(menu == 0)
        {
            playerKart.gameObject.SetActive(true);
            DeleteKartsInRace();
            HideCustomisationPlayerAndKart();
        }

        if(menu == 1)
        {
            OpenRaceMenu(0);
            UpdateRaceDisplay();
            playerKart.gameObject.SetActive(false);
            SpawnKartsInRace();
        }

        if (menu == 2)
        {
            RefreshQuestDisplayBoard();
        }

        if(menu == 3)
        {
            playerKart.gameObject.SetActive(false);
            OpenCustomisationMenu();
        }
    }

    void CloseAllMenus()
    {
        for (int i = 0; i < Menus.Length; i++)
        {
            Menus[i].SetActive(false);
        }
    }

    #region Race Menu

    public GameObject SoloRace_MapName;
    public GameObject SoloRace_MapDisplayImage;
    public GameObject SoloRace_AIAmount;
    public GameObject SoloRace_KartType;
    public GameObject SoloRace_Difficulty;

    public Transform RaceSelectCamera;

    public GameObject[] RaceMenus;
    int RaceMenuOpen = 0;

    public void OpenRaceMenu(int RaceMenu)
    {
        CloseAllRaceMenus();
        RaceMenus[RaceMenu].SetActive(true);
        RaceMenuOpen = RaceMenu;

        UpdateRaceInformation();
    }

    void CloseAllRaceMenus()
    {
        foreach (GameObject menu in RaceMenus)
        {
            menu.SetActive(false);
        }
    }

    void UpdateRaceInformation()
    {
        if(RaceMenuOpen == 2)
        {
            gm.SoloRace_RaceType = 0;
            UpdateSoloRaceInformation();
        }
    }

    void UpdateSoloRaceInformation()
    {
        SoloRace_MapName.GetComponent<Text>().text = gm.Maps[gm.SoloRace_MapChosen].GetComponent<MapStorage>().MapName;
        SoloRace_AIAmount.GetComponent<Text>().text = "AI Amount: " + (gm.SoloRace_AIAmount == 0 ? "OFF" : gm.SoloRace_AIAmount.ToString());
        SoloRace_KartType.GetComponent<Text>().text = "Kart Type: " + (gm.SoloRace_KartType == 0 ? "Custom" : "Standardized");
        SoloRace_Difficulty.GetComponent<Text>().text = "Difficulty: " + (gm.SoloRace_RaceDifficulty == 0 ? "Easy" : gm.SoloRace_RaceDifficulty == 1 ? "Normal" : gm.SoloRace_RaceDifficulty == 2 ? "Hard" : "Expert");
        SoloRace_MapDisplayImage.GetComponent<Image>().sprite = gm.Maps[gm.SoloRace_MapChosen].GetComponent<MapStorage>().MapDisplayImage;
    }

    void UpdateRaceDisplay()
    {
        Camera.main.transform.SetParent(RaceSelectCamera);
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localEulerAngles = Vector3.zero;
        UpdateSoloRaceInformation();
    }

    public void ChangeSoloRaceMap(int amount)
    {
        if (amount > 0)
        {
            gm.SoloRace_MapChosen = gm.SoloRace_MapChosen == (gm.Maps.Count - 1) ? 0 : (gm.SoloRace_MapChosen + 1);
        }
        else
        {
            gm.SoloRace_MapChosen = gm.SoloRace_MapChosen == 0 ? (gm.Maps.Count - 1) : (gm.SoloRace_MapChosen - 1);
        }

        UpdateRaceInformation();
    }

    public void ChangeSoloRaceAIAmount(int amount)
    {
        gm.SoloRace_AIAmount += amount;
        if (gm.SoloRace_AIAmount > 7) gm.SoloRace_AIAmount = 0;
        if (gm.SoloRace_AIAmount < 0) gm.SoloRace_AIAmount = 7;

        UpdateRaceInformation();
    }

    public void ChangeSoloRaceKartType(int amount)
    {
        gm.SoloRace_KartType += amount;
        if (gm.SoloRace_KartType > 1) gm.SoloRace_KartType = 0;
        if (gm.SoloRace_KartType < 0) gm.SoloRace_KartType = 1;

        UpdateRaceInformation();
    }

    public void ChangeSoloRaceDifficulty(int amount)
    {
        gm.SoloRace_RaceDifficulty += amount;
        if (gm.SoloRace_RaceDifficulty > 3) gm.SoloRace_RaceDifficulty = 0;
        if (gm.SoloRace_RaceDifficulty < 0) gm.SoloRace_RaceDifficulty = 3;

        UpdateRaceInformation();
    }

    public void StartSoloRace()
    {
        gm.StartedSoloRaceVariables = true;
        StartCoroutine(gm.StartNormalRace());
    }


    public GameObject[] StartPositions;
    public GameObject KartDisplayer;
    public List<GameObject> SpawnedKartsInRace;
    public int RaceKartDisplaySlotOn = 0;


    void DeleteKartsInRace()
    {
        for (int i = 0; i < SpawnedKartsInRace.Count; i++)
        {
            Destroy(SpawnedKartsInRace[i]);
        }
        SpawnedKartsInRace.Clear();
        RaceKartDisplaySlotOn = 0;
    }

    void SpawnKartsInRace()
    {
        GameObject NewKart = Instantiate(KartDisplayer);
        NewKart.transform.SetParent(StartPositions[RaceKartDisplaySlotOn].transform);
        NewKart.transform.localPosition = Vector3.zero;
        NewKart.transform.localScale = Vector3.one;
        NewKart.transform.localRotation = Quaternion.identity;

        NewKart.GetComponent<KartDisplayer>().PlayerKart = true;
        NewKart.GetComponent<KartDisplayer>().DisplayCharacter = true;
        NewKart.GetComponent<KartDisplayer>().RemoveColliders = true;
        NewKart.GetComponent<KartDisplayer>().CreateCarBlocks();
        SpawnedKartsInRace.Add(NewKart);
    }

    #endregion

    #region Camera
    [Header("Camera------------------------------------------------------------------------------")]
    [SerializeField] Vector3 CameraOffset;
    [SerializeField] float CameraFollowSpeed;
    [SerializeField] float CameraDelayDistance = 1f;
    public void CameraFollowPlayer()
    {
        Vector3 positionTo = player.transform.position + CameraOffset;
        Transform camera = Camera.main.transform;
        float distance = Vector3.Distance(camera.position, positionTo);
        float distScale = Mathf.Clamp(distance / CameraDelayDistance, 0.25f, 1.5f);

        camera.transform.position = Vector3.Lerp(camera.position, positionTo, CameraFollowSpeed * Time.fixedDeltaTime * distScale);
        camera.transform.LookAt(player.transform);
    }
    #endregion

    #region Main Menu
    [Header("MAIN MENU------------------------------------------------------------------------------------")]
    [SerializeField] Transform player;
    [SerializeField] float UIScale;
    [SerializeField] float uiYoffset;

    [SerializeField] Transform playerMap;

    public Text CubitText;
    public Text AdbitText;
    public Text GolbitText;
    public void UpdatePlayerStats()
    {
        CubitText.text = Format.FormatInt(p.Cubits);
        AdbitText.text = Format.FormatInt(p.AdventureCubits);
        GolbitText.text = Format.FormatInt(p.GoldenCubits);
    }

    public void PlayerBlipUpdate()
    {
        Vector3 posW = player.transform.position;
        Vector2 posUI = new Vector2(posW.x, posW.z);
        posUI *= UIScale;
        posUI.y -= uiYoffset;

        playerMap.transform.localPosition = -posUI;
    }

    public float EnterPreviewOpenTime = 0.1f;
    public List<string> EnterNames = new List<string>();
    public Transform EnterPreview;
    float ShowEnterTimeOpen = 1;
    float ShowEnterTimeShut = 1;
    public void ShowEnterName()
    {
        bool active = EnterNames.Count > 0;
        if (active)
        {
            EnterPreview.Find("NameText").GetComponent<Text>().text = EnterNames[0];
            ShowEnterTimeOpen += Time.fixedDeltaTime;
            ShowEnterTimeShut = 0;
        }
        else
        {
            ShowEnterTimeShut += Time.fixedDeltaTime;
            ShowEnterTimeOpen = 0;
        }

        float EnterPreviewOpenSpeed = 1 / EnterPreviewOpenTime;
        float scale = active ? (ShowEnterTimeOpen * EnterPreviewOpenSpeed) : (1 - ShowEnterTimeShut * EnterPreviewOpenSpeed);
        scale = Mathf.Clamp(scale, 0, 1);
        EnterPreview.localScale = Vector3.one * scale;
    }
    #endregion

    #region Quest Board

    [Header("Quest Board")]
    public int QuestCurrentlyDisplaying;
    QuestManager qM;

    [SerializeField] GameObject QuestSlot;
    [SerializeField] Transform QuestSlotHolder;

    List<GameObject> questSlots = new List<GameObject>();
    public void RefreshQuestDisplayBoard()
    {
        for (int i = 0; i < questSlots.Count; i++)
        {
            Destroy(questSlots[i]);
        }  
        questSlots.Clear();

        for (int i = 0; i < qM.QuestIDs.Count; i++)
        {
            GameObject newSlot = Instantiate(QuestSlot, QuestSlotHolder);
            newSlot.transform.localPosition = Vector3.zero;
            newSlot.transform.localScale = Vector3.one;
            newSlot.GetComponent<QuestSlot>().QuestSlotID = i;
            newSlot.GetComponent<QuestSlot>().RefreshSlot();
            questSlots.Add(newSlot);
        }
        RefreshQuestInformation();
    }

    [SerializeField] Image QuestDisplayingDisplayImage;
    [SerializeField] Text QuestDisplayingDisplayName;
    [SerializeField] Text QuestDisplayingDescription;
    public void RefreshQuestInformation()
    {
        QuestDisplayingDisplayImage.sprite = qM.QuestDisplayImage(QuestCurrentlyDisplaying);
        QuestDisplayingDisplayName.text = qM.Quests[qM.QuestIDs[QuestCurrentlyDisplaying]].QuestName;
        QuestDisplayingDescription.text = qM.QuestStatus(QuestCurrentlyDisplaying);
    }

    #endregion

    #region Customisation
    [Header("CUSTOMISATION-----------------------------------------------------------------")]

    public GameObject CustomisationCameraPosition;

    void OpenCustomisationMenu()
    {
        Camera.main.transform.SetParent(CustomisationCameraPosition.transform);
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localEulerAngles = Vector3.zero;

        ToggleCustomisationPanel("None");
        ShowCustomisationPlayerAndKart();            
    }


    public GameObject CustomisationDisplayKart;
    public GameObject CustomisationDisplayCharacter;
    

    void ShowCustomisationPlayerAndKart() 
    {
        CustomisationDisplayKart.SetActive(true);
        CustomisationDisplayKart.GetComponent<KartDisplayer>().CreateCarBlocks();

        CustomisationDisplayCharacter.SetActive(true);
        CustomisationDisplayCharacter.GetComponent<DisplayCharacter>().RefreshCharacter();
    }

    void HideCustomisationPlayerAndKart()
    {
        CustomisationDisplayKart.SetActive(false);
        CustomisationDisplayCharacter.SetActive(false);
    }

    public string CustomisationPanelOpenType;

    public void ToggleCustomisationPanel(string panel)
    {
        if(CustomisationPanelOpenType == panel)
        {
            CustomisationPanelOpenType = "None";
        }
        else
        {
            CustomisationPanelOpenType = panel;
        }
        RefreshCustomisationPanel();
    }

    public void SelectCustomisation(int customisation)
    {
        if(CustomisationPanelOpenType == "Heads")
        {
            cM.HeadSelected = customisation; 
        }
        else if(CustomisationPanelOpenType == "Bodies")
        {
            cM.BodySelected = customisation; 
        }
        else if (CustomisationPanelOpenType == "Legs")
        {
            cM.LegsSelected = customisation;
        }
        else if (CustomisationPanelOpenType == "AnimationStyle")
        {
            cM.AnimationStyleSelected = customisation;
        }
        CustomisationPanelOpenType = "None";
        CustomisationDisplayCharacter.GetComponent<DisplayCharacter>().RefreshCharacter();
        CustomisationDisplayCharacter.GetComponent<DisplayCharacter>().AnimationPlaying = 0;
        RefreshCustomisationPanel();
    }

    public Transform CustomisationSlotParent;
    public GameObject CustomisationPanelSlot;
    public GameObject CustomisationsPanel;
    List<GameObject> CustomisationSlots = new List<GameObject>();
    List<int> CustomisationSlotIDs = new List<int>();
    public void RefreshCustomisationPanel()
    {
        //delete previous slots
        for (int i = 0; i < CustomisationSlots.Count; i++)
        {
            Destroy(CustomisationSlots[i]);
        }
        CustomisationSlots.Clear();
        CustomisationSlotIDs.Clear();

        //open or hide panel depending on what
        CustomisationsPanel.SetActive(CustomisationPanelOpenType != "None");

        //get new slots
        if (CustomisationPanelOpenType == "None") return;


        List<Customisation> customisationsToDisplay = new List<Customisation>();
        if (CustomisationPanelOpenType == "Heads")
        {
            for (int i = 0; i < cM.HeadOwned.Length; i++)
            {
                if (cM.HeadOwned[i])
                {
                    customisationsToDisplay.Add(cM.Heads[i]);
                    CustomisationSlotIDs.Add(i);
                }
            }
        }
        else if(CustomisationPanelOpenType == "Bodies")
        {
            for (int i = 0; i < cM.BodyOwned.Length; i++)
            {
                if (cM.BodyOwned[i])
                {
                    customisationsToDisplay.Add(cM.Bodies[i]);
                    CustomisationSlotIDs.Add(i);
                }
            }
        }
        else if (CustomisationPanelOpenType == "Legs")
        {
            for (int i = 0; i < cM.LegsOwned.Length; i++)
            {
                if (cM.LegsOwned[i])
                {
                    customisationsToDisplay.Add(cM.Legs[i]);
                    CustomisationSlotIDs.Add(i);
                }
            }
        }
        else if (CustomisationPanelOpenType == "AnimationStyle")
        {
            for (int i = 0; i < cM.AnimationStyleOwned.Length; i++)
            {
                if (cM.AnimationStyleOwned[i])
                {
                    customisationsToDisplay.Add(cM.AnimationStyles[i]);
                    CustomisationSlotIDs.Add(i);
                }
            }
        }

        //create slots
        for (int i = 0; i < customisationsToDisplay.Count; i++)
        {
            GameObject newSlot = Instantiate(CustomisationPanelSlot, CustomisationSlotParent);
            newSlot.GetComponent<CustomisationSlot>().CustomisationID = CustomisationSlotIDs[i];
            newSlot.GetComponent<CustomisationSlot>().GeneralCustomisationID = customisationsToDisplay[i].GeneralID;
            newSlot.GetComponent<CustomisationSlot>().RefreshSlot();
            CustomisationSlots.Add(newSlot);
        }
    }

    #endregion
}
