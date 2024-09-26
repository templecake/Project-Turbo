using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        UpdateUI();
    }

    private void Update()
    {
    }

    public void SceneButtonPressed(string sceneName)
    {
      GameObject.Find("GameManager").GetComponent<GameManager>().LoadScene(sceneName);
    }


    public GameObject ArcadeMenu;
    public GameObject Arcade_MainPanel;
    public GameObject Arcade_SoloRaceMenuPanel;

    string MenuOpen = "Arcade_MainPanel";

    [SerializeField] GameObject SoloRace_MapName;
    [SerializeField] GameObject SoloRace_RaceType;
    [SerializeField] GameObject SoloRace_AIAmount;
    [SerializeField] GameObject SoloRace_KartType;
    [SerializeField] GameObject SoloRace_MapDisplayName;
    [SerializeField] GameObject SoloRace_MapDisplayImage;

    public void OpenSoloRaceMenuPanel()
    {
        ArcadeMenu.SetActive(true);
        Arcade_MainPanel.SetActive(false);
        Arcade_SoloRaceMenuPanel.SetActive(true);

        MenuOpen = "Arcade_SoloRaceMenuPanel";
        UpdateUI();
    }

    public void OpenArcadeMenu()
    {
        ArcadeMenu.SetActive(true);
        Arcade_MainPanel.SetActive(true);
        Arcade_SoloRaceMenuPanel.SetActive(false);

        MenuOpen = "Arcade_MainPanel";
        UpdateUI();
    }

    public void ChangeSoloRaceMap(int amount)
    {
        if(amount > 0)
        {
            gm.SoloRace_MapChosen = gm.SoloRace_MapChosen == (gm.Maps.Count - 1) ? 0 : (gm.SoloRace_MapChosen + 1);
        }
        else
        {
            gm.SoloRace_MapChosen = gm.SoloRace_MapChosen == 0 ? (gm.Maps.Count - 1) : (gm.SoloRace_MapChosen - 1);
        }

        UpdateUI();
    }

    public void ChangeSoloRaceType(int amount)
    {
        gm.SoloRace_RaceType += amount;
        if (gm.SoloRace_RaceType > 1 ) gm.SoloRace_RaceType = 0;
        if (gm.SoloRace_RaceType < 0) gm.SoloRace_RaceType = 1;

        UpdateUI();
    }

    public void ChangeSoloRaceAIAmount(int amount)
    {
        gm.SoloRace_AIAmount += amount;
        if (gm.SoloRace_AIAmount > 7) gm.SoloRace_AIAmount = 0;
        if (gm.SoloRace_AIAmount < 0) gm.SoloRace_AIAmount = 7;

        UpdateUI();
    }

    public void ChangeSoloRaceKartType(int amount)
    {
        gm.SoloRace_KartType += amount;
        if (gm.SoloRace_KartType > 1) gm.SoloRace_KartType = 0;
        if (gm.SoloRace_KartType < 0) gm.SoloRace_KartType = 1;

        UpdateUI();
    }

    public void ChangeSoloRaceDifficulty(int amount)
    {
        gm.SoloRace_RaceDifficulty += amount;
        if (gm.SoloRace_RaceDifficulty > 3) gm.SoloRace_RaceDifficulty = 0;
        if (gm.SoloRace_RaceDifficulty < 0) gm.SoloRace_RaceDifficulty = 1;

        UpdateUI();
    }

    void UpdateUI()
    {
        gm = FindObjectOfType<GameManager>();

        ArcadeMenu.SetActive(false);
        Arcade_MainPanel.SetActive(false);
        Arcade_SoloRaceMenuPanel.SetActive(false);

        switch (MenuOpen)
        {
            case "Arcade_MainPanel":
                ArcadeMenu.SetActive(true);
                Arcade_MainPanel.SetActive(true);
                break;

            case "Arcade_SoloRaceMenuPanel":
                ArcadeMenu.SetActive(true);
                Arcade_SoloRaceMenuPanel.SetActive(true);
                break;
        }

        SoloRace_MapName.GetComponent<Text>().text = gm.Maps[gm.SoloRace_MapChosen].GetComponent<MapStorage>().MapName;
        SoloRace_RaceType.GetComponent<Text>().text = gm.SoloRace_RaceType == 0 ? "Standard" : "Time Trial";
        SoloRace_AIAmount.GetComponent<Text>().text = gm.SoloRace_AIAmount == 0 ? "OFF" : gm.SoloRace_AIAmount.ToString();
        SoloRace_KartType.GetComponent<Text>().text = gm.SoloRace_KartType == 0 ? "Custom" : "Standardized";

        SoloRace_MapDisplayName.GetComponent<Text>().text = gm.Maps[gm.SoloRace_MapChosen].GetComponent<MapStorage>().MapName;
        SoloRace_MapDisplayImage.GetComponent<Image>().sprite = gm.Maps[gm.SoloRace_MapChosen].GetComponent<MapStorage>().MapDisplayImage;

    }

    public void StartSoloRace()
    {
        gm.StartedSoloRaceVariables = true;
        StartCoroutine(gm.StartNormalRace());
    }
}
