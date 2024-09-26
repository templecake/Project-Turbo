using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Formatting;

public class PostRacePlaceUpdate : MonoBehaviour
{
    public int Place;
    RaceManager rm;

    private void Start()
    {
        rm = FindObjectOfType<RaceManager>();
    }

    public GameObject PlayerShow;
    public Text PlaceText;
    public Text UsernameText;
    public Text TimeText;
    public Image CharacterImage;
    public void Refresh()
    {
        rm = FindObjectOfType<RaceManager>();
        PlayerCar thisCar = null;
        for (int i = 0; i < rm.KartsInLobby.Count; i++)
        {
            if (rm.KartsInLobby[i].Place == Place)
            {
                thisCar = rm.KartsInLobby[i];
                break;
            }
        }

        PlaceText.text = Place.ToString();
        if (thisCar == null)
        {
            PlayerShow.SetActive(false);
            UsernameText.text = "N/A";
            TimeText.text = "--:--:---";
            CharacterImage.gameObject.SetActive(false);
            return;                                                   
        }

        CharacterImage.gameObject.SetActive(true);
        PlayerShow.SetActive(thisCar.isPlayerCar);
        UsernameText.text = thisCar.PlayerName;
        TimeText.text = thisCar.Finished ? Format.FormatTime(thisCar.GetTotalTime()) : "Still Racing";
        CharacterImage.sprite = thisCar.DisplayImage;
    }
}
