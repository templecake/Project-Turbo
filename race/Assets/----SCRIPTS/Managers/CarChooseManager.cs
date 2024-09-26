using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class CarChooseManager : MonoBehaviour
{
    public int CurrentCarDisplaying;
    Player player;

    private void Start()
    {
        Debug.Log("Started");

        player = FindObjectOfType<Player>();

        CurrentCarDisplaying = player.KartSelected;

        if (player.playersKart.Count == 0) { CurrentCarDisplaying = -1; }

        KartNameText.GetComponent<Text>().text = " ";

        UpdateDisplayUI();
        UpdateDisplayKart();
    }

    private void Update()
    {
        CameraControl();
    }

    public void RightButton()
    {
        Debug.Log("Right button pressed");

        if (player.playersKart.Count == 0) { CurrentCarDisplaying = -1; }

        if (CurrentCarDisplaying == player.playersKart.Count - 1) { CurrentCarDisplaying = 0; } else { CurrentCarDisplaying += 1; }

        UpdateDisplayKart();
        UpdateDisplayUI();
    }

    public void LeftButton()
    {
        Debug.Log("Left button pressed");

        if (player.playersKart.Count == 0) { CurrentCarDisplaying = -1; }

        if (CurrentCarDisplaying == 0) { CurrentCarDisplaying = player.playersKart.Count - 1; } else { CurrentCarDisplaying -= 1; }

        UpdateDisplayKart();
        UpdateDisplayUI();
    }

    [SerializeField] KartDisplayer KD;


    public void SelectKart()
    {
        if (CurrentCarDisplaying == -1 || player.playersKart.Count == 0) return;

        player.KartSelected = CurrentCarDisplaying;
        UpdateDisplayUI();
    }

    void UpdateDisplayKart()
    {
        KD.PlayerKart = true;
        KD.PlayerKartID = CurrentCarDisplaying;
        KD.CreateCarBlocks();

        Debug.Log("Updated Display Kart");
    }

    [SerializeField] Text KartNameText;
    [SerializeField] GameObject SelectButton;

    [SerializeField] GameObject EditButton;

    [SerializeField] PentagonStatDisplay pentStats;

    void UpdateDisplayUI()
    {
        Debug.Log("Updated Display UI");

        KartNameText.GetComponent<Text>().text = "No Presets Created";

        if (CurrentCarDisplaying == -1) { return; }

        if (player.playersKart.Count == 0) return;

        KartNameText.GetComponent<Text>().text = player.playersKart[CurrentCarDisplaying].KartName;

        bool DisplayingKartIsSelected = player.KartSelected == CurrentCarDisplaying;

        string SelectButtonText = player.KartSelected == CurrentCarDisplaying ? "SELECTED" : "SELECT";

        SelectButton.SetActive(true);
        SelectButton.GetComponentInChildren<Text>().text = SelectButtonText;
        SelectButton.GetComponent<Button>().interactable = !DisplayingKartIsSelected;

        EditButton.SetActive(true);

        BlockStorage bs = FindObjectOfType<BlockStorage>();
        player.playersKart[CurrentCarDisplaying].ReturnVisualStats(bs, out pentStats.Weight, out pentStats.Speed, out pentStats.Acceleration, out pentStats.Boost, out pentStats.Steering);
        
        //SelectButton.GetComponent<Image>().color = DisplayingKartIsSelected ? new Color(0f, 0f, 0f, 0.4f) : new Color(0.3f, 0.3f, 0.3f, 0.4f);
    }

    public void StartEditingKart()
    {
        if (CurrentCarDisplaying == -1) { return; }

        if (player.playersKart.Count == 0) return;

        FindObjectOfType<GameManager>().StartedEditingKart = true;
        FindObjectOfType<GameManager>().EditingKartID = CurrentCarDisplaying;
        FindObjectOfType<GameManager>().LoadScene("GARAGE");
    }

    public void CreatePreset()
    {
        FindObjectOfType<GameManager>().StartedEditingKart = false;
        FindObjectOfType<GameManager>().EditingKartID = -1;

        FindObjectOfType<GameManager>().LoadScene("GARAGE");
    }

    public void ReturnToMenu()
    {
        FindObjectOfType<GameManager>().LoadScene("MAINMENU");
    }

    public GameObject CameraSpin;

    Vector3 mp1;
    Vector3 mp2;

    [SerializeField] float camSpinSpeed;
    [SerializeField] float decaySpeed;
    Vector3 lastRot;

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
            lastRot = rot;
            CameraSpin.transform.localRotation = Quaternion.Euler(rot);
        }
        else
        {
            if(lastRot.y > 0)
            lastRot = Vector3.Lerp(lastRot, Vector3.zero, decaySpeed * Time.deltaTime);

            if (lastRot.y < 0)
                lastRot = Vector3.Lerp(lastRot, Vector3.zero, -decaySpeed * Time.deltaTime);

            if (lastRot.magnitude < 0.1f) lastRot = Vector3.zero;

            CameraSpin.transform.localRotation = Quaternion.Euler(lastRot);
        }

        if (Input.GetMouseButtonUp(1))
        {
            mp1 = new Vector3(0, 0, 0);
        }
    }









    #region NewDesign

    public int PlayerKartDisplaying;
    public List<RenderTexture> KartImages;
    public RenderTexture EmptyKartTexture;

    public void AddNewPreset()
    {

    }

    public void NextPreset()
    {
        if(PlayerKartDisplaying == (player.playersKart.Count - 1))
        {
            AddNewPreset();
        }
        else
        {

        }
    }

    public void PreviousPreset()
    {
        if (PlayerKartDisplaying == 0) return;
        PlayerKartDisplaying--;
    }

    public void RefreshVisuals()
    {

    }





    #endregion

}
