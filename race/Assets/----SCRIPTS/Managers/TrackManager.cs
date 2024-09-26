using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    GameObject Map;

    public List<GameObject> Checkpoints;
    public List<GameObject> StartPositions;

    public GameObject KartPrefab;
    public GameObject ExplosionPrefab;
    public GameObject hologramPrefab;

    GameObject plyrKart = null;

    public float TimePassed;
    float TrackTotalLength;

    public List<int> positions = new List<int>();
    public int PlayersInGame;

    public bool RaceLoaded;
    public bool RaceReady;
    public bool RaceStarted;
    bool carsCreated;

    public int MaxLaps;
 
    public List<PlayerCar> carsInLobby;

    public List<AIRoute> AIRoutes = new List<AIRoute>();
    public AIRoute generalRoute;

    public GameManager.AIDifficulty Difficulty;
    public GameManager.RaceType RaceType;
    public int AI_Racing;

    private void Start()
    {
        LoadGame();
    }

    public void LoadGame()
    {
        GameManager gm = FindObjectOfType<GameManager>();

        if (gm.StartedSoloRaceVariables)
        {
            gm.StartedSoloRaceVariables = false;

            int MapID = gm.SoloRace_MapChosen;

            switch (gm.SoloRace_RaceDifficulty)
            {
                default:
                    Difficulty = GameManager.AIDifficulty.Basic;
                    break;
                case 1:
                    Difficulty = GameManager.AIDifficulty.Easy;
                    break;
                case 2:
                    Difficulty = GameManager.AIDifficulty.Medium;
                    break;
                case 3:
                    Difficulty = GameManager.AIDifficulty.Hard;
                    break;
            }

            RaceType = gm.SoloRace_RaceType == 0 ? GameManager.RaceType.Standard : GameManager.RaceType.TimeTrial;

            AI_Racing = gm.SoloRace_AIAmount; 

            LoadMap(MapID); //Load Map
        }
        StartCoroutine(StartUp());
    }

    void LoadMap(int mID)
    {
        GameObject _map = Instantiate(FindObjectOfType<GameManager>().GetMap(mID));
        _map.transform.position = Vector3.zero;
        _map.transform.localScale = Vector3.one;
        Map = _map;

        AIRoutes.Clear();

        AIRoutes = _map.GetComponent<MapStorage>().GetAIRoute();
        generalRoute = _map.GetComponent<MapStorage>().GetGeneralRoute();

        RenderSettings.fog = true;
        RenderSettings.fogColor = _map.GetComponent<MapStorage>().FogColour;
        RenderSettings.fogDensity = _map.GetComponent<MapStorage>().FogDensity;
        RenderSettings.skybox = _map.GetComponent<MapStorage>().SkyboxMaterial;
        RenderSettings.ambientSkyColor = _map.GetComponent<MapStorage>().GlobalLightColour;


    }

    void GetMapDetails()
    {
        GameObject CheckpointHolder = Map.transform.Find("Checkpoints").gameObject;

        CheckpointScript[] c = CheckpointHolder.transform.GetComponentsInChildren<CheckpointScript>();
        for (int i = 0; i < c.Length; i++)
        {
            Checkpoints.Add(c[i].gameObject);
        }
    }

    void GetStartPositions()
    {
        for (int i = 0; i < Map.transform.Find("KartPositions").childCount; i++)
        {
            StartPositions.Add(Map.transform.Find("KartPositions").GetChild(i).gameObject);
        }
    }
   
    IEnumerator StartUp()
    {
        GetMapDetails(); //Load the Checkpoints First

        for (int i = 0; i < Checkpoints.Count; i++)
        {
            if (i > 0)
            {
                float distance = Vector3.Distance(Checkpoints[i].transform.position, Checkpoints[i - 1].transform.position);
                TrackTotalLength += distance;
            }
        }

        TrackTotalLength *= 3;

        RaceLoaded = false;

        

        GetStartPositions(); //Get the spawn positions for karts

        StartCoroutine(CreateKarts()); //Create Karts

        while (!carsCreated) //wait for cars to be created
        {
            yield return new WaitForSeconds(Time.fixedDeltaTime*10);
        }

        //region start sequence
        RaceLoaded = true;
        yield return new WaitForSeconds(1); //intro scene
        RaceReady = true;
        plyrKart.GetComponent<CameraFollow>().camFollow = true;
        yield return new WaitForSeconds(1);
        Transform IntroNumbers = plyrKart.transform.Find("KartUI").Find("IntroSequence").Find("ScreenNumbers");
        IntroNumbers.GetComponent<Animator>().SetTrigger("ThreeShow");
        yield return new WaitForSeconds(1.5f);
        IntroNumbers.GetComponent<Animator>().SetTrigger("TwoShow");
        yield return new WaitForSeconds(1.5f);
        IntroNumbers.GetComponent<Animator>().SetTrigger("OneShow");
        yield return new WaitForSeconds(1.5f); //321
        IntroNumbers.GetComponent<Animator>().SetTrigger("GoShow");
        RaceStarted = true;
        yield return null;
    }

    void Update()
    {
        if (RaceStarted)
        {
            UpdateKarts();
            CalculatePositions();
        }
    }

    int num = 0;
    GameObject plrKart;
    IEnumerator CreateKarts()
    {
        num = 0;
        carsInLobby = new List<PlayerCar>();

        //player kart
        CreatePlayerKart();

        num++;



        yield return new WaitForSeconds(.5f);

        //other karts
        StartCoroutine(CreateAIs());
        carsCreated = true;
        
    }

    void CreatePlayerKart()
    {
        plrKart = Instantiate(KartPrefab);
        plrKart.transform.localScale = Vector3.one;
        Player player = FindObjectOfType<Player>();
        plrKart.GetComponent<KartController>().thisKart = player.playersKart[player.KartSelected];
        plrKart.GetComponent<KartController>().BuildKart();

        plrKart.transform.position = StartPositions[num].transform.position;
        plrKart.transform.rotation = StartPositions[num].transform.rotation;
        plrKart.GetComponent<CameraFollow>().cam = Camera.main;
        plrKart.GetComponent<CameraFollow>().camFollow = false;
        plrKart.GetComponent<KartController>().KartID = num;
        plrKart.transform.SetParent(transform.Find("SpawnedKarts"));
        plrKart.name = num.ToString();
        plyrKart = plrKart;

        PlayerCar pc = new PlayerCar();
        pc.isPlayerCar = true;
        pc.UpdatePosition(plrKart.transform.position);
        pc.UpdateRotation(plrKart.transform.Find("main").eulerAngles);
        //pc.UpdateCheckpoint(0, this);
        pc.CurrentLap = 0;
        pc.LapTime = new float[3];
        pc.ingameKart = plrKart;
        carsInLobby.Add(pc);
    }

    IEnumerator CreateAIs()
    {
        AIManager aiM = FindObjectOfType<AIManager>();
        for (int i = 1; i < PlayersInGame; i++)
        {
            int AIid = Random.Range(0, aiM.AICharacters.Count);
            AICharacter selectedCharacter = aiM.AICharacters[AIid];

            GameObject aiKart = Instantiate(KartPrefab);
            aiKart.transform.localScale = Vector3.one;
            aiKart.GetComponent<KartController>().LoadAIKart(AIid);
            aiKart.GetComponent<KartController>().BuildKart();

            aiKart.transform.position = StartPositions[num].transform.position;
            aiKart.transform.rotation = StartPositions[num].transform.rotation;
            aiKart.GetComponent<CameraFollow>().camFollow = false;
            Destroy(aiKart.transform.Find("KartUI").gameObject);
            aiKart.GetComponent<KartController>().AIKart = true;
            aiKart.GetComponent<KartController>().KartID = num;
            aiKart.GetComponent<KartController>().IsPlayerKart = false;
            aiKart.transform.SetParent(transform.Find("SpawnedKarts"));
            aiKart.name = num.ToString();

            PlayerCar aipc = new PlayerCar();
            aipc.isPlayerCar = true;
            aipc.UpdatePosition(plrKart.transform.position);
            aipc.UpdateRotation(plrKart.transform.Find("main").eulerAngles);
            //aipc.UpdateCheckpoint(0, this);
            aipc.isAI = true;
            aipc.aiID = AIid;

            aipc.CurrentLap = 0;
            aipc.LapTime = new float[3];
            aipc.ingameKart = aiKart;
            carsInLobby.Add(aipc);

            num++;
            yield return new WaitForSeconds(.15f);
        }
    }

    void UpdateKarts()
    {
        if (carsInLobby.Count == 0) { Debug.LogError("No karts in lobby"); return; }
        for (int i = 0; i < carsInLobby.Count; i++)
        {
            if (carsInLobby[i] != null)
            {
                if (carsInLobby[i].Finished == true)
                {
                    if (carsInLobby[i].isPlayerCar)
                    {
                        carsInLobby[i].ingameKart.GetComponent<KartController>().AIKart = true;
                    }
                }

                #region Update Kart
                carsInLobby[i].UpdateLapTime(Time.deltaTime); //lap time
                GameObject k = carsInLobby[i].ingameKart;
                if (k != null)
                {
                    carsInLobby[i].UpdatePosition(k.transform.position); //position
                    Vector3 kartRotation = k.transform.eulerAngles + k.transform.Find("main").eulerAngles;
                    carsInLobby[i].UpdateRotation(kartRotation); //rotation
                }
                #endregion

                #region Checkpoint Check
                List<GameObject> nextCheckpoints = new List<GameObject>(); //get next checkpoints
                for (int x = 0; x < Checkpoints[carsInLobby[i].CurrentCheckpoint].GetComponent<CheckpointScript>().NextCheckpoints.Length; x++)
                {
                    nextCheckpoints.Add(Checkpoints[carsInLobby[i].CurrentCheckpoint].GetComponent<CheckpointScript>().NextCheckpoints[x]);
                }

                bool hitNextCheckpoint = false;
                int cnum = 0;
                if (k != null)
                {
                    Collider[] col = Physics.OverlapSphere(k.transform.position, 2); //check karts position
                    for (int c = 0; c < col.Length; c++)
                    {
                        if (col[c].tag == "Checkpoint")
                        { //check collision c is a checkpoint
                            for (int che = 0; che < nextCheckpoints.Count; che++) //check if collision c is one of the next checkpoints
                            {
                                if (col[c].GetComponent<CheckpointScript>().CheckpointNum == nextCheckpoints[che].GetComponent<CheckpointScript>().CheckpointNum) { hitNextCheckpoint = true; cnum = nextCheckpoints[che].GetComponent<CheckpointScript>().CheckpointNum; break; }
                            }
                        }
                    }
                }

                #endregion

                #region Progress Check
                float prog = carsInLobby[i].CurrentLap * 1000 + carsInLobby[i].CurrentCheckpoint * 10;
                if (carsInLobby[i].CurrentCheckpoint > 0) {
                    int current = carsInLobby[i].CurrentCheckpoint;
                    float distance = Vector3.Distance(Checkpoints[current].transform.position, Checkpoints[current-1].transform.position);
                    float distancefromcheck = Vector3.Distance(carsInLobby[i].ingameKart.transform.position, Checkpoints[current].transform.position);
                    prog += distancefromcheck/distance;
                }
                carsInLobby[i].UpdateProgress(prog);
                #endregion
            }
        }



    }

    float t;
    bool checkingpos;
    [SerializeField] float[] times;
    [SerializeField] float[] sortedTimes;
    void CalculatePositions()
    {
        t += Time.deltaTime;
        if (t > .25f)
        {
            t = 0;
            Debug.Log("Checking Position");
            times = new float[carsInLobby.Count];
            sortedTimes = new float[carsInLobby.Count];

            for (int i = 0; i < PlayersInGame; i++)
            {
                times[i] = carsInLobby[i].Progress;
                sortedTimes[i] = carsInLobby[i].Progress;
            }

            System.Array.Sort(sortedTimes);

            for (int i = 0; i < PlayersInGame; i++)
            {
                for (int a = 0; a < times.Length; a++)
                {
                    if (sortedTimes[a] == carsInLobby[i].Progress)
                    {
                        carsInLobby[i].UpdatePlace(8 - a); break;
                    }
                }
            }
        }
    }

    public int UpdateAIRoute()
    {
        int a = 0;
        int l = AIRoutes.Count;
        a = Random.Range(0, l);
        return a;
    }

    public int GetLap(int KartID)
    {
        return carsInLobby[KartID].CurrentLap+1;
    }

    public IEnumerator RespawnKart(int KartID, int AIcurrentpoint)
    {
        Destroy(carsInLobby[KartID].ingameKart);
        GameObject hologram = Instantiate(hologramPrefab);
        hologram.transform.position = Checkpoints[carsInLobby[KartID].CurrentCheckpoint].transform.Find("respawnPos").position;

        hologram.transform.localScale = new Vector3(7, 7, 7);

        yield return new WaitForSeconds(.75f);


        GameObject Kart = Instantiate(KartPrefab);
        Kart.transform.localScale = Vector3.one;
        Kart.transform.position = Checkpoints[carsInLobby[KartID].CurrentCheckpoint].transform.Find("respawnPos").position;
        Kart.transform.rotation = Checkpoints[carsInLobby[KartID].CurrentCheckpoint].transform.Find("respawnPos").rotation;
      
        Kart.GetComponent<KartController>().KartID = KartID;
        Kart.transform.SetParent(transform.Find("SpawnedKarts"));
        Kart.name = KartID.ToString();


        if (!carsInLobby[KartID].isAI)
        {
            Kart.GetComponent<CameraFollow>().cam = Camera.main;
            Kart.GetComponent<CameraFollow>().camFollow = true;
            Kart.GetComponent<KartController>().AIKart = false;

            Kart.GetComponent<KartController>().thisKart = FindObjectOfType<Player>().playersKart[FindObjectOfType<Player>().KartSelected];
            Kart.GetComponent<KartController>().BuildKart();
            plyrKart = Kart;

        }
        else
        {
            Kart.GetComponent<KartController>().AIKart = true;
            Kart.GetComponent<KartController>().IsPlayerKart = false;
            Kart.GetComponent<KartController>().thisKart = FindObjectOfType<AIManager>().AICharacters[carsInLobby[KartID].aiID].charactersCar;
            Kart.GetComponent<KartController>().BuildKart();
            Destroy(Kart.transform.Find("KartUI").gameObject);
            yield return new WaitForSeconds(Time.deltaTime);
            Kart.GetComponent<KartController>().CurrentAIRoute = AIRoutes[UpdateAIRoute()].positions;
            Kart.GetComponent<KartController>().currentPoint = AIcurrentpoint;

        }

        carsInLobby[KartID].ingameKart = Kart;
        yield return false;
    }


}
