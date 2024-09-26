using Formatting;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    GameManager gm;
    CustomisationManager cm;
    public bool RaceStarted = false;
    public bool AdvancedToPostRace = false;

    public int RaceLoadingStage = 0; //loading map, loading karts, camera to kart, 3, 2, 1, go, race playing 
    public bool MapLoaded = false;

    public float[] CubitPositionMultiplier;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        cm = FindObjectOfType<CustomisationManager>();
        StartCoroutine(StartRace());
    }

    private void Update()
    {
        CoroutineFix();
        if (RaceLoadingStage < 2) return;
        UpdateUI();
        if (!RaceStarted) return;
        UpdatePlayerLapTimes();
        UpdateCurrentKartPositions();
        UpdateProgress();
        CalculatePositions();
    }

    int f;

    int RandomCheckNumber = 0;
    int CheckNumberOneSecondAgo = 0;
    float CoroCheckTime;
    void CoroutineFix()   //keep checking random number is not similar, 1 in 2.1 billion chance of a small break.
    {
        CoroCheckTime += Time.deltaTime;
        if (CoroCheckTime >= 1f)
        {
            CoroCheckTime = 0;
            if (RandomCheckNumber == CheckNumberOneSecondAgo)
            {
                if (RaceLoadingStage == 0) gm.ReturnToMainMenu();
                if (RaceLoadingStage == 1) gm.ReturnToMainMenu(); //error has occured so return to main menu
                if (RaceLoadingStage == 7) StartCoroutine(WaitForKartsToFinish());
                if (RaceLoadingStage == 8) StartCoroutine(PostRaceVisuals());
                Debug.Log("Resumed Coroutine");
            }
            CheckNumberOneSecondAgo = RandomCheckNumber;
        }
    }

    [Header("Race Information------------------------------------------------------------------------------------")]
    public int RaceType;
    public int RaceDifficulty;
    public int AI_amount;
    public int KartType;

    public int Laps;
    public List<AIRoute> aiRoutes = new List<AIRoute>();
    public List<GameObject> AbilitiesInRace = new List<GameObject>();
    

    IEnumerator StartRace()
    {
        FindObjectOfType<QuestManager>().GetPreRaceQuestInformation();
        PlayerCubitsBefore = FindObjectOfType<Player>().Cubits;
        //load map
        RaceLoadingStage = 0; //loading map
        LoadMap();
        while (!MapLoaded)
        {
            RandomCheckNumber = Random.Range(0, 2147483647);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        RaceLoadingStage = 1; //loading karts

        RaceType = gm.SoloRace_RaceType;
        RaceDifficulty = gm.SoloRace_RaceDifficulty;
        AI_amount = gm.SoloRace_AIAmount;
        KartType = gm.SoloRace_KartType;
        StartCoroutine(MoveBlackBarsIn(0.01f));
        StartCoroutine(LoadKarts());
        while (!KartsLoaded)
        {
            RandomCheckNumber = Random.Range(0, 2147483647);
            yield return new WaitForSeconds(Time.deltaTime);
            //while karts arent loaded, play intro animation
        }
        //wait a few seconds after loaded
        yield return new WaitForSeconds(1f);

        RaceLoadingStage = 2; //move camera to karts
        //3 2 1 go
        yield return new WaitForSeconds(2f);
        RaceLoadingStage = 3; //3
        Debug.Log("3!");
        yield return new WaitForSeconds(1f);
        RaceLoadingStage = 4; //2
        Debug.Log("2!");
        StartNumber_Two.SetActive(true);
        StartNumber_Two.GetComponent<Animator>().SetTrigger("Play");
        yield return new WaitForSeconds(1f);
        RaceLoadingStage = 5; //1
        Debug.Log("1!");
        StartNumber_One.SetActive(true);
        StartNumber_One.GetComponent<Animator>().SetTrigger("Play");
        yield return new WaitForSeconds(1f);
        RaceLoadingStage = 6; //go
        Debug.Log("Go!");
        StartNumber_Go.GetComponent<Animator>().SetTrigger("Play");
        RaceStarted = true;
        StartCoroutine(MoveBlackBarsOut(0.25f));
        yield return new WaitForSeconds(.5f);
        RaceLoadingStage = 7;
        RaceStartUI();
        //race
        StartCoroutine(WaitForKartsToFinish());
        
        yield return false;
    }

    IEnumerator WaitForKartsToFinish()
    {
        float scale = 0;
        while (scale < 1)
        {
            scale += Time.deltaTime * 10;
            RaceMainCanvas.transform.localScale = Vector3.one * scale;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        scale = 1;
        RaceMainCanvas.transform.localScale = Vector3.one * scale;

        while (!KartsInLobby[0].Finished) //until player is finished
        {
            RandomCheckNumber = Random.Range(0, 2147483647);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        StartCoroutine(MoveBlackBarsIn(.25f));
        if (KartsInLobby[0].ingameKart)
        {
            float PlayerSteering = KartsInLobby[0].ingameKart.GetComponent<KartController3>().Steering;
            KartsInLobby[0].ingameKart.GetComponent<KartController3>().Player = false;
            KartsInLobby[0].ingameKart.GetComponent<KartController3>().Steering = PlayerSteering * 2;
        }
        if (KartsInLobby[0].Place == 1)
        {
            FindObjectOfType<QuestManager>().PlacedFirst();
        }
        if (KartsInLobby[0].Place <= 3)
        {
            FindObjectOfType<QuestManager>().PlacedTopThree();
        }

        RaceLoadingStage = 8;
        StartCoroutine(PostRaceVisuals());

        yield return false;
    }


    float ViewTimeUpdateMin = 5;
    float ViewTimeUpdateMax = 15;
    public GameObject[] PositionsDisplay;


    IEnumerator PostRaceVisuals()
    {
        //show finished ui
        //tv camera
        RaceFinishedPopup.GetComponent<Animator>().SetTrigger("Finish");
        float UpdateViewTime = Random.Range(ViewTimeUpdateMin, ViewTimeUpdateMax);
        float TimeSinceViewUpdated = 0;
        while (!AdvancedToPostRace)
        {
            RandomCheckNumber = Random.Range(0, 2147483647);
            TimeSinceViewUpdated += Time.deltaTime;

            if (CameraInPanView)
            {
                if (TimeInPanView > 0)
                {
                    CameraPans[CameraViewOn].GetComponent<CameraRacePointFollow>().Activated = true;
                    TimeInPanView -= Time.deltaTime;
                }
                else
                {
                    CameraPans[CameraViewOn].GetComponent<CameraRacePointFollow>().Activated = false;
                    CameraInPanView = false;
                }
            }
            else
            {
                if (TimeSinceViewUpdated > UpdateViewTime)
                {
                    UpdateViewTime = Random.Range(ViewTimeUpdateMin, ViewTimeUpdateMax);
                    TimeSinceViewUpdated = 0;
                    RandomiseCurrentCameraViewOn();
                }
            }

            for (int i = 0; i < PositionsDisplay.Length; i++)
            {
                PositionsDisplay[i].GetComponent<PostRacePlaceUpdate>().Refresh();
            }

            RaceFinishedUI();
            yield return new WaitForSeconds(Time.deltaTime);
        }
        RaceLoadingStage = 9;

        DisplayCubit = PlayerCubitsBefore;
        CubitsEarned = Random.Range(300, 501);

        StartCoroutine(RewardScreen());

        yield return false;
    }


    public Text MultiplierText;
    public Text PostRacePositionText;
    public Text CubitAmountText;
    public Text CubitsEarnedText;

    float MultiplierBefore;
    float MultiplierShow;

    float CubitsEarnedBefore;
    float CubitsEarnedShow;

    float DisplayCubit;
    [SerializeField] int CubitsEarned = 0;
    public GameObject QuestDisplay;
    public Transform QuestDisplayHolder;
    List<GameObject> questsShow = new List<GameObject>();

    IEnumerator RewardScreen()
    {
        AwardPlayer();
        PostRaceUI();

        //need to know:
        //-quests already had
        //-quests already had finished
        //-quests added and finished
        //quests added and not finished
        int SlotOn = 0;
        QuestManager qm = FindObjectOfType<QuestManager>();

        for (int i = 0; i < qm.QuestsToShow.Count; i++)
        {
            GameObject slot = Instantiate(QuestDisplay, QuestDisplayHolder);
            slot.transform.localScale = Vector3.zero;
            slot.GetComponent<PostRaceQuestSlot>().QuestID = qm.QuestsToShow[i];
            slot.GetComponent<PostRaceQuestSlot>().StartProgress = qm.QuestsProgressBeforeRace[i];
            slot.GetComponent<PostRaceQuestSlot>().QuestSlotNumber = SlotOn;
            slot.GetComponent<PostRaceQuestSlot>().QuestFinished = qm.QuestCompleted[i];
            questsShow.Add(slot);

            if (!qm.QuestCompleted[i])
            {
                SlotOn++;
            }

            bool done = false;
            while (slot && !done)
            {
                if (!slot.GetComponent<PostRaceQuestSlot>().Finished) done = true;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            yield return new WaitForSeconds(0.5f);
        }
        //show quests completed

        //show pictures


        
        //set up
        Player p = FindObjectOfType<Player>();
        CubitAmountText.text = Format.FormatInt((int)DisplayCubit);
        //show earned


        MultiplierShow = p.Multiplier;
        CubitsEarnedText.text = "+" + Format.FormatInt(CubitsEarned);
        MultiplierText.text = "Multiplier: x" + Format.RoundToDecimal(MultiplierShow, 2);
        PostRacePositionText.text = Format.FormatPosition(KartsInLobby[0].Place);

        yield return new WaitForSeconds(1f);

        //multiplier increased
        
        float PlayerPositionMultiply = CubitPositionMultiplier[KartsInLobby[0].Place - 1] * p.Multiplier;
        float CubitsTotal = PlayerCubitsBefore + (float)CubitsEarned * PlayerPositionMultiply;

        float MultiplyLerpValue = 0;
        while (MultiplierShow < PlayerPositionMultiply)
        {
            MultiplierShow = Mathf.Lerp(p.Multiplier, PlayerPositionMultiply, MultiplyLerpValue);
            MultiplierText.text = "Multiplier: x" + Format.RoundToDecimal(MultiplierShow, 2);
            MultiplyLerpValue += Time.deltaTime * 2;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        MultiplierShow = PlayerPositionMultiply;
        MultiplierText.text = "Multiplier: x" + Format.RoundToDecimal(MultiplierShow, 2);

        yield return new WaitForSeconds(1f);
        //earned increase
        CubitsEarnedShow = (float) CubitsEarned;
        float CubitsEarnedMultiplied = (float)CubitsEarned * PlayerPositionMultiply;

        float CubitEarnedLerpValue = 0;
        while (CubitsEarnedShow < CubitsEarnedMultiplied)
        {
            CubitsEarnedShow = Mathf.Lerp((float) CubitsEarned, CubitsEarnedMultiplied, CubitEarnedLerpValue);
            CubitsEarnedText.text = "+" + Format.FormatInt((int)CubitsEarnedShow);
            CubitEarnedLerpValue += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        CubitsEarnedShow = (int) CubitsEarnedMultiplied;
        CubitsEarnedText.text = "+" + Format.FormatInt((int)CubitsEarnedShow);

        yield return new WaitForSeconds(1);

        //add earned to cubits

        float DisplayCubitLerpTime = 0;
        while (DisplayCubit < p.Cubits)
        {
            DisplayCubit = Mathf.Lerp(PlayerCubitsBefore, p.Cubits, DisplayCubitLerpTime);
            CubitAmountText.text = Format.FormatInt((int)DisplayCubit);
            DisplayCubitLerpTime += Time.deltaTime / 2;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        DisplayCubit = p.Cubits;
        CubitAmountText.text = Format.FormatInt((int)DisplayCubit);

        yield return false;
    }

    public void AdvanceButtonPressed()
    {
        AdvancedToPostRace = true; 
    }

    public void ReturnButtonPressed()
    {
        gm.ReturnToMainMenu();
    }


    [Header("Player------------------------------------------------------------------------------------")]
    public GameObject PlayersCurrentKart;
    public List<PlayerCar> KartsInLobby = new List<PlayerCar>();

    int PlayerCubitsBefore;

    public void AwardPlayer()
    {
        Player p = FindObjectOfType<Player>();
        float PlayerPositionMultiply = CubitPositionMultiplier[KartsInLobby[0].Place - 1] * p.Multiplier;
        int CubitReward = (int) (CubitsEarned * PlayerPositionMultiply);
        p.AddCubits(CubitReward);

        Debug.Log("Player Position Multiplier: " + PlayerPositionMultiply);
        Debug.Log("Player's Multiplier: " + p.Multiplier);
        Debug.Log("Position Multiplier: " + CubitPositionMultiplier[KartsInLobby[0].Place - 1]);

        Debug.Log("Awarded " + CubitsEarned + " cubits with a x" + PlayerPositionMultiply + " multiplier.");
    }



    [Header("Map Information---------------------------------------------------------------------------------------")]
    public int MapSelected;
    public GameObject Map;
    public List<GameObject> StartPositions = new List<GameObject>();
    public List<GameObject> Checkpoints = new List<GameObject>();

    //load map 
    void LoadMap()
    {
        int MapSelected = gm.SoloRace_MapChosen;
        GameObject CurrentMap = gm.Maps[MapSelected];
        Map = Instantiate(CurrentMap);
        Map.transform.localScale = Vector3.one;
        Map.transform.localPosition = Vector3.zero;
        Map.name = Map.GetComponent<MapStorage>().MapName;
        CameraPans = Map.GetComponent<MapStorage>().CameraPanPositions;

        int StartPositionsAmount = Map.transform.Find("StartPositions").childCount;
        for (int i = 0; i < StartPositionsAmount; i++)
        {
            StartPositions.Add(Map.transform.Find("StartPositions").GetChild(i).gameObject);
        }

        int CheckpointsAmount = Map.transform.Find("Checkpoints").childCount;
        for (int i = 0; i < CheckpointsAmount; i++)
        {
            Checkpoints.Add(Map.transform.Find("Checkpoints").GetChild(i).gameObject);
        }

        MapLoaded = true;
    }

    #region Karts
    [Header("Kart Information--------------------------------------------------------------------------------------------")]
    public GameObject KartPrefab;

    //create karts
    int CreatingPosition = 0;
    bool KartsLoaded = false;
    IEnumerator LoadKarts()
    {
        CreatePlayer();
        CreatingPosition++;

        for (int i = 0; i < AI_amount; i++)
        {
            CreateAI();
            CreatingPosition++;
            yield return new WaitForSeconds(0.1f);
        }
        CreatePlayerBlips();
        KartsLoaded = true;

        yield return false;
    }      

    void CreatePlayer()
    {
        PlayersCurrentKart = Instantiate(KartPrefab, transform.Find("Karts"));
        PlayersCurrentKart.transform.position = StartPositions[CreatingPosition].transform.position;
        PlayersCurrentKart.GetComponent<KartController3>().Player = true;
        PlayersCurrentKart.GetComponent<KartController3>().aiID = -1;
        PlayersCurrentKart.GetComponent<KartController3>().InGamePlayersKart = PlayersCurrentKart;
        if (RaceType == 0)
        {
            PlayersCurrentKart.transform.localScale = Vector3.one;
            PlayersCurrentKart.GetComponent<KartController3>().sphereRB.transform.position = StartPositions[CreatingPosition].transform.position;
        }
        else
        {
            //spawn in race start pos
            PlayersCurrentKart.GetComponent<KartController3>().sphereRB.transform.position = StartPositions[CreatingPosition].transform.position;
            PlayersCurrentKart.transform.localScale = Vector3.one;
        }
        PlayersCurrentKart.GetComponent<KartController3>().LoadedIn = true;


        PlayerCar pc = new PlayerCar();
        pc.isPlayerCar = true;
        pc.UpdatePosition(PlayersCurrentKart.transform.Find("main").position);
        pc.UpdateRotation(PlayersCurrentKart.transform.Find("main").eulerAngles);
        pc.UpdateCheckpoint(0, this);
        pc.CurrentLap = 0;
        pc.PlayerName = FindObjectOfType<Player>().PlayerUsername;
        pc.LapTime = new float[Laps];
        pc.ingameKart = PlayersCurrentKart;
        pc.DisplayImage = cm.Heads[cm.HeadSelected].DisplayImage;
        KartsInLobby.Add(pc);

        for (int i = 0; i < CameraPans.Count; i++)
        {
            CameraPans[i].GetComponent<CameraRacePointFollow>().Subject = PlayersCurrentKart;
        }
    }
    
    void CreateAI()
    {
        GameObject AI_Kart = Instantiate(KartPrefab, transform.Find("Karts"));
        AI_Kart.transform.position = StartPositions[CreatingPosition].transform.position;
        AI_Kart.GetComponent<KartController3>().Player = false;
        int aiID = Random.Range(0, 3);
        AI_Kart.GetComponent<KartController3>().aiID = aiID; 
        AI_Kart.GetComponent<KartController3>().InGamePlayersKart = PlayersCurrentKart;
        AI_Kart.GetComponent<KartController3>().IsPlayerViewKart = false;

        if (RaceType == 0)
        {
            AI_Kart.transform.localScale = Vector3.one;
            AI_Kart.GetComponent<KartController3>().sphereRB.transform.position = StartPositions[CreatingPosition].transform.position;
        }
        else
        {
            //spawn in race start pos
            AI_Kart.GetComponent<KartController3>().sphereRB.transform.position = StartPositions[CreatingPosition].transform.position;
            AI_Kart.transform.localScale = Vector3.one;
        }
        AI_Kart.GetComponent<KartController3>().LoadedIn = true;

        PlayerCar pc = new PlayerCar();
        pc.isPlayerCar = false;
        pc.UpdatePosition(AI_Kart.transform.Find("main").position);
        pc.UpdateRotation(AI_Kart.transform.Find("main").eulerAngles);
        pc.UpdateCheckpoint(0, this);
        pc.CurrentLap = 0;
        pc.LapTime = new float[Laps];
        pc.PlayerName = FindObjectOfType<AIManager>().AICharacters[aiID].CharacterName;
        pc.ingameKart = AI_Kart;
        pc.DisplayImage = cm.Heads[FindObjectOfType<AIManager>().AICharacters[aiID].CharacterHead].DisplayImage;
        KartsInLobby.Add(pc);
    }

    IEnumerator RespawnPlayerKart()
    {
        yield return new WaitForSeconds(1);
    }

    IEnumerator RespawnAIKart()
    {
        yield return new WaitForSeconds(1);
    }

    //start race, AI routes and Ghosts
    public AIRoute GetRandomAIRoute()
    {
        Debug.Log("Requested AI Route");
        int RouteChosen = Random.Range(0, Map.GetComponent<MapStorage>().AIRoutes.Count);
        return Map.GetComponent<MapStorage>().AIRoutes[RouteChosen];
    }

    void UpdatePlayerLapTimes()
    {
        for (int i = 0; i < KartsInLobby.Count; i++)
        {
            KartsInLobby[i].UpdateLapTime(Time.deltaTime);
        }
    }

    void UpdateCurrentKartPositions()
    {
        for (int i = 0; i < KartsInLobby.Count; i++)
        {
            KartsInLobby[i].UpdatePosition(KartsInLobby[i].ingameKart.transform.Find("main").position);
            KartsInLobby[i].UpdateRotation(KartsInLobby[i].ingameKart.transform.Find("main").eulerAngles);
        }
    }

    void UpdateProgress()
    {
        for (int i = 0; i < KartsInLobby.Count; i++)
        {
            int FinishedAdder = KartsInLobby[i].Finished ? (KartsInLobby.Count - KartsInLobby[i].Place + 1) : 0;
            float prog = (KartsInLobby[i].CurrentLap + FinishedAdder) * 1000 + KartsInLobby[i].CurrentCheckpoint * 10;
            if (KartsInLobby[i].CurrentCheckpoint > 0)
            {
                int current = KartsInLobby[i].CurrentCheckpoint;
                float distance = Vector3.Distance(Checkpoints[current].transform.position, Checkpoints[current - 1].transform.position);
                float distancefromcheck = distance;
                if (KartsInLobby[i].ingameKart != null) distancefromcheck = Vector3.Distance(KartsInLobby[i].ingameKart.transform.position, Checkpoints[current].transform.position);
                prog += distancefromcheck / distance;
            }
            KartsInLobby[i].UpdateProgress(prog);
        }
    }

    float TimeToCalculatePosition = 0;
    void CalculatePositions()
    {
        TimeToCalculatePosition += Time.deltaTime;
        if (TimeToCalculatePosition > .1)
        {
            TimeToCalculatePosition = 0;
            float[] times = new float[KartsInLobby.Count];
            float[] sortedTimes = new float[KartsInLobby.Count];

            for (int i = 0; i < KartsInLobby.Count; i++)
            {
                times[i] = KartsInLobby[i].Progress;
                sortedTimes[i] = KartsInLobby[i].Progress;
            }

            System.Array.Sort(sortedTimes);

            for (int i = 0; i < KartsInLobby.Count; i++)
            {
                for (int a = 0; a < times.Length; a++)
                {
                    if (sortedTimes[a] == KartsInLobby[i].Progress)
                    {
                        KartsInLobby[i].UpdatePlace(AI_amount + 1 - a); break;
                    }
                }
            }
        }
    }

    void CalculatePlayerFinishPositions()
    {
        float[] times = new float[KartsInLobby.Count];
        float[] sortedTimes = new float[KartsInLobby.Count];

        for (int i = 0; i < KartsInLobby.Count; i++)
        {
            times[i] = 0;
            for (int lap = 0; lap < Laps; lap++)
            {
                times[i] += KartsInLobby[i].LapTime[lap];
            }

            sortedTimes[i] = KartsInLobby[i].Progress;
        }

        System.Array.Sort(sortedTimes);

        for (int i = 0; i < KartsInLobby.Count; i++)
        {
            for (int a = 0; a < times.Length; a++)
            {
                float thistime = 0;
                for (int lap = 0; lap < Laps; lap++)
                {
                    thistime += KartsInLobby[i].LapTime[lap];
                }

                if (sortedTimes[a] == thistime)
                {
                    KartsInLobby[i].UpdateFinishPlace(AI_amount + 1 - a); break;
                }
            }
        }
    }

    public void PlayerHitCheckpoint(GameObject Kart, int Checkpoint)
    {
        int ThisKart = -1;

        for (int i = 0; i < KartsInLobby.Count; i++)
        {
            if (KartsInLobby[i].ingameKart == Kart)
            {
                ThisKart = i;
                break;
            }
        }

        if(ThisKart == -1) { Debug.LogError("No Player Kart Found"); return; }

        int PlayersCurrentCheckpoint = KartsInLobby[ThisKart].CurrentCheckpoint;

        GameObject[] NextCheckpoints = Checkpoints[PlayersCurrentCheckpoint].GetComponent<CheckpointScript>().NextCheckpoints;
        for (int i = 0; i < NextCheckpoints.Length; i++)
        {
            if (NextCheckpoints[i].GetComponent<CheckpointScript>().CheckpointNum == Checkpoint)
            {
                KartsInLobby[ThisKart].UpdateCheckpoint(Checkpoint, this);
                Debug.Log("Next Checkpoint");
                break;
            }
        }
    }

    #endregion

    #region UI
    
    [Header("UI--------------------------------------------------------------------------------------------------")]
    public float MapScale;

    public Transform PlayerBlipHolder;
    public GameObject PlayerBlip;

    bool PlayerBlipsCreated = false;

    void CreatePlayerBlips()
    {
        for (int i = 0; i < KartsInLobby.Count; i++)
        {
            GameObject newBlip = Instantiate(PlayerBlip, PlayerBlipHolder);
            newBlip.transform.name = "Blip " + i;
            newBlip.transform.localScale = KartsInLobby[i].isPlayerCar ? Vector3.one : Vector3.one * 0.8f;
            newBlip.transform.localPosition = Vector3.zero;
            newBlip.transform.Find("PlayerSprite").transform.GetComponent<Image>().sprite = KartsInLobby[i].DisplayImage;
            KartsInLobby[i].playerBlip = newBlip;

            if (!KartsInLobby[i].isPlayerCar)
            {
                Color c = Color.white;
                c.a = 0.9f;
                newBlip.transform.Find("PlayerSprite").transform.GetComponent<Image>().color = c;
                newBlip.GetComponent<Image>().enabled = false;
            }

        }
        PlayerBlipsCreated = true;
    }


    public GameObject RaceCanvas;
    public Text LapText;
    public Text TimeText;
    public Text Lap1TimeText;
    public Text Lap2TimeText;
    public Text Lap3TimeText;
    public Text PositionText;

    void UpdateUI()
    {
        RaceCanvas.SetActive(true);
        UpdateRaceInformation();
        UpdateMap();
    }

    public GameObject StartNumber_Two;
    public GameObject StartNumber_One;
    public GameObject StartNumber_Go;

    public GameObject RaceFinishedPopup;
    public void DisableStartNumbers(int num)
    {
        if(num == 1) StartNumber_Two.SetActive(false);
        if (num == 2) StartNumber_One.SetActive(false);
        if (num == 3) StartNumber_Go.SetActive(false);
    }

    void UpdateRaceInformation()
    {
        LapText.text = (KartsInLobby[0].CurrentLap + 1) + "/" + Laps;
        float time = 0;
        for (int i = 0; i < KartsInLobby[0].LapTime.Length; i++)
        {
            time += KartsInLobby[0].LapTime[i];
        }

        TimeText.text = Format.FormatTime(time);

        Lap2TimeText.gameObject.SetActive(KartsInLobby[0].CurrentLap > 0);
        Lap3TimeText.gameObject.SetActive(KartsInLobby[0].CurrentLap > 1);
        Lap1TimeText.text = Format.FormatTime(KartsInLobby[0].LapTime[0]);
        if (Lap2TimeText.gameObject.activeSelf) Lap2TimeText.text = Format.FormatTime(KartsInLobby[0].LapTime[1]);
        if (Lap3TimeText.gameObject.activeSelf) Lap3TimeText.text = Format.FormatTime(KartsInLobby[0].LapTime[2]);

        PositionText.text = Format.FormatPosition(KartsInLobby[0].Place);
    }
    
    void UpdateMap()
    {
        if (!PlayerBlipsCreated) return;
        for (int i = 0; i < KartsInLobby.Count; i++)
        {
            if (KartsInLobby[i].ingameKart == null) continue;
            Vector2 PlayerPosition = new Vector2(KartsInLobby[i].ingameKart.transform.position.x, KartsInLobby[i].ingameKart.transform.position.z);
            PlayerPosition /= MapScale;
            KartsInLobby[i].playerBlip.GetComponent<RectTransform>().localPosition = PlayerPosition;

            if (KartsInLobby[i].isPlayerCar) KartsInLobby[i].playerBlip.transform.SetAsLastSibling();
        }
    }

    public GameObject RaceStartCanvas;
    public GameObject RaceMainCanvas;
    public GameObject RaceFinishedCanvas;
    public GameObject PostRaceCanvas;

    void RaceStartUI()
    {
        RaceStartCanvas.SetActive(false);
        RaceMainCanvas.SetActive(true);
    }

    void RaceFinishedUI()
    {
        RaceMainCanvas.SetActive(false);
        RaceFinishedCanvas.SetActive(true);
    }

    void PostRaceUI()
    {
        RaceFinishedCanvas.SetActive(false);
        PostRaceCanvas.SetActive(true);
    }

    public GameObject BlackBars;
    IEnumerator MoveBlackBarsIn(float timeToMove)
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime / timeToMove;
            float size = Mathf.Lerp(1.4f, 1f, t);
            BlackBars.transform.localScale = new Vector3(1, size, 1);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        BlackBars.transform.localScale = Vector3.one;
        yield return false;
    }

    IEnumerator MoveBlackBarsOut(float timeToMove)
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime / timeToMove;
            float size = Mathf.Lerp(1f, 1.4f, t);
            BlackBars.transform.localScale = new Vector3(1, size, 1);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        BlackBars.transform.localScale = new Vector3(1, 1.4f, 1);
        yield return false;
    }


    #endregion

    #region Visual

    public int CurrentCameraViewOn = 0; //0 - player, 1 - kartspectate

    public GameObject[] CameraPositions;
    public string[] CameraPositionType; //Static, Track, Zoom  
    List<GameObject> CameraPans = new List<GameObject>();
    public GameObject tvStatic;

    void RandomiseCurrentCameraViewOn()
    {
        CurrentCameraViewOn = Random.Range(0, 2);
    }

    int CameraViewOn = -1;
    public bool CameraInPanView = false;
    float TimeInPanView = 0;
    public void UpdateCameraPanView(int view, float TimeFor)
    {
        if (KartsInLobby[0].Finished && CameraViewOn != view)
        {
            CameraInPanView = true;
            TimeInPanView = TimeFor;
            CameraViewOn = view;
            tvStatic.SetActive(true);
        }
    }



    #endregion

}
