using Formatting;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KartController : MonoBehaviour
{
    private Rigidbody rb;
    private InputManager im;
    private TrackManager tm;

    public int KartID;
    public bool CanMove;
    public bool IsPlayerKart = true;
    public int AIKartID;
    public Car thisKart;
    int LapOn;
    int Position;
    [SerializeField] bool OnGrass;

    #region Kart Statistics
    [Header("Kart Statistics")]

    public float customisedWeight;
    public float customisedMaxSpeed;
    public float customisedAcceleration;
    public float customisedBoostGain;
    public float customisedBoostCapacity;
    public float customisedBoostAcceleration;
    public float customisedBoostMaxSpeed;
    public float customisedSteerSpeed;
    public float customisedBoostUsage;
    #endregion

    #region Player Settings
    [Header("Player Settings")]

    public float DriveDeadzone;
    public float SteerDeadzone;
    public bool driftButtonPressedThisFrame;
    bool driftButtonUp;
    public float FieldOfView;
    public float FieldOfViewScale;
    #endregion

    #region Kart Base Variables
    [Header("Kart Base Variables")]
    [SerializeField] float floorCheck; //distance to ground for kart to be "touching"
    [SerializeField] bool TouchingGround; //whether the kart is touching the ground
    [SerializeField] bool GravityAffect; // whether fall gravity is affecting the kart
    [SerializeField] float AirTime; //air time of the kart
    [SerializeField] float GravityAmount; // the base gravity amount
    [SerializeField] float FallAmount; //the gravity amount when falling

    public float BoostLoss; //the rate at which boost speed diminishes

    [SerializeField] float TyreMaxTurn; //the maximum angle the tyres can turn

    [SerializeField] float DriftRotate; //amount the kart will rotate when drifting
    float driftRotAm; //amount kart has rotated when drifting

    [SerializeField] float DriftPivot; //amount the kart will pivot when drifting
    float driftPivotAm; //amount kart has pivoted when drifting;

    float driftCounterTurn; //the time that the player has been counter turning a drift;

    public bool AIKart;
    public float CarTurnInput;
    public float CarPivotInput;
    public float CarAccelerateInput;
    public float CarDriftInput;
    public float CarBoostInput;
    public float CarPowerUpUse;
    #endregion

    #region Acceleration Variables
    [Header("Acceleration")]

    [SerializeField] float CurrentSpeed = 0; 
    [SerializeField] float RealSpeed; //real speed of the kart

    private float carMaxSpeed; 
    private float carAcceleration;

    public float ReverseMaxSpeed;
    public float ReverseAcceleration;

    public float StopSpeed; //speed which kart stops
    #endregion

    #region Steering Variables
    [Header("Steering")]

    

    public float carSteerSpeed;

    [SerializeField] float steerDirection;
    [SerializeField] float driftTime;
    [SerializeField] float currentTyreRot;

    float steerAm; //float determining the amount the tyres have steered
    #endregion

    #region Drifting Variables
    [Header("Drifting")]

    [SerializeField] bool driftLeft = false;
    [SerializeField] bool driftRight = false;
    public bool isSliding = false;

    [SerializeField] LayerMask raycastMask;
    [SerializeField] GameObject driftParticles;

    [SerializeField] float DriftThreshhold;
    public float outwardDriftForce = 2;
    float landedTime;
    [SerializeField] float DriftTransitionSpeed;
    [SerializeField] float DriftTurnNeed;

    [SerializeField] float DriftCounterTurnMax;
    bool CloseToGround;
    #endregion

    #region Particle Variables
    [Header("Particles")]

    public GameObject boostFire;
    public GameObject boostExplosion;

    public GameObject windParticles;
    #endregion

    #region Boost Variables
    [Header("Boost")]
    public float BoostAmount;
    private float carBoostCapacity;
    private float carBoostAcceleration;
    private float carBoostMaxSpeed;
    private float carBoostGain;
    public float BoostTime = 0;

    public bool boosting;

    public float ExtraBoostTime;
    public float ExtraBoost;

    [SerializeField] float AirTimeForBoost;
    [SerializeField] float AirTimeAdd;
    #endregion

#region Unity Functions
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        im = FindObjectOfType<InputManager>();
        tm = FindObjectOfType<TrackManager>();
    }

    void Update()
    {
        CalculateStats();
        Inputs();

        if (!AIKart)
        {
            GetPlayerInputs();
            UpdateUI();
            Visual();
        }
        else
        {
            AI();
        }

        if (tm)
        {
            if (tm.RaceStarted && CanMove)
            {
                GroundCheck();
                Move();
                TyreSteer();
                Steer();
                Drift();
                Boosts();
                Particles();
                PowerUp();
             }
        }
        else
        {
            if (CanMove)
            {
                GroundCheck();
                Move();
                TyreSteer();
                Steer();
                Drift();
                Boosts();
                Particles();
                PowerUp();
            }
        }
        
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R) && !AIKart)
        {
            StartCoroutine(RespawnKart(currentPoint));
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.E) && !AIKart)
        {
            StartCoroutine(DestroyKart());
        }


        Audio();
    }
    #endregion

    #region Build Kart
    [SerializeField] GameObject placedBlocks;
    BlockStorage blockStorage;
    public List<GameObject> steerTyres = new List<GameObject>();
    public List<GameObject> staticTyres = new List<GameObject>();
    public List<GameObject> boosters = new List<GameObject>();

    public void LoadPlayerKart(int kart) {

        Debug.Log(transform.name + " Loaded Player Kart");
        Player player = FindObjectOfType<Player>();
        Car playersKart = player.playersKart[player.KartSelected];
        Car thisCar = Format.CopyCar(playersKart);
        thisKart = thisCar;
        BuildKart();
    }

    public void LoadAIKart(int kart)
    {
        Debug.Log(transform.name + "Loaded AI Kart");
        Car aikart = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AIManager>().AICharacters[kart].charactersCar;
        Car thisCar = Format.CopyCar(aikart);
        thisKart = thisCar;
        IsPlayerKart = false;
        BuildKart();
    }

    public GameObject CharacterDisplay;
    public GameObject SpawnedCharacter;

    public void BuildKart()
    {
        steerTyres = new List<GameObject>(0);
        staticTyres = new List<GameObject>(0);
        boosters = new List<GameObject>(0);

        float lowestY=0;
        float lowestX=0;
        float lowestZ=0;
        float highestY=0;
        float highestX=0;
        float highestZ=0;

        #region Blocks
        blockStorage = GameObject.Find("GameManager").GetComponent<BlockStorage>();
        for (int i = 0; i < thisKart.blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[thisKart.blocks[i]];
            GameObject block = Instantiate(bl.buildingPrefab);
            block.transform.SetParent(placedBlocks.transform);
            block.transform.localScale = new Vector3(bl.SizeX, bl.SizeY, bl.SizeZ);
            block.transform.localPosition = thisKart.positions[i];
            block.transform.localRotation = Quaternion.Euler(thisKart.rotations[i]);

            if (bl.canBeColoured)
            {
                MeshRenderer[] meshes = block.GetComponentsInChildren<MeshRenderer>();
                for (int mesh = 0; mesh < meshes.Length; mesh++) { if (meshes[mesh].transform.parent.name == "paintable") meshes[mesh].material = blockStorage.materials[thisKart.materials[i]].LinkedMaterial; }
            }
            bool tyre = false;
            if (bl.blockType == GameManager.BlockType.Tyre) { staticTyres.Add(block); tyre = true; }
            if (bl.blockType == GameManager.BlockType.Tyre) { steerTyres.Add(block); tyre = true; }
            if (bl.blockType == GameManager.BlockType.Booster) { boosters.Add(block); }

            if (tyre)
            {
                if (block.transform.rotation.y < 0)
                {
                    block.transform.Find("tyre").Find("driftParticles").transform.localPosition = new Vector3(-0.25f, .5f, 0);
                    block.transform.Find("tyre").Find("driftSmoke").transform.localPosition = new Vector3(-0.5f, 0, 0);
                    block.transform.Find("tyre").Find("driftParticles").transform.localEulerAngles = new Vector3(0, 0, -45);
                }
                else
                {
                    block.transform.Find("tyre").Find("driftParticles").transform.localPosition = new Vector3(0.25f, .5f, 0);
                    block.transform.Find("tyre").Find("driftSmoke").transform.localPosition = new Vector3(0.5f, 0, 0);
                    block.transform.Find("tyre").Find("driftParticles").transform.localEulerAngles = new Vector3(0, 0, 45);
                }
            }

            if(thisKart.positions[i].y > highestY)  { highestY = thisKart.positions[i].y; }
            if (thisKart.positions[i].x > highestX) { highestX = thisKart.positions[i].x; }
            if (thisKart.positions[i].z > highestZ) { highestZ = thisKart.positions[i].z; }
            if (thisKart.positions[i].y < lowestY)  { lowestY = thisKart.positions[i].y; }
            if (thisKart.positions[i].x < lowestX)  { lowestX = thisKart.positions[i].x; }
            if (thisKart.positions[i].z < lowestZ)  { lowestZ = thisKart.positions[i].z; }

            block.transform.name = i.ToString();
            block.layer = LayerMask.NameToLayer("Kart");

            float carWidth = highestX - lowestX;
            float carHeight = highestY - lowestY;
            float carLength = highestZ - lowestZ;
            GetComponent<CapsuleCollider>().radius = ((carWidth + carHeight) / 4) - 0.4f;
            GetComponent<CapsuleCollider>().height = carLength;

        }
        #endregion

        #region Seat

        GameObject seat = transform.Find("main").Find("body").Find("seat").gameObject;
        MeshRenderer[] seatPaintables = seat.transform.Find("paintable").GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < seatPaintables.Length; i++)
        {
            seatPaintables[i].material = blockStorage.materials[thisKart.seatColour].LinkedMaterial;
        }

        SpawnedCharacter = Instantiate(CharacterDisplay);
        Transform charPosition = seat.transform.Find("driverPosition");

        SpawnedCharacter.transform.SetParent(charPosition);
        SpawnedCharacter.transform.localScale = Vector3.one;
        SpawnedCharacter.transform.localRotation = Quaternion.identity;
        SpawnedCharacter.transform.localPosition = Vector3.zero;

        DisplayCharacterScript disp = SpawnedCharacter.GetComponent<DisplayCharacterScript>();
        CustomisationManager cM = FindObjectOfType<CustomisationManager>();
        AIManager aiM = FindObjectOfType<AIManager>();

        Debug.Log("Player kart? " + IsPlayerKart);
        if (IsPlayerKart)
        {
            Debug.Log("Loaded Player Character");
            disp.HeadChosen = cM.HeadSelected;
            disp.BodyChosen = cM.BodySelected;
            disp.LegsChosen = cM.LegsSelected;
            disp.AnimationStyleChosen = cM.AnimationStyleSelected;
        }
        else
        {
            disp.HeadChosen = aiM.AICharacters[AIKartID].CharacterHead;
            disp.BodyChosen = aiM.AICharacters[AIKartID].CharacterBody;
            disp.LegsChosen = aiM.AICharacters[AIKartID].CharacterLegs;
            disp.AnimationStyleChosen = aiM.AICharacters[AIKartID].CharacterStyle;
        }
        disp.RefreshCharacter();
        disp.PlayAnimation("DRIVE_IDLE");

        #endregion

        #region Kart Stats
        customisedWeight = 0;
        for (int i = 0; i < thisKart.blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[thisKart.blocks[i]];
            customisedWeight += bl.Weight;
        }

        customisedMaxSpeed = 0;
        for (int i = 0; i < thisKart.blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[thisKart.blocks[i]];
            customisedMaxSpeed += bl.MaxSpeed;
        }

        customisedAcceleration = 0;
        for (int i = 0; i < thisKart.blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[thisKart.blocks[i]];
            customisedAcceleration += bl.Acceleration;
        }

        customisedBoostGain = 0; //average
        int BGs = 0;
        for (int i = 0; i < thisKart.blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[thisKart.blocks[i]];
            //if (bl.BoostGain > 0) BGs++;
            //customisedBoostGain += bl.BoostGain;
        }
        customisedBoostGain /= (BGs > 0 ? BGs : 1);

        customisedBoostCapacity = 0;
        for (int i = 0; i < thisKart.blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[thisKart.blocks[i]];
            customisedBoostCapacity += bl.BoostCapacity;
        }

        customisedBoostUsage = 0;
        for (int i = 0; i < thisKart.blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[thisKart.blocks[i]];
            //customisedBoostUsage += bl.BoostUsage;
        }

        customisedBoostAcceleration = 0; //average
        int BAs = 0;
        for (int i = 0; i < thisKart.blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[thisKart.blocks[i]];
            //if (bl.BoostAcceleration > 0) BAs++;
            //customisedBoostAcceleration += bl.BoostAcceleration;
        }
        customisedBoostAcceleration /= (BAs > 0 ? BAs : 1);

        customisedBoostMaxSpeed = 0; //average
        int BMSs = 0;
        for (int i = 0; i < thisKart.blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[thisKart.blocks[i]];
            if (bl.BoostMaxSpeed > 0) BMSs++;
            customisedBoostMaxSpeed += bl.BoostMaxSpeed;
        }
        customisedBoostMaxSpeed /= (BMSs > 0 ? BMSs : 1);

        customisedSteerSpeed = 0;
        int SSs = 0;
        for (int i = 0; i < thisKart.blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[thisKart.blocks[i]];
            if (bl.SteerSpeed > 0) SSs++;
            customisedSteerSpeed += bl.SteerSpeed;
        }
        customisedSteerSpeed /= (SSs > 0 ? SSs : 1);
        #endregion
    }
    #endregion
    void CalculateStats()
    {
        float scaled = customisedWeight / 100;

        scaled -= 1;
        scaled /= 4;
        scaled += 1;
        scaled = 1 - scaled;
        scaled += 1;

        scaled = Mathf.Clamp(scaled, 0.8f, 1.2f);
        float minScaled = ((scaled - 1) / 2) + 1;
        float grassScale = OnGrass ? 0.75f : 1;
        float grassBoostScale = OnGrass ? 0.9f : 1;

        carMaxSpeed = customisedMaxSpeed * scaled * grassScale;
        carAcceleration = customisedAcceleration * minScaled * grassScale;
        carBoostGain = customisedBoostGain;
        carBoostCapacity = customisedBoostCapacity;
        carBoostMaxSpeed = carMaxSpeed + customisedBoostMaxSpeed * minScaled * grassBoostScale;
        carBoostAcceleration = customisedBoostAcceleration * minScaled;
        carSteerSpeed = customisedSteerSpeed * minScaled;

        ReverseAcceleration = carAcceleration / 4;
        ReverseMaxSpeed = carMaxSpeed / 4;

        LapOn = tm ? tm.GetLap(KartID) : 0;
    }

    #region Movement
    [SerializeField] float accel = 0;

    void Move()
    {
        RealSpeed = transform.InverseTransformDirection(rb.velocity).z;
       
        accel = Mathf.Lerp(accel, CarAccelerateInput, 8 * Time.deltaTime);

        if (boosting)
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, ExtraBoostTime > 0 ? carBoostMaxSpeed+ExtraBoost : carBoostMaxSpeed , Time.deltaTime * carBoostAcceleration);
        }
        else if(ExtraBoostTime > 0)
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, carMaxSpeed+ExtraBoost, Time.deltaTime * carBoostAcceleration);
        }
        else
        {
            if (accel > DriveDeadzone)
            {
                if (CurrentSpeed > carMaxSpeed)
                {
                    CurrentSpeed = Mathf.Lerp(CurrentSpeed, carMaxSpeed, Time.deltaTime * BoostLoss);
                }
                else
                {
                    CurrentSpeed = Mathf.Lerp(CurrentSpeed, carMaxSpeed, Time.deltaTime * carAcceleration);

                }
            }
            else if (accel < -DriveDeadzone)
            {
                CurrentSpeed = Mathf.Lerp(CurrentSpeed, -ReverseMaxSpeed, CurrentSpeed > 0 ? Time.deltaTime * carAcceleration : Time.deltaTime * ReverseAcceleration);
                if (RealSpeed < 0.05f && CurrentSpeed > 0.5f)
                {
                    CurrentSpeed = 0;
                }
            }
            else
            {
                CurrentSpeed = Mathf.Lerp(CurrentSpeed, 0, Time.deltaTime * StopSpeed);
            }

         
        }

        float driftscale = Mathf.Clamp(Mathf.Abs(CarTurnInput), 0.8f, 1f); //scale depending on turn
        Vector3 xAdd = driftRight ? transform.right*-outwardDriftForce*driftscale : driftLeft ? transform.right*outwardDriftForce*driftscale : Vector3.zero; // when drifting
        //set the speed
        Vector3 vel = transform.forward * CurrentSpeed;
        vel += xAdd*CurrentSpeed;

        if (GravityAffect) //artificial gravity
        {
            vel.y = rb.velocity.y - FallAmount * Time.deltaTime;
        }
        else
        {
            vel.y = rb.velocity.y - GravityAmount * Time.deltaTime;
        }
        rb.velocity = vel;

        if (ExtraBoostTime > 0) { ExtraBoostTime -= Time.deltaTime; } //remove extra boost
        
    }
  
    void TyreSteer()
    {
        float turn = CarTurnInput;
        float turnifzero = turn > SteerDeadzone ? turn : turn < -SteerDeadzone ? turn : .5f;
        float maxTemp = TyreMaxTurn;
        if (isSliding) { maxTemp *= 1.25f; }

        currentTyreRot = Mathf.Lerp(currentTyreRot, turn > SteerDeadzone ? maxTemp : turn < -SteerDeadzone ? -maxTemp : 0, Mathf.Abs(turnifzero) * Time.deltaTime * 5);


        for (int i = 0; i < steerTyres.Count; i++)
        {

            float dir = 1;
            if(steerTyres[i].transform.localRotation.y < 0) { dir = -1; }
            steerTyres[i].transform.GetChild(0).Find("tyremain").localEulerAngles = new Vector3(-dir * currentTyreRot, 0, 0);

            steerTyres[i].transform.GetChild(0).Find("tyremain").Find("wheel").Rotate(-dir * 45 * CurrentSpeed * Time.deltaTime, 0, 0);
        } 

        for (int i = 0; i < staticTyres.Count; i++)
        {
            float dir = 1;
            if (staticTyres[i].transform.localRotation.y < 0) { dir = -1; }
            staticTyres[i].transform.GetChild(0).Find("tyremain").Find("wheel").Rotate(-dir * 45 * CurrentSpeed * Time.deltaTime, 0, 0);
        }

        
    }

    void Steer()
    {
        steerDirection = Mathf.Lerp(steerDirection, CarTurnInput, 8*Time.deltaTime); 
        Vector3 SteerDirVector;

        float steerAmount;

        steerAmount = RealSpeed > 10 ? (RealSpeed / 4) : steerAmount = (RealSpeed / 1.5f) ;
        steerAmount = RealSpeed > 0.05f ? Mathf.Clamp(steerAmount, 5, 9999999) : 0;
        if (isSliding) { steerAmount *= 1.25f; }
        steerAmount = Mathf.Clamp(steerAmount, 5f, Mathf.Infinity);
        steerAmount *= steerDirection;

        if(CurrentSpeed < 0) { steerAmount *= -1f; }

        if (driftRight)
        {
            steerAmount = Mathf.Clamp(steerAmount, -DriftCounterTurnMax, Mathf.Infinity);
        }

        if (driftLeft)
        {
            steerAmount = Mathf.Clamp(steerAmount, -Mathf.Infinity, DriftCounterTurnMax);
        }

        SteerDirVector = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + steerAmount, transform.eulerAngles.z);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, SteerDirVector, carSteerSpeed * Time.deltaTime);
    }

    float TimeSinceLastDriftSound = 0f;


    public int DriftStageOn = 0;
    //0 stage - not drifting
    //1st stage - smoke
    //2nd stage - small flames
    //3rd stage - medium flames
    //4th stage - big flames

    void Drift()
    {
        TimeSinceLastDriftSound += Time.deltaTime;
        if (driftButtonPressedThisFrame && CloseToGround && landedTime > 0.1f)
        {
            GetComponent<Animator>().SetTrigger("KartHop");
        }

        if (driftButtonPressedThisFrame && CloseToGround && landedTime > 0.1f && RealSpeed > DriftThreshhold) // drift from hop
        {
            Debug.Log("StartDrifting");
            if(steerDirection > DriftTurnNeed)
            {
                driftRight = true;
                driftLeft = false;

            }else if(steerDirection < -DriftTurnNeed)
            {
                driftRight = false;
                driftLeft = true;
            }

        }

        if (CarDriftInput>0 && CloseToGround && landedTime < 0.1f && RealSpeed > DriftThreshhold) // drift from landing
        {
            if(steerDirection > DriftTurnNeed)
            {
                driftRight = true;
                driftLeft = false;

            }else if(steerDirection < -DriftTurnNeed)
            {
                driftRight = false;
                driftLeft = true;

            }
        }
        isSliding = driftRight || driftLeft;

        bool driftEnded = false;
        

        if (CarDriftInput > 0 && CloseToGround && RealSpeed > DriftThreshhold && isSliding && driftCounterTurn < 3)
        {
            driftEnded = false;
            driftTime += Time.deltaTime;

            #region Drift Particles
            //particles
            for (int i = 0; i < staticTyres.Count; i++)
            { 
                ParticleSystem smoke = staticTyres[i].transform.Find("tyre").Find("driftSmoke").GetComponent<ParticleSystem>();
                smoke.transform.eulerAngles = Vector3.zero;
                ParticleSystem yellow = staticTyres[i].transform.Find("tyre").Find("driftParticles").transform.Find("first").GetComponent<ParticleSystem>();
                ParticleSystem orange = staticTyres[i].transform.Find("tyre").Find("driftParticles").transform.Find("second").GetComponent<ParticleSystem>();
                ParticleSystem red = staticTyres[i].transform.Find("tyre").Find("driftParticles").transform.Find("third").GetComponent<ParticleSystem>();


                if (driftTime > .3f && RealSpeed > 20 && !smoke.isPlaying) if (DriftStageOn == 0) { DriftStageOn = 1; smoke.Play(); }
                if (driftTime > .5f && !yellow.isPlaying) if (DriftStageOn == 1) { DriftStageOn = 2; yellow.Play(); }
                if (driftTime > 1f && !orange.isPlaying) if (DriftStageOn == 2) { DriftStageOn = 3; orange.Play(); }
                if (driftTime > 1.5f && !red.isPlaying) if (DriftStageOn == 3) { DriftStageOn = 4; red.Play(); }

                staticTyres[i].transform.Find("tyre").Find("driftParticles").transform.Find("driftLight").GetComponent<Light>().intensity = Mathf.Lerp(0, 1.5f, driftTime / 4);
                staticTyres[i].transform.Find("tyre").Find("driftParticles").transform.Find("driftLight").gameObject.SetActive(true);
            }

            for (int i = 0; i < steerTyres.Count; i++)
            {
                ParticleSystem smoke = steerTyres[i].transform.Find("tyre").Find("driftSmoke").GetComponent<ParticleSystem>();
                smoke.transform.eulerAngles = Vector3.zero;
                if (driftTime > .3f && RealSpeed > 20 && !smoke.isPlaying) smoke.Play();
            }
            #endregion
        }
        else
        {
            if(isSliding)
            {
                if (TimeSinceLastDriftSound > 0.1f)
                {
                    GetComponent<AudioSource>().PlayOneShot(BoostIncreaseSoundEffect, 0.25f);
                    TimeSinceLastDriftSound = 0f;
                }

                driftEnded = true;
                isSliding = false;
            }
            driftLeft = false;
            driftRight = false;

            BoostAmount += (DriftStageOn * carBoostGain)/3;

            //BoostAmount += driftTime * carBoostGain;

            DriftStageOn = 0;

            
            BoostAmount = Mathf.Clamp(BoostAmount, 0, carBoostCapacity);

            driftTime = 0;
            driftCounterTurn = 0;

            #region Disable Drift Particles
            for (int i = 0; i < staticTyres.Count; i++)
            {
                staticTyres[i].transform.Find("tyre").Find("driftParticles").transform.Find("first").GetComponent<ParticleSystem>().Stop();
                staticTyres[i].transform.Find("tyre").Find("driftParticles").transform.Find("second").GetComponent<ParticleSystem>().Stop();
                staticTyres[i].transform.Find("tyre").Find("driftParticles").transform.Find("third").GetComponent<ParticleSystem>().Stop();
                staticTyres[i].transform.Find("tyre").Find("driftSmoke").GetComponent<ParticleSystem>().Stop();
                float inte = staticTyres[i].transform.Find("tyre").Find("driftParticles").transform.Find("driftLight").GetComponent<Light>().intensity;
                staticTyres[i].transform.Find("tyre").Find("driftParticles").transform.Find("driftLight").GetComponent<Light>().intensity = Mathf.Lerp(inte, 0, 5*Time.deltaTime);
            }

            for (int i = 0; i < steerTyres.Count; i++)
            {
                steerTyres[i].transform.Find("tyre").Find("driftSmoke").GetComponent<ParticleSystem>().Stop();
            }
            #endregion

        }

        float amb = DriftRotate * Random.Range(0.8f, 1.2f);
        amb *= Mathf.Abs(CarTurnInput);
        amb = Mathf.Clamp(amb, 10, 60);

        float bmb = DriftPivot * Random.Range(0.8f, 1.2f);
        bmb *= Mathf.Abs(CarTurnInput);
        bmb = Mathf.Clamp(amb, 5, 20);

        driftPivotAm = 0;

        if (driftRight)
        {
            driftRotAm = Mathf.Lerp(driftRotAm, amb, DriftTransitionSpeed * Time.deltaTime);
            //driftPivotAm = Mathf.Lerp(driftPivotAm, -bmb, DriftTransitionSpeed * Time.deltaTime * 5);
            driftCounterTurn = CarTurnInput <= 0 ? driftCounterTurn + Time.deltaTime : 0;
        }
        else if (driftLeft)
        {
            driftRotAm = Mathf.Lerp(driftRotAm, -amb, DriftTransitionSpeed * Time.deltaTime);
            //driftPivotAm = Mathf.Lerp(driftPivotAm, bmb, DriftTransitionSpeed * Time.deltaTime * 5);
            driftCounterTurn = CarTurnInput >= 0 ? driftCounterTurn + Time.deltaTime : 0;
        }
        else
        {
            driftRotAm = Mathf.Lerp(driftRotAm, 0, 4 * Time.deltaTime);
            //driftPivotAm = Mathf.Lerp(driftPivotAm, 0, DriftTransitionSpeed * Time.deltaTime * 5);
        }

        transform.Find("main").localEulerAngles = new Vector3(0, driftRotAm, driftPivotAm);

    }

    void GroundCheck()
    {
        GetComponent<Collider>().enabled = false;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, floorCheck*2, raycastMask))
        {
            if (hit.transform != transform)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 8f * Time.deltaTime);
                CloseToGround = true;
            }
        }
        else
        {
            CloseToGround = false;
        }


        if (Physics.Raycast(transform.position, Vector3.down, out hit, floorCheck, raycastMask))
        {
            Debug.Log(hit.transform.name);
            if (hit.transform != transform)
            {
                if (TouchingGround == false) { GetComponent<Animator>().SetTrigger("KartLand"); PlayLandSound(); TouchingGround = true; }
                landedTime += Time.deltaTime;
                if (AirTime > AirTimeForBoost)
                {
                    IncreaseExtraBoost(AirTimeAdd);
                }
                AirTime = 0;
            }
            else{
                TouchingGround = false;
                landedTime = 0;
            }

            OnGrass = false;
            if (hit.transform.tag == "Grass")
            {
                OnGrass = true;
            }
        }
        else
        {
            TouchingGround = false;
            landedTime = 0;
        }

        if (Physics.Raycast(transform.position, Vector3.down, out hit, floorCheck, raycastMask))
        {
            if (hit.transform != transform)
            {
                GravityAffect = false;
            }
        }
        else
        {
            GravityAffect = true;
        }

        //grass check
        Vector3 pos = transform.position;
        pos.y -= 1;
        Vector3 sizes = new Vector3(2, 1, 2);
        Collider[] cols = Physics.OverlapBox(pos, sizes, Quaternion.Euler(Vector3.zero), raycastMask);
        OnGrass = false;
        for (int i = 0; i < cols.Length; i++)
        {
            if(cols[i].tag == "Grass")
            {
                //OnGrass = true;
            }
            else
            {
                OnGrass = false;    
            }
        }

        GetComponent<Collider>().enabled = true;

        if (!TouchingGround) { AirTime += Time.deltaTime; }


    }

    void Boosts()
    {
        if (CarBoostInput > 0 && BoostAmount > 0)
        {
            BoostAmount -= Time.deltaTime*customisedBoostUsage;
            boosting = true;
        }
        else
        {
            boosting = false;
        }

        #region Boost Particles
        for (int boo = 0; boo < boosters.Count; boo++)
        {
            int ps = boosters[boo].transform.Find("boostParticles").childCount;
            for (int i = 0; i < ps; i++)
            {
                ParticleSystem part = boosters[boo].transform.Find("boostParticles").GetChild(i).GetComponent<ParticleSystem>();
                if (part != null)
                {
                    if (boosting)
                    {
                        if (!part.isPlaying)
                        {
                            part.Play();
                        }
                    }
                    else { part.Stop(); }
                }
            }
        }
        
        #endregion
    }

    public void IncreaseExtraBoost(float amount)
    {
        ExtraBoost += amount;    
    
    }

    IEnumerator RespawnKart(int AIcurrentPoint)
   {
        CanMove = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<Collider>().enabled = false;
        GameObject hologram = Instantiate(tm.hologramPrefab);
        hologram.transform.position = transform.position;
        hologram.transform.localScale = new Vector3(7, 7, 7);
        yield return new WaitForSeconds(1);
        tm.StartCoroutine(tm.RespawnKart(KartID, currentPoint));
        Destroy(gameObject);
        yield return false;
   }
    
    public IEnumerator DestroyKart()
    {
        Vector3 centre = transform.position;
        Transform[] children = transform.Find("main").GetComponentsInChildren<Transform>();

        GameObject explosion = Instantiate(tm.ExplosionPrefab);
        explosion.transform.position = transform.position;
        explosion.transform.localScale = Vector3.one;

        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].tag == "buildingBlock")
            {
                Vector3 distance = children[i].position - centre;
                distance.Normalize();
                Rigidbody newrb = children[i].gameObject.AddComponent<Rigidbody>();
                newrb.velocity = distance * 25f;
            }
        }
        rb.constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<Collider>().enabled = false;
        CanMove = false;
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(RespawnKart(currentPoint));
        
    }
    #endregion

    #region Inputs
    void Inputs()
    {
        driftButtonPressedThisFrame = false;
        if (!driftButtonPressedThisFrame && driftButtonUp)
        {
            if (CarDriftInput > 0) { driftButtonPressedThisFrame = true; driftButtonUp = false; }
        }
        if(CarDriftInput == 0)
        {
            driftButtonUp = true;
        }


        PowerUpButtonPressedThisFrame = false;
        if (!PowerUpButtonPressedThisFrame && PowerUpButtonUp)
        {
            if (CarPowerUpUse > 0) { PowerUpButtonPressedThisFrame = true; PowerUpButtonUp = false; }
        }
        if (CarPowerUpUse == 0)
        {
            PowerUpButtonUp = true;
        }

    }

    void GetPlayerInputs()
    {
        CarTurnInput = im.CarTurn;
        CarPivotInput = im.CarPivot;
        CarAccelerateInput = im.CarAccelerate;
        CarDriftInput = im.CarDrift;
        CarBoostInput = im.CarBoost;
        CarPowerUpUse = im.CarPowerupUse;
    }
    #endregion

    #region Visual

    void Visual()
    {
        float fovInc = Mathf.Clamp(CurrentSpeed / carMaxSpeed * FieldOfViewScale, 1, 2);
        GetComponent<CameraFollow>().cam.fieldOfView = Mathf.Lerp(GetComponent<CameraFollow>().cam.fieldOfView, boosting ? FieldOfView * fovInc : FieldOfView, 2.5f * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position - new Vector3(0, floorCheck, 0));
        Gizmos.color = Color.red;

        Gizmos.color = Color.white;
        if (AIKart) { Gizmos.DrawLine(transform.position, AI_positionHeadingTo); }
    }

    void Particles()
    {
        if (RealSpeed > carMaxSpeed && !AIKart) { if (!windParticles.GetComponent<ParticleSystem>().isPlaying) windParticles.GetComponent<ParticleSystem>().Play(); } else { windParticles.GetComponent<ParticleSystem>().Stop(); };

        if (OnGrass)
        {
            for (int i = 0; i < staticTyres.Count; i++)
            {
                ParticleSystem[] dirts = staticTyres[i].transform.Find("dirtParticles").GetComponentsInChildren<ParticleSystem>();
                for (int d = 0; d < dirts.Length; d++)
                {

                    if (!dirts[d].isPlaying && OnGrass && RealSpeed>10) { dirts[d].Play(); }

                }
            }

            for (int i = 0; i < staticTyres.Count; i++)
            {
                ParticleSystem[] dirts = staticTyres[i].transform.Find("dirtParticles").GetComponentsInChildren<ParticleSystem>();
                for (int d = 0; d < dirts.Length; d++)
                {

                    if (!dirts[d].isPlaying && OnGrass && RealSpeed > 10) { dirts[d].Play(); }

                }
            }
        }

      
    }

    #endregion

    #region UI

    [Header("UI")]
    bool UIShown;
    float uiSizeTransition;
    float TimeSinceBoosting;

    [SerializeField] GameObject BoostBar;
    [SerializeField] GameObject BoostBackground;
    [SerializeField] Animator BoostAnimator;
    [SerializeField] Color BoostBarColour;
    float LastBoostAmount = 0;
    float updatingTime;

    void UpdateUI()
    {
        Transform UIParent = transform.Find("KartUI").Find("RaceUI");
        UIParent.gameObject.SetActive(tm ? tm.RaceReady : false);
        if (tm)
        {
            if (tm) { if (!tm.RaceReady) { return; } }

            if (!UIShown)
            {
                uiSizeTransition = 0.1f;
                UIShown = true;
            }


            if (uiSizeTransition < 1) { uiSizeTransition += Time.deltaTime * 2; UIParent.transform.localScale = new Vector3(uiSizeTransition, uiSizeTransition, uiSizeTransition); }

           
            #region Boost
            if (BoostAmount != LastBoostAmount || boosting)
            {
                TimeSinceBoosting = 0;
                LastBoostAmount = BoostAmount;
                updatingTime = 0.1f;
            }
            else
            {
                TimeSinceBoosting += Time.deltaTime;
            }

            float boostPercentage = BoostAmount / carBoostCapacity;
            Vector2 boostBackWidthSizeDelta = BoostBackground.GetComponent<RectTransform>().sizeDelta;
            BoostBar.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(BoostBar.GetComponent<RectTransform>().sizeDelta, new Vector2(boostBackWidthSizeDelta.x * boostPercentage, boostBackWidthSizeDelta.y), 3*Time.deltaTime);

            BoostAnimator.SetBool("Boosting", boosting);

            if (updatingTime>0)
            {
                BoostBar.GetComponent<Image>().color = Color.Lerp(BoostBarColour, new Color(0, 0, 0, 0), 1-(updatingTime*10));
                BoostBackground.GetComponent<Image>().color = Color.Lerp(new Color(0, 0, 0, 0.1f), new Color(0, 0, 0, 0), 1-(updatingTime*10));
                updatingTime -= Time.deltaTime;
            }
            else
            {
                if (TimeSinceBoosting > .25f)
                {
                    float t = (TimeSinceBoosting - .25f)*2;
                    BoostBar.GetComponent<Image>().color = Color.Lerp(BoostBarColour, new Color(0, 0, 0, 0), t);
                    BoostBackground.GetComponent<Image>().color = Color.Lerp(new Color(0, 0, 0, 0.1f), new Color(0, 0, 0, 0), t);
                }
            }

            //float bmlerp = UIParent.transform.Find("boostmeter").GetComponentInChildren<Slider>().value;
            //UIParent.transform.Find("boostmeter").GetComponentInChildren<Slider>().value = Mathf.Lerp(bmlerp, BoostAmount / carBoostCapacity, 2 * Time.deltaTime);

            #endregion

            Text firstLapText = UIParent.transform.Find("FirstLapTime").GetComponent<Text>();
            Text secondLapText = UIParent.transform.Find("SecondLapTime").GetComponent<Text>();
            Text thirdLapText = UIParent.transform.Find("ThirdLapTime").GetComponent<Text>();
            Text timeText = UIParent.transform.Find("TimeText").GetComponent<Text>();
            Text lapText = UIParent.transform.Find("LapText").GetComponent<Text>();
            Text positionText = UIParent.transform.Find("PositionText").GetComponent<Text>();

            PlayerCar pc = tm.carsInLobby[KartID];
            firstLapText.gameObject.SetActive(pc.CurrentLap >= 0);
            firstLapText.text = "---" + Format.FormatTime(pc.LapTime[0]);

            secondLapText.gameObject.SetActive(pc.CurrentLap >= 1);
            secondLapText.text = "---" + Format.FormatTime(pc.LapTime[1]);

            thirdLapText.gameObject.SetActive(pc.CurrentLap >= 2);
            thirdLapText.text = "---" + Format.FormatTime(pc.LapTime[2]);

            timeText.text = Format.FormatTime(pc.LapTime[0] + pc.LapTime[1] + pc.LapTime[2]);


            lapText.text = LapOn + "/3";
            positionText.text = Format.FormatPosition(pc.Place);
        }
    }
    #endregion

    #region Audio

    public AudioClip LandSoundEffect;
    public AudioClip BoostIncreaseSoundEffect;

    public float EnginePitchMin;
    public float EnginePitchMax;
    public float EnginePitchScale;

    private void Audio()
    {
        AudioSource[] sources = GetComponentsInChildren<AudioSource>();
        for (int i = 0; i < sources.Length; i++)
        {
            if (!sources[i].GetComponent<KartController>())
            sources[i].pitch = Mathf.Clamp(Mathf.Lerp(sources[i].pitch, (RealSpeed*EnginePitchScale / carMaxSpeed), Time.deltaTime), EnginePitchMin, EnginePitchMax);

            if (!sources[i].isPlaying) { sources[i].Play(); }
            if (!IsPlayerKart) sources[i].volume = 0.1f;
        }
    }

    void PlayLandSound()
    {
        Debug.Log("Played land sound");
        GetComponent<AudioSource>().PlayOneShot(LandSoundEffect, .75f);
    }
    #endregion

    #region Power Ups
    [Header("Power Ups")]
    public int CurrentPowerUp;
    public int PowerUpAmount;

    [SerializeField] bool PowerUpButtonPressedThisFrame;
    bool PowerUpButtonUp;

    void PowerUp()
    {
        if(PowerUpButtonPressedThisFrame && HasPowerUp())
        {
            UsePowerUp();
        }
    }

    public void AddPowerUp(int PowerUp, int Amount)
    {
        if (!HasPowerUp())
        {
            CurrentPowerUp = PowerUp;
            PowerUpAmount = Amount;
            Debug.Log("Power up added " + PowerUpAmount);
        }
    }

    public bool HasPowerUp()
    {
        return PowerUpAmount > 0;
    }

    public void UsePowerUp()
    {
        Debug.Log("Attempted Power Up Use");
        if (PowerUpAmount <= 0) { return; }
        if(CurrentPowerUp >= GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Powerups.Count) { Debug.LogError("Power Up Does Not Exist!"); return; }
        PowerUp power = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Powerups[CurrentPowerUp];
        if (power.PowerUpObject == null) { Debug.LogError("Power Up " + power.PowerUpName + "has no object attached!"); return; }


        GameObject powerObject = Instantiate(power.PowerUpObject);
        powerObject.GetComponent<PowerUpSettings>().PowerUpOwner = KartID;
        powerObject.transform.position = transform.position;
        powerObject.transform.eulerAngles = transform.eulerAngles;
        powerObject.transform.localScale = Vector3.one;

        switch (power.powerUpType)
        {
            case GameManager.PowerUpType.TargettingSingle:

                //get target
                //

                break;

            case GameManager.PowerUpType.TargettingFirst:

                int firstplace=0;
                int secondplace=0;
                bool secondAttack = false;

                for (int i = 0; i < tm.carsInLobby.Count; i++)
                {
                    if (tm.carsInLobby[i].Place == 1) {
                        if (i == KartID) { secondAttack = true; }
                        firstplace = i;
                    
                    }
                    if (tm.carsInLobby[i].Place == 2) 
                    { secondplace = i; 
                    }
                }
                if (secondAttack) { 

                    powerObject.GetComponent<PowerUpSettings>().PowerUpTargetting.Add(secondplace); 

                } else
                {

                    powerObject.GetComponent<PowerUpSettings>().PowerUpTargetting.Add(firstplace);

                }

                break;

            case GameManager.PowerUpType.TargettingAll:
                for (int i = 0; i < tm.carsInLobby.Count; i++)
                {
                    if (i != KartID) { powerObject.GetComponent<PowerUpSettings>().PowerUpTargetting.Add(i); }
                }
                break;
        }
        PowerUpAmount--;

    }



    #endregion

    #region AI

    [SerializeField] float AI_TimeStill=0;
    float AI_ReversingTime = 0;
    Vector3 AI_positionHeadingTo;
    float AI_tpassed;
    Vector3 AI_positionOneSecondAgo = Vector3.zero;

    public List<Vector3> AI_positionsGoing;

    float AI_DriftTime = 0;

    float dtel = 0;

    public List<Vector3> CurrentAIRoute = new List<Vector3>(0);
    public int currentPoint = 0;
    public bool pointSet;
    float TimeSpentOnCurrentCheck = 0;
    bool respawned=false;


    void AI()
    {
        if (tm.RaceStarted)
        {
            AI_Move();
            AI_Turn();
            AI_Drift();
            AI_CheckReached();
            AI_Boosting();
            AI_PowerUp();
        }
    }

    float RandomTurn = 0;
    void AI_Move()
    {
        if (AI_ReversingTime > 0)
        {
            CarAccelerateInput = -1;
            AI_ReversingTime -= Time.deltaTime;
        }
        else
        {
            CarAccelerateInput = 1;
        }

        AI_tpassed += Time.deltaTime;
        AI_TimeStill += Time.deltaTime;
        if (AI_tpassed > 1)
        {
            AI_tpassed = 0;
            float distanceToLastPos = Vector3.Distance(transform.position, AI_positionOneSecondAgo);
            if (distanceToLastPos >= 7)
            {
                AI_TimeStill = 0;
            }

            if (AI_TimeStill >= 2)
            {
                AI_ReversingTime = 3;
                RandomTurn = Random.Range(0, 2) == 0 ? 1 : -1;
            }
            
            if(AI_TimeStill >= 5 && !respawned)
            {
                respawned = true;
                StartCoroutine(RespawnKart(currentPoint));
            }

            AI_positionOneSecondAgo = transform.position;
        }

    }

    void AI_Turn()
    {
        Vector3 tP = transform.position;
        tP.y = 0;
        Vector3 bP = AI_positionHeadingTo;
        bP.y = 0;
        Vector3 targetdir = tP - bP;


        float angleDiff = -Vector3.SignedAngle(targetdir, -transform.forward, Vector3.up);

        if (angleDiff > 40f)
        {
            AI_DriftTime = .5f;
            CarTurnInput = 1;
        }
        else if (angleDiff > 30f)
        {
            AI_DriftTime = .5f;
            CarTurnInput = isSliding ? .75f : 1f;
        }
        else if (angleDiff > 2.5f)
        {
            CarTurnInput = isSliding ? .5f : 1f;
        }
        else if (angleDiff < -40f)
        {
            AI_DriftTime = .5f;
            CarTurnInput = -1;
        }
        else if (angleDiff < -30f)
        {
            AI_DriftTime = .5f;
            CarTurnInput = isSliding ? -.75f : -1f;
        }
        else if (angleDiff < -2.5f)
        {
            CarTurnInput = isSliding ? -.5f : -1f;
        }
        else
        {
            CarTurnInput = 0;
        }

        if (AI_ReversingTime > 0)
        {
            CarTurnInput = RandomTurn;
        }
    }

    float dtgo = 0;
    void AI_Drift()
    {
        if (AI_DriftTime <= 0){ CarDriftInput = 0; dtel = 0; return; }

        AI_DriftTime -= Time.deltaTime;
        dtgo -= Time.deltaTime;

        if (dtgo <= 0)
        {
            CarDriftInput = 1;
            if (!isSliding)
            {
                dtel += Time.deltaTime;
            }
            else { dtel = 0; }
        }
        else
        {
        CarDriftInput = 0;

        }
        if(dtel > .25f) dtgo = .1f;

    }

    void AI_CheckReached()
    {
        TimeSpentOnCurrentCheck += Time.deltaTime;
        if (currentPoint >= CurrentAIRoute.Count)
        {
            int newRoute = tm.UpdateAIRoute();
            CurrentAIRoute = tm.AIRoutes[newRoute].positions;
            currentPoint = 0;
            TimeSpentOnCurrentCheck = 0;
        }

        AI_positionHeadingTo = CurrentAIRoute[currentPoint];
        if(Vector3.Distance(transform.position, AI_positionHeadingTo) < 20) { currentPoint++; }

        if(TimeSpentOnCurrentCheck > 5)
        {
            carSteerSpeed *= 2;
        }

    }

    void AI_Boosting()
    {
       CarBoostInput = BoostAmount > 0 ? 1 : 0;
    }

    float AI_PowerUpCooldown;
    float AIcoolpassed;
    void AI_PowerUp()
    {
        AIcoolpassed += Time.deltaTime;
        if(AIcoolpassed>AI_PowerUpCooldown && HasPowerUp())
        {
            UsePowerUp();
            AI_PowerUpCooldown = Random.Range(5f, 15f);
        }

    }
    #endregion

}
