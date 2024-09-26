using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.EventSystems.EventTrigger;

public class KartController3 : MonoBehaviour
{
#region Variables----------------------------------------------------------
    InputManager im;
    BlockStorage bs;
    Player player;
    GameManager gm;
    RaceManager rm;
    AIManager aiM;
    RealWorldMainMenu rrm;
    DisplayCharacter dispCharacter;
    GameUIManager gum;

    [Header("Kart Storage------------------------------------------------------------------------------------------------------------------------------------------------------------------------")]
    public bool Player;
    public bool IsPlayerViewKart = true;
    public Rigidbody sphereRB;
    public List<GameObject> FrontTyres = new List<GameObject>();
    public List<GameObject> BackTyres = new List<GameObject>();
    public List<GameObject> AllTyres = new List<GameObject>();
    public List<GameObject> Boosters = new List<GameObject>();
    List<GameObject> BoosterForwards = new List<GameObject>();

    [Header("Kart Stats------------------------------------------------------------------------------------------------------------------------------------------------------------------------")]
    public float Acceleration;
    public float Speed;
    public float Steering;
    public float Boost;
    public float Weight;

    public float AI_Render_Distance = 60f;


    [Header("External Forces------------------------------------------------------------------------------------------------------------------------------------------------------------------------")]
    public float GravityForce = 25f;
    public float DragOnGround = 3;
    public float DragInAir = 0.5f;
    
    

    [Header("Kart Info------------------------------------------------------------------------------------------------------------------------------------------------------------------------")]
    public bool CanSuperJump;
    public bool TouchingGround;
    public float RealSpeed = 0; //The real speed of the Kart

    [Header("Ground Checks------------------------------------------------------------------------------------------------------------------------------------------------------------------------")]
    public LayerMask GroundLayerMask;

    

    #region Initial Conditions Variables
        float OutwardsDriftForce = 7500; //Outwards force applied whilst drifting
        float SuperBoostForce = 15; //Force of the Super Jump
        float groundRayLength = 1;
        float DriftThreshold = 20; //The speed needed to start and maintain drifting

        float Yoffset = -0.7f; //The offset for when the kart is on a ramp
        float cameraFOV = 60;
    #endregion

    #region Kart Information Variables
        float TimeCounterSteering = 0; //The time spent countersteering
        float timeSinceLastSuperJump = 0; //The time since the last super jump
        float landedTime = 0; //The time the kart has been on the ground

        bool FrontTouching = false; //The raycast for the front touch has hit.
        bool BackTouching = false;  //The raycast for the back touch has hit.
        bool RightTouching = false; //The raycast for the right touch has hit.
        bool LeftTouching = false; //The raycast for the left touch has hit.

        float rampAngle = 0; //Current angle of the ramp the kart is on in degrees
        float AirTime = 0; //Current airtime of the kart 
        Vector3 lastNextRot; //The last rotation before leaving a ramp
        float timeSinceLastSuperBoost = 0f; //The time since the last super boost
    #endregion

    #region Movement Variables
        bool driftRight = false;
        bool driftLeft = false;

        float driftTime = 0;
        float steerDirection = 0;

        float SuperBoostTime;

        [SerializeField] float BoostStorage = 0; //The amount of boost the kart has
        float MaxBoostStorage = 10;

        float minimumTurnAmount = .5f;
    #endregion

    #region Input Variables
        bool driftButtonPressedThisFrame;
        bool driftButtonUp;

        bool PowerUpButtonPressedThisFrame;
        bool PowerUpButtonUp;

        [SerializeField] float InputY;
        float InputX;

        float CarTurnInput;
        float CarPivotInput;
        [SerializeField] float CarAccelerateInput;
        float CarDriftInput;
        float CarBoostInput;
        float CarPowerUpUse;
    #endregion

    #region Visual Variables
        float CameraStartFOV = 60;

        public float TyreTurn = 40;
        float BodyDriftTurn = 35;
        float current = 0;
    #endregion

    #endregion

#region Unity Functions---------------------------------------------------------
    void Start()
    {
        StartUpLocal();
    }

    void Update()
    {
        transform.position = sphereRB.transform.position + transform.right*positionOffset.x + transform.up * positionOffset.y + transform.forward * positionOffset.z;

        if (Player)
        {
            GetPlayerInputs();
        }
        else
        {
            AI();
        }

        Inputs();
        DriftVisuals();
        if (rm) if (rm.RaceLoadingStage < 6) return;
        Turn();
        Drift();

        Debug.DrawLine(groundRayFrom.position, groundRayFrom.position - groundRayFrom.transform.up * groundRayLength);

        timeSinceLastSuperBoost += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        //if (!Player) return;
        MoveCamera();
        UpdateUI();

        BoostVisual();
        TyresVisual();
        EngineSound();
        if (rm) if (rm.RaceLoadingStage < 6) return;
        CalculatePhysics();
        CheckTouchingGround();

        
    }
    #endregion

#region Inputs---------------------------------------------------------

    float TimeAcceleratingBeforeRace = 0f;

    void Inputs()
    {
        if (rm)
        {
            if(InputY > 0.25f && !rm.RaceStarted)
            {
                TimeAcceleratingBeforeRace += Time.deltaTime;
            }
            if (InputY <= 0)
            {
                TimeAcceleratingBeforeRace = 0;
            }
            if(rm.RaceStarted && TimeAcceleratingBeforeRace != 0)
            {
                float StartBoostAmount = 0;
                if (TimeAcceleratingBeforeRace > 2) StartBoostAmount = 0;
                else if (TimeAcceleratingBeforeRace > 1.25f) StartBoostAmount = 2;
                else if (TimeAcceleratingBeforeRace > .75f) StartBoostAmount = 0.5f;

                AddBoostAmount(StartBoostAmount);
                Debug.Log(TimeAcceleratingBeforeRace);
                TimeAcceleratingBeforeRace = 0;
            }
        }

        driftButtonPressedThisFrame = false;
        if (!driftButtonPressedThisFrame && driftButtonUp)
        {
            if (CarDriftInput > 0) { driftButtonPressedThisFrame = true; driftButtonUp = false; }
        }
        if (CarDriftInput == 0)
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
        if (gum.PauseMenuOpen)
        {
            CarTurnInput = 0;
            CarPivotInput = 0;
            CarAccelerateInput = 0;
            CarDriftInput = 0;
            CarBoostInput = 0;
            CarPowerUpUse = 0;

            InputY = 0;
            InputX = 0;
        }
        else
        {
            CarTurnInput = im.CarTurn;
            CarPivotInput = im.CarPivot;
            CarAccelerateInput = im.CarAccelerate;
            CarDriftInput = im.CarDrift;
            CarBoostInput = im.CarBoost;
            CarPowerUpUse = im.CarPowerupUse;

            InputY = CarAccelerateInput;
            InputX = CarTurnInput;
        }
    }
    #endregion

#region StartUp----------------------------------------------------

    [Header("STARTUP---------------------------------------------------------------------------------")]
    public int aiID = 0;
    public GameObject AssosciatedCollider;
    public bool LoadedIn;

    void StartUpLocal()
    {
        sphereRB.transform.parent = null;
        AssosciatedCollider = sphereRB.gameObject;
        CameraStartFOV = Camera.main.fieldOfView;
        im = (FindObjectOfType<InputManager>() != null) ? FindObjectOfType<InputManager>() : null;
        gm = (FindObjectOfType<GameManager>() != null) ? FindObjectOfType<GameManager>() : null;
        bs = (FindObjectOfType<BlockStorage>() != null) ? FindObjectOfType<BlockStorage>() : null;
        player = (FindObjectOfType<Player>() != null) ? FindObjectOfType<Player>() : null;
        rm = (FindObjectOfType<RaceManager>() != null) ? FindObjectOfType<RaceManager>() : null;
        aiM = (FindObjectOfType<AIManager>() != null) ? FindObjectOfType<AIManager>() : null;
        rrm = (FindObjectOfType<RealWorldMainMenu>() != null) ? FindObjectOfType<RealWorldMainMenu>() : null;
        gum = (FindObjectOfType<GameUIManager>() != null) ? FindObjectOfType<GameUIManager>() : null;
        if (LoadedIn) StartUpPublic();
    }
    public void StartUpPublic()
    {
        Debug.Log("Kart Startup");
        if (bs && player) BuildKart();

        if (Boosters.Count > 0)
        {
            for (int i = 0; i < Boosters.Count; i++)
            {
                GameObject boosterForward = new GameObject("BoosterForward" + i);
                boosterForward.transform.SetParent(transform);
                boosterForward.transform.position = Boosters[i].transform.position;
                boosterForward.transform.forward = Boosters[i].transform.forward;
                BoosterForwards.Add(boosterForward);
            }
        }

        StartupSFX();
    }

    public List<GameObject> blocksCreated = new List<GameObject>();
    GameObject SpawnedCharacter;
    public GameObject CharacterDisplay;
    
    public bool KartBuilt;
    public void BuildKart()
    {
        Car thisCar = null;
        Debug.Log("Kart Built");
        if (Player) thisCar = player.playersKart[player.KartSelected];
        if (!Player && aiM && aiM.AICharacters[aiID].charactersCar != null) thisCar = aiM.AICharacters[aiID].charactersCar;
        if(thisCar == null) { Debug.LogError("No Kart Found!"); return; }

        foreach (Transform child in transform.Find("main").GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);
        }

        Transform kartMain = transform.Find("main");

        Vector3 blockOffset = Vector3.zero;

        AllTyres.Clear();
        FrontTyres.Clear();
        BackTyres.Clear();
        Boosters.Clear();
        blocksCreated.Clear();

        float HighestZ = 0;
        float LowestZ = 0;
        float HighestY = 0;
        float LowestY = 0;

        for (int i = 0; i < thisCar.blocks.Count; i++)
        {
            if (thisCar.positions[i].z > HighestZ) HighestZ = thisCar.positions[i].z;
            if (thisCar.positions[i].z < LowestZ) LowestZ = thisCar.positions[i].z;
            if (thisCar.positions[i].y > HighestY) HighestY = thisCar.positions[i].y;
            if (thisCar.positions[i].y < LowestY) LowestY = thisCar.positions[i].y;

            BuildingBlock bl = bs.blocks[thisCar.blocks[i]];
            GameObject block = Instantiate(bl.buildingPrefab, kartMain);
            block.transform.localScale = new Vector3(bl.SizeX, bl.SizeY, bl.SizeZ);
            block.transform.localPosition = thisCar.positions[i];
            block.transform.localRotation = Quaternion.Euler(thisCar.rotations[i]);
            blocksCreated.Add(block);

            if (bl.canBeColoured)
            {
                MeshRenderer[] meshes = block.GetComponentsInChildren<MeshRenderer>();
                for (int mesh = 0; mesh < meshes.Length; mesh++) { if (meshes[mesh].transform.parent.name == "paintable") meshes[mesh].material = bs.materials[thisCar.materials[i]].LinkedMaterial; }
            }


            if(bl.blockType == GameManager.BlockType.Tyre)
            {
                Debug.Log("Tyre Added");
                blockOffset += thisCar.positions[i];
                block.transform.localEulerAngles = Vector3.zero;
                AllTyres.Add(block);

                if (thisCar.positions[i].z > 0)
                {
                    FrontTyres.Add(block);
                }
                if (thisCar.positions[i].z < 0)
                {
                    BackTyres.Add(block);
                }
                if (thisCar.positions[i].x > 0)
                {
                    block.transform.Find("tyre").Find("driftParticles").localScale = new Vector3(1, 1, 1);
                }
                if (thisCar.positions[i].x < 0)
                {
                    block.transform.Find("tyre").Find("driftParticles").localScale = new Vector3(-1, 1, 1);
                }
            }

            if (bl.blockType == GameManager.BlockType.Booster)
            {
                Boosters.Add(block);
                ParticleSystem[] parts = block.transform.Find("boostParticles").GetComponentsInChildren<ParticleSystem>();
                for (int x = 0; x < parts.Length; x++)
                {
                    BoostParticles.Add(parts[x]);
                }
            }

            if(bl.blockType == GameManager.BlockType.Engine)
            {
                EngineAudio = bl.BlockAudio;
            }

            block.transform.name = i.ToString();
            block.layer = LayerMask.NameToLayer("Kart");
        }

        Vector3 AverageWheelLocation = Vector3.zero;
        for (int i = 0; i < AllTyres.Count; i++)
        {
            AllTyres[i].GetComponent<TyreConnectionCheck>().CheckConnections();
            AverageWheelLocation += AllTyres[i].transform.localPosition;
        }
        AverageWheelLocation /= AllTyres.Count;

        for (int i = 0; i < blocksCreated.Count; i++)
        {
            foreach (Collider col in blocksCreated[i].GetComponentsInChildren<Collider>())
            {
                Destroy(col);
            }
        }

        for (int i = 0; i < BackTyres.Count; i++)
        {
            StageOneDriftParticles.Add(BackTyres[i].transform.Find("tyre").Find("driftParticles").Find("StageOne").GetComponent<ParticleSystem>());
            StageTwoDriftParticles.Add(BackTyres[i].transform.Find("tyre").Find("driftParticles").Find("StageTwo").GetComponent<ParticleSystem>());
            StageThreeDriftParticles.Add(BackTyres[i].transform.Find("tyre").Find("driftParticles").Find("StageThree").GetComponent<ParticleSystem>());
        }

        blockOffset = -AverageWheelLocation;
        blockOffset.y = -1.5f - LowestY;
        positionOffset = blockOffset;

        groundRayFrom.transform.localPosition = AverageWheelLocation;

        #region Seat
        GameObject seat = Instantiate(bs.seats[0], kartMain);
        seat.transform.localPosition = Vector3.zero;
        seat.transform.localScale = Vector3.one;
        MeshRenderer[] seatPaintables = seat.transform.Find("paintable").GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < seatPaintables.Length; i++)
        {
            seatPaintables[i].material = bs.materials[thisCar.seatColour].LinkedMaterial;
        }
        #endregion

        KartBuilt = true;
        GetStats();

        #region Character
        if (SpawnedCharacter) Destroy(SpawnedCharacter);
        
        SpawnedCharacter = Instantiate(CharacterDisplay);
        Transform charPosition = seat.transform.Find("driverPosition");
        
        SpawnedCharacter.transform.SetParent(charPosition);
        SpawnedCharacter.transform.localScale = Vector3.one;
        SpawnedCharacter.transform.localRotation = Quaternion.identity;
        SpawnedCharacter.transform.localPosition = Vector3.zero;
        
        dispCharacter = SpawnedCharacter.GetComponent<DisplayCharacter>();
        if (Player)
        {
            dispCharacter.BuildPlayerCharacter = true;
        }
        else
        {
            dispCharacter.BuildAIcharacter = true;
            dispCharacter.AIid = aiID;
        }
        dispCharacter.AnimationPlaying = 1;
        dispCharacter.Build = true;
        dispCharacter.GetStartStats();

       
        #endregion
    }

    void GetStats()
    {
        Car thisCar = null;
        Debug.Log("Kart Built");
        if (Player) thisCar = player.playersKart[player.KartSelected];
        if (!Player && aiM && aiM.AICharacters[aiID].charactersCar != null) thisCar = aiM.AICharacters[aiID].charactersCar;

        if(rm != null && gm.SoloRace_KartType == 1)
        {
            if (!Player) Steering *= 2;
            return;
        }


        float weightTemp = 0;

        float speedTemp = 0;
        float accelTemp = 0;
        int engines = 0;

        float steerTemp = 0;
        float boostTemp = 0;
        float boostcapacityTemp = 0;

        for (int i = 0; i < thisCar.blocks.Count; i++)
        {
            BuildingBlock ThisBlock = bs.blocks[thisCar.blocks[i]];
            if (ThisBlock == null) continue;

            weightTemp += ThisBlock.Weight;

            speedTemp += ThisBlock.MaxSpeed;
            accelTemp += ThisBlock.Acceleration;
            if (ThisBlock.blockType == GameManager.BlockType.Engine) engines++;

            steerTemp += ThisBlock.SteerSpeed;
            boostTemp += ThisBlock.BoostMaxSpeed;
            boostcapacityTemp += ThisBlock.BoostCapacity;
        }

        boostTemp /= Boosters.Count;
        steerTemp /= AllTyres.Count;

        speedTemp /= engines;
        accelTemp /= engines;

        weightTemp = Mathf.Clamp(weightTemp, 50, 250);
        weightTemp = weightTemp * (1 - (weightTemp / 500f) );

        Weight = weightTemp;

        Speed = speedTemp;
        Acceleration = accelTemp;

        Boost = boostTemp;
        MaxBoostStorage = boostcapacityTemp;

        Steering = rrm != null ? steerTemp * 1.5f : Player ? steerTemp : steerTemp * 2.5f; ;

        sphereRB.mass = weightTemp;

    }


#endregion

#region Turning and Drifting---------------------------------------------------------
    [Header("Drifting---------------------------------------------------------------------------------------")]
    public bool IsSliding = false;

    public float DriftAmount = 1.5f;
    public float DriftTurnNeed = .25f;
    [Range(-1f, 1f)] public float MinimumOutwardForce = 0.25f;

    void Turn()
    {
        steerDirection = Mathf.Lerp(steerDirection, CarTurnInput, 8 * Time.deltaTime);

        if (driftRight)
        {
            steerDirection = Mathf.Clamp(steerDirection, -MinimumOutwardForce, Mathf.Infinity);
        }
        else if (driftLeft)
        {
            steerDirection = Mathf.Clamp(steerDirection, -Mathf.Infinity, MinimumOutwardForce);
        }

        float driftscale = IsSliding ? Mathf.Clamp(Mathf.Abs(CarTurnInput), 0.8f, 1f) * DriftAmount : 1;

        float scale = Mathf.Abs(InputY) > 0 ? InputY / Mathf.Abs(InputY) : 1f;

        float speedScale = Mathf.Clamp( Mathf.Abs(RealSpeed / 40), minimumTurnAmount, 1);

        transform.Rotate(transform.up * steerDirection * Steering * Time.deltaTime * 10f * scale * driftscale * speedScale);

        //if(DisplayCharacter) DisplayCharacter.AnimatorSetBool("TurningRight", steerDirection > 0.25f);
        //if(DisplayCharacter)  DisplayCharacter.AnimatorSetBool("TurningLeft", steerDirection < -0.25f);

        Vector3 rot = transform.localEulerAngles;
        rot.z = 0;
        transform.localEulerAngles = rot;
    }

    public float CounterSteerTimeAllow;

    [SerializeField] int BoostLevel = 0;
    void Drift()
    {
        timeSinceLastSuperJump += Time.deltaTime;
        if(driftButtonPressedThisFrame && CanSuperJump && timeSinceLastSuperJump > 0.8f && AirTime < 0.5f) //Super Jump
        {
            timeSinceLastSuperJump = 0f;
            sphereRB.AddForce(transform.up * Mathf.Abs(RealSpeed/60) * 1500, ForceMode.Impulse);
            Debug.Log("Super Jumped");
        }

        if(driftButtonPressedThisFrame && TouchingGround && landedTime > 0.1f && Mathf.Abs(InputX) >= DriftTurnNeed && !IsSliding && RealSpeed >= DriftThreshold)     //start drifting
        {
            if (InputX > 0)
            {
                driftRight = true;
                driftLeft = false;
            }
            if (InputX < 0)
            {
                driftLeft = true;
                driftRight = false;
            }
            IsSliding = driftRight || driftLeft;
            BoostLevel = 0;
        }

        if (CarDriftInput > 0 && TouchingGround && landedTime < 0.1f && RealSpeed > DriftThreshold &&!IsSliding) //drift from landing
        {
            Debug.Log("Started Drifting on Land");
            PlaySoundEffect(LandAudio, LandAudioVolume);
            driftTime = 0.3f;
            if (steerDirection > DriftTurnNeed)
            {
                driftRight = true;
                driftLeft = false;
            }
            else if (steerDirection < -DriftTurnNeed)
            {
                driftRight = false;
                driftLeft = true;
            }
            IsSliding = driftRight || driftLeft;
            BoostLevel = 0;
        }

        if(IsSliding && CarDriftInput > 0 && RealSpeed >= DriftThreshold)  //keep drifting
        {
            driftTime += Time.deltaTime;
            if(driftTime > 2f)
            {
                BoostLevel = 3;
            }else if(driftTime > 1.2f)
            {
                BoostLevel = 2;
            }
            else if(driftTime > 0.6f)
            {
                BoostLevel = 1;
            }

            if(driftRight && CarTurnInput < DriftTurnNeed)           //Stop counter steering too much
            {
                TimeCounterSteering += Time.deltaTime;
            }
            else if (driftLeft && CarTurnInput > -DriftTurnNeed)
            {
                TimeCounterSteering += Time.deltaTime;
            }
            else
            {
                TimeCounterSteering -= Time.deltaTime;
            }
            TimeCounterSteering = Mathf.Clamp(TimeCounterSteering, 0, CounterSteerTimeAllow + 0.1f);

            if(TimeCounterSteering >= CounterSteerTimeAllow)
            {
                driftLeft = false;
                driftRight = false;
                IsSliding = false;
                driftTime = 0;
                AddDriftBoost(BoostLevel);
                BoostLevel = 0;
            }
        } 
        else if (driftButtonUp)
        {
            //end drifting
            driftLeft = false;
            driftRight = false;
            IsSliding = false;
            driftTime = 0;
            AddDriftBoost(BoostLevel);
            BoostLevel = 0;
        } 

    }


    float StartUpSmokeTime = 0;
    public List<ParticleSystem> StageOneDriftParticles = new List<ParticleSystem>();
    public List<ParticleSystem> StageTwoDriftParticles = new List<ParticleSystem>();
    public List<ParticleSystem> StageThreeDriftParticles = new List<ParticleSystem>();
    void DriftVisuals()
    {
        if(!Player) if (Vector3.Distance(transform.position, InGamePlayersKart.transform.position) > AI_Render_Distance) return;

        if (driftButtonPressedThisFrame && TouchingGround)
        {
            GetComponent<Animator>().SetTrigger("KartHop");
            PlaySoundEffect(JumpAudio, JumpAudioVolume);
            StartCoroutine(PlaySFXAfterDelay(LandAudio, LandAudioVolume, 0.2f));
        }

        for (int i = 0; i < AllTyres.Count; i++)
        {
            float spinSpeed = (RealSpeed / 60f);
            spinSpeed = Mathf.Clamp(spinSpeed, -1f, 1f);
            if (Mathf.Abs(RealSpeed) < 0.5f) spinSpeed = 0;
            AllTyres[i].transform.Find("tyre").Find("tyremain").Find("wheel").Rotate(new Vector3(-spinSpeed * 360 * 8 * Time.deltaTime, 0, 0), Space.Self);

            bool can = Physics.Raycast(AllTyres[i].transform.position, -AllTyres[i].transform.up, out RaycastHit hit, 0.55f, GroundLayerMask);

            bool StartUpSmoke = AverageAccelPs > Speed*3 || AverageAccelPs < -Speed*2;
            if (StartUpSmoke) {
                StartUpSmokeTime += Time.deltaTime;
            }
            else
            {
                StartUpSmokeTime = 0;
            }
            bool StartUpSmokeShow = StartUpSmoke && StartUpSmokeTime < 1f;

            AllTyres[i].transform.Find("tyre").Find("tyremain").Find("DriftParticle").GetComponent<TrailRenderer>().emitting = (StartUpSmokeShow || (IsSliding && driftTime > 0.3f)) && can;
        }

        for (int i = 0; i < StageOneDriftParticles.Count; i++)
        {
            ParticleSystem StageOne = StageOneDriftParticles[i];
            if (BoostLevel < 1) StageOne.Stop();
            if (BoostLevel >= 1 && !StageOne.isPlaying) StageOne.Play();
        }

        for (int i = 0; i < StageTwoDriftParticles.Count; i++)
        {
            ParticleSystem StageTwo = StageTwoDriftParticles[i];
            if (BoostLevel < 2) StageTwo.Stop();
            if (BoostLevel >= 2 && !StageTwo.isPlaying) StageTwo.Play();
        }

        for (int i = 0; i < StageThreeDriftParticles.Count; i++)
        {
            ParticleSystem StageThree = StageThreeDriftParticles[i];
            if (BoostLevel < 3) StageThree.Stop();
            if (BoostLevel >= 3 && !StageThree.isPlaying) StageThree.Play();
        }

    }

    float SpeedBefore = 0;
    float AverageAccelPs = 0;
    float AverageAccelUpdateTime = 0;

    void TyresVisual()
    {
        AverageAccelUpdateTime += Time.fixedDeltaTime;
        if(AverageAccelUpdateTime >= .1f)
        {
            AverageAccelUpdateTime = 0;
            AverageAccelPs = (RealSpeed - SpeedBefore) / .1f;
            SpeedBefore = RealSpeed;
        }

        for (int i = 0; i < FrontTyres.Count; i++)
        {
            FrontTyres[i].transform.Find("tyre").Find("tyremain").localEulerAngles = new Vector3(0, 90 + TyreTurn * steerDirection, 0);
        }
    }
    
    
    #endregion

#region Driving---------------------------------------------------------
    float DriveInput = 0;
    [SerializeField] Transform RampAssist;
    bool IsBoosting;
    bool IsSuperBoosting;
    void CalculatePhysics()
    {

        DriveInput = Mathf.Lerp(DriveInput, InputY, Time.fixedDeltaTime * (Acceleration/18f));
        if (InputY > 0) DriveInput = Mathf.Clamp(DriveInput, 0, 1); else DriveInput = Mathf.Clamp(DriveInput, -1, 0);
        Vector3 ForceApplied = Vector3.zero;
        Vector3 forward = transform.forward;
        if (TouchingGround)
        {
            sphereRB.drag = DragOnGround;

            if (Mathf.Abs(InputY) > 0)
            {
                if (rampAngle > 5) forward = RampAssist.forward;
                //if(RealSpeed < Speed) 
                ForceApplied += forward * DriveInput * (Speed/12f) * 12000f;
                //ForceApplied += transform.up * DriveInput * Mathf.Sin(rampAngle);
            }

            if (IsSliding)
            {
                if (driftRight)
                {
                    float ForceMultiplier = Mathf.Abs(Mathf.Clamp(steerDirection, 0.2f, 1f));
                    ForceApplied -= transform.right * OutwardsDriftForce * ForceMultiplier;
                }

                if (driftLeft)
                {
                    float ForceMultiplier = Mathf.Abs(Mathf.Clamp(steerDirection, -1f, -0.2f));
                    ForceApplied += transform.right * OutwardsDriftForce * ForceMultiplier;
                }
            }
        }
        else
        {
            sphereRB.drag = DragInAir;
            sphereRB.AddForce(-Vector3.up * GravityForce * 100f);
        }

        IsBoosting = CarBoostInput > 0 && SuperBoostTime <= 0 && BoostStorage > 0;
        if (IsBoosting)
        {
            Vector3 boostForward = Vector3.zero;
            for (int i = 0; i < BoosterForwards.Count; i++)
            {
                boostForward += BoosterForwards[i].transform.forward;
            }
            ForceApplied += boostForward * Boost * 1000f;
            BoostStorage -= Time.fixedDeltaTime;
        }

        IsSuperBoosting = SuperBoostTime > 0;
        if (IsSuperBoosting)
        {
            ForceApplied += forward * SuperBoostForce * 1000f;
            SuperBoostTime -= Time.fixedDeltaTime;
        }

        sphereRB.AddForce(ForceApplied);

        RealSpeed = transform.InverseTransformDirection(sphereRB.velocity).z;
    }

    
    #endregion

#region Boost

    void AddLandBoost(float t)
    {
        if (t >= 3)
        {
            AddBoostAmount(3f);
            return;
        }

        if (t >= 2f)
        {
            AddBoostAmount(1.5f);
            return;
        }

        if (t >= 1)
        {
            AddBoostAmount(1f);
            
            return;
        }

    }
    float LandBoostPotential()
    {
        if (AirTime >= 3f)
        {
            return 3f;
        }
        else if (AirTime >= 2f)
        {
            return 1.5f;
        }
        else if (AirTime >= 1f)
        {
            return 1f;
        }
        return 0f;
    }

    void AddDriftBoost(int StageOn)
    {
        if (StageOn == 1) AddBoostAmount(.5f);
        if (StageOn == 2) AddBoostAmount(1.75f);
        if (StageOn == 3) AddBoostAmount(2.8f);
    }

    public void AddBoostAmount(float amount)
    {
        if (IsPlayerViewKart)
        {
            float boostVol = (amount / 3f);
            boostVol = Mathf.Clamp(boostVol, 0.5f, 1f);
            PlaySoundEffect(BoostAddAudio, BoostAddAudioVolume * boostVol);
        }
        BoostStorage += amount;
        BoostStorage = Mathf.Clamp(BoostStorage, 0, MaxBoostStorage);
    }

    public List<ParticleSystem> BoostParticles = new List<ParticleSystem>();
    void BoostVisual()
    {
        if (!Player) if (Vector3.Distance(transform.position, InGamePlayersKart.transform.position) > AI_Render_Distance) return;

        //is boosting
        for (int i = 0; i < BoostParticles.Count; i++)
        {
          if ((IsBoosting || IsSuperBoosting) && !BoostParticles[i].isPlaying) BoostParticles[i].Play();
          if (!IsBoosting && !IsSuperBoosting) BoostParticles[i].Stop();
        }

    }

    #endregion

#region Powerups

    [Header("Powerups")]
    public int CurrentPowerup = -1;

    public void ExplodeKart()
    {
        Transform[] children = transform.Find("main").GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].tag == "buildingBlock")
            {
                Vector3 distance = children[i].position - (transform.position - new Vector3(0, 1, 0));
                distance.y *= 2;
                distance.Normalize();
                Rigidbody newrb = children[i].gameObject.AddComponent<Rigidbody>();
                newrb.AddForce(distance * 2000f);
                newrb.useGravity = true;
                children[i].gameObject.AddComponent<BoxCollider>();

                newrb.mass = 1;
            }
        }
        sphereRB.constraints = RigidbodyConstraints.FreezeAll;
        sphereRB.GetComponent<Collider>().enabled = false;
        RespawnKart();
    }



    public void RespawnKart()
    {

    }

    public void GivePowerup(int Powerup)
    {
        CurrentPowerup = Powerup;
    }

    public void UsePowerup()
    {

    }


#endregion

#region Ground Check---------------------------------------------------------
    [Header("Raycasts------------------------------------------------------------------------------------------------")]
    [SerializeField] Transform groundRayFrom;
    [SerializeField] Transform frontGroundRay;
    [SerializeField] Transform backGroundRay;
    [SerializeField] Transform leftGroundRay;
    [SerializeField] Transform rightGroundRay;

    void CheckTouchingGround()
    {
        Vector3 localEulerAngles = Vector3.zero;
        float driftTurnAmount = 0;
        if (IsSliding)
        {
            if (driftRight)
            {
                driftTurnAmount = Mathf.Clamp(steerDirection, 0.25f, 1);
            }
            else
            {
                driftTurnAmount = Mathf.Clamp(steerDirection, -1, -.25f);
            }

        }
        float yval = transform.Find("main").localEulerAngles.y;
        current = Mathf.Lerp(current, driftTurnAmount * BodyDriftTurn, 5 * Time.deltaTime);
        localEulerAngles += new Vector3(0, current, 0);

        RaycastHit hit;
        if (Physics.Raycast(groundRayFrom.position, -transform.up, out hit, groundRayLength, GroundLayerMask))
        {
            TouchingGround = true;
            landedTime += Time.fixedDeltaTime;
            AddLandBoost(AirTime);
            AirTime = 0;
        }
        else
        {
            TouchingGround = false;
            landedTime = 0;
            AirTime += Time.fixedDeltaTime;
        }

        RaycastHit frontHit;
        RaycastHit backHit;
        RaycastHit leftHit;
        RaycastHit rightHit;

        FrontTouching = Physics.Raycast(frontGroundRay.position, -transform.up, out frontHit, 1 + groundRayLength * 2f, GroundLayerMask);
        BackTouching = Physics.Raycast(backGroundRay.position, -transform.up, out backHit, 1 + groundRayLength * 2f, GroundLayerMask);
        LeftTouching = Physics.Raycast(leftGroundRay.position, -transform.up, out leftHit, 1 + groundRayLength * 2f, GroundLayerMask);
        RightTouching = Physics.Raycast(rightGroundRay.position, -transform.up, out rightHit, 1 + groundRayLength * 2f, GroundLayerMask);

        if (FrontTouching && BackTouching && LeftTouching && RightTouching)
        {
            // Get the hit points
            Vector3 frontHitPoint = frontHit.point;
            Vector3 backHitPoint = backHit.point;
            Vector3 leftHitPoint = leftHit.point;
            Vector3 rightHitPoint = rightHit.point;

            float frontYval = frontHitPoint.y - backHitPoint.y;
            float frontXval = Mathf.Sqrt(Mathf.Pow((frontHitPoint.x - backHitPoint.x), 2) + Mathf.Pow((frontHitPoint.z - backHitPoint.z), 2));
            float forwardRot = Mathf.Atan(-frontYval / frontXval) * Mathf.Rad2Deg;

            float rightYval = rightHitPoint.y - leftHitPoint.y;
            float rightXval = Mathf.Sqrt(Mathf.Pow((rightHitPoint.x - leftHitPoint.x), 2) + Mathf.Pow((rightHitPoint.z - leftHitPoint.z), 2));
            float rightRot = Mathf.Atan(rightYval / rightXval) * Mathf.Rad2Deg;

            Vector3 nextRot = new Vector3(forwardRot, 0f, rightRot);
            lastNextRot = nextRot;

            Quaternion thisRotation = transform.Find("main").localRotation;
            localEulerAngles += nextRot;
            transform.Find("main").localPosition = new Vector3(0, Mathf.Lerp(0, Yoffset, forwardRot / 90f));

            rampAngle = Mathf.Clamp(-forwardRot, 0, 90);
            RampAssist.transform.position = backHitPoint;
            RampAssist.transform.LookAt(frontHitPoint);
        }
        else
        {
            localEulerAngles += lastNextRot;
        }

        if (!Player) if (Vector3.Distance(transform.position, InGamePlayersKart.transform.position) > AI_Render_Distance) return;
        transform.Find("main").localRotation = Quaternion.RotateTowards(transform.Find("main").localRotation, Quaternion.Euler(localEulerAngles), 180*Time.fixedDeltaTime);
    }
    #endregion

#region Visual---------------------------------------------------------
    [Header("Visual------------------------------------------------------------------------------------------------------------------------------------------------------------------------")]
    public Vector3 positionOffset;

    public Transform cameraFollowPoint;
    public float CameraFollowSpeed = 20;

    public Transform CameraSpectatePoint;
    public float CameraSpectateFollowSpeed = 200;

    public GameObject KartUI;

    void MoveCamera()
    {
        if (rrm || !IsPlayerViewKart) return;

        if(rm != null)
        {
            if (rm.RaceLoadingStage < 2) return;
            if (rm.CameraInPanView)
            {
                return;
            }
            else if(rm.CurrentCameraViewOn == 0)
            {
                NormalCameraFollow();
            }else if(rm.CurrentCameraViewOn == 1)
            {
                CameraFollowSpectate();
            }

            return;
        }

        NormalCameraFollow();
    }

    void NormalCameraFollow()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraFollowPoint.position, Time.fixedDeltaTime * CameraFollowSpeed);
        Camera.main.transform.forward = Vector3.Lerp(Camera.main.transform.forward, cameraFollowPoint.transform.forward, Time.fixedDeltaTime * 5);

        float speedScale = Mathf.Abs(RealSpeed / 60f);
        speedScale = Mathf.Clamp(speedScale, 1, 1.5f);
        cameraFOV = Mathf.Lerp(cameraFOV, CarBoostInput > 0 ? speedScale * CameraStartFOV : CameraStartFOV, 5 * Time.fixedDeltaTime);

        Camera.main.fieldOfView = cameraFOV;
    }

    void CameraFollowSpectate()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, CameraSpectatePoint.position, Time.fixedDeltaTime * CameraSpectateFollowSpeed);
        Camera.main.transform.forward = Vector3.Lerp(Camera.main.transform.forward, CameraSpectatePoint.transform.forward, Time.fixedDeltaTime * CameraSpectateFollowSpeed);

        Camera.main.fieldOfView = CameraStartFOV;
    }


    float UI_Boost_Scale = 0;
    float UI_Boost_Preview_Scale = 0;
    void UpdateUI()
    {
        if (KartUI == null) return;

        KartUI.SetActive(Player && !rrm);
        if (!Player) return;

        float scale = BoostStorage / MaxBoostStorage;
        UI_Boost_Scale = Mathf.Lerp(UI_Boost_Scale, scale, Time.fixedDeltaTime * 4);
        KartUI.transform.Find("Main").transform.Find("Booster").transform.Find("BoosterFill").transform.localScale = new Vector3(UI_Boost_Scale, 1, 1);

        float BoostPreviewAmount = BoostLevel == 0 ? 0 : BoostLevel == 1 ? .5f : BoostLevel == 2 ? 1.75f : 2.8f;
        float PreviewScale = (BoostStorage / MaxBoostStorage) + (BoostPreviewAmount / MaxBoostStorage) + (LandBoostPotential() / MaxBoostStorage);
        UI_Boost_Preview_Scale = Mathf.Lerp(UI_Boost_Preview_Scale, PreviewScale, Time.fixedDeltaTime * 4);
        KartUI.transform.Find("Main").transform.Find("Booster").transform.Find("BoosterPreview").transform.localScale = new Vector3(UI_Boost_Preview_Scale, 1, 1);

    }
    #endregion

#region External Functions---------------------------------------------------------
    public void SuperBoostHit(float BoostTime)
    {
        if (timeSinceLastSuperBoost >= 1f)
        {
            timeSinceLastSuperBoost = 0;
            SuperBoostTime = BoostTime;
        }
    }

    #endregion

#region AI

    [Header("AI-------------------------------------------------------------------------")]
    [SerializeField] AIRoute CurrentRoute = null;
    public int AI_CurrentPointOn = 0;
    public GameObject InGamePlayersKart;

    void AI()
    {
        if (!Player && rm != null)
        {
            if(CurrentRoute == null)
            {
                AI_UpdateRoute();
                return;
            }
            AI_Move();
            AI_CheckReached();
            AI_Turn();
            AI_Drift();
            AI_Boost();
            AI_PowerUp();
        }
    }
    
    void AI_UpdateRoute()
    {
        CurrentRoute = rm.GetRandomAIRoute();
        AI_CurrentPointOn = 0;
    }

    public float AI_CheckRadius = 0.1f;
    Vector3 AI_PositionHeadingTo;
    void AI_CheckReached()
    {
        if(CurrentRoute.positions.Count == 0)
        {
            AI_UpdateRoute();
            return;
        }

        int HeadingTo = 0;
        if (AI_CurrentPointOn != CurrentRoute.positions.Count - 1) HeadingTo = AI_CurrentPointOn + 1;
        Debug.DrawLine(transform.position, CurrentRoute.positions[HeadingTo]);
        AI_PositionHeadingTo = CurrentRoute.positions[HeadingTo];
        float DistanceToNext = Vector3.Distance(transform.position, AI_PositionHeadingTo);


        if (DistanceToNext < CurrentRoute.CompleteRadius[HeadingTo])
        {
            if (HeadingTo == 0)
            {
                AI_UpdateRoute();
            }
            else
            {
                AI_CurrentPointOn++;
                //Debug.Log("AI Reached Location " + AI_CurrentPointOn);
            }
        }
    }

    void AI_Move()
    {
        InputY = 1;
        CarAccelerateInput = 1;
    }

    [SerializeField] float AI_Angle_To_Next_Point = 0;
    void AI_Turn()
    {
        Vector3 tP = transform.position;
        tP.y = 0;
        Vector3 bP = AI_PositionHeadingTo;
        bP.y = 0;

        Vector3 targetdir = tP - bP;

        AI_Angle_To_Next_Point = -Vector3.SignedAngle(targetdir, -transform.forward, Vector3.up);


        CarTurnInput = 0;
        CarDriftInput = 0;

        float TurnAmount = AI_Angle_To_Next_Point / 45;
        TurnAmount = TurnAmount >= 0 ? Mathf.Clamp(TurnAmount, 0.2f, 1f) : Mathf.Clamp(TurnAmount, -1f, 0.1f);

        if (AI_Angle_To_Next_Point > 25f)
        {
            AI_Drift_Time = Random.Range(0.25f, 1.5f);
            CarTurnInput = TurnAmount;
        }
        else if (AI_Angle_To_Next_Point > 15)
        {
            AI_Drift_Time = Random.Range(0.25f, 1.5f);
            CarTurnInput = TurnAmount;
        }
        else if (AI_Angle_To_Next_Point > 10f)
        {
            CarTurnInput = TurnAmount;
        }
        else if (AI_Angle_To_Next_Point < -25)
        {
            AI_Drift_Time = -Random.Range(0.25f, 1.5f);
            CarTurnInput = TurnAmount;
        }
        else if (AI_Angle_To_Next_Point < -15)
        {
            AI_Drift_Time = -Random.Range(0.25f, 1.5f);
            CarTurnInput = TurnAmount;
        }
        else if (AI_Angle_To_Next_Point < -10f)
        {
            CarTurnInput = TurnAmount;
        }
    }


    float AI_Drift_Time = 0f;

    void AI_Drift()
    {
        if(AI_Drift_Time > 0.02f)
        {
           AI_Drift_Time -= Time.deltaTime;
            CarDriftInput = 1;
        }
        else if(AI_Drift_Time < -0.02f)
        {
            AI_Drift_Time += Time.deltaTime;
            CarDriftInput = 1;
        }
    }

    void AI_Boost()
    {
        //dont boost if drifting or if not at a close angle to the next checkpoint
        CarBoostInput = (IsSliding || Mathf.Abs(AI_Angle_To_Next_Point) > 10f) ? 0 : 1;
    }

    void AI_PowerUp()
    {

    }
    #endregion

#region SFX

    [Header("AUDIO-------------------------------------------------------------------------------------")]
    public AudioClip EngineAudio;

    public AudioClip JumpAudio;
    public float JumpAudioVolume;

    public AudioClip BoostAddAudio;
    public float BoostAddAudioVolume;

    public AudioClip LandAudio;
    public float LandAudioVolume;

    public AudioSource EngineAudioSourceLocal;
    public AudioSource EngineAudioSourceGlobal;
    public AudioSource BoosterAudioSourceLocal;
    public AudioSource BoosterAudioSourceGlobal;
    public AudioSource SFXAudioSourceLocal;
    public AudioSource SFXAudioSourceGlobal;

    void StartupSFX()
    {
        if (!IsPlayerViewKart)
        {
            EngineAudioSourceLocal.gameObject.SetActive(false);
            EngineAudioSourceGlobal.gameObject.SetActive(true);
            BoosterAudioSourceLocal.gameObject.SetActive(false);
            BoosterAudioSourceGlobal.gameObject.SetActive(true);
            SFXAudioSourceLocal.gameObject.SetActive(false);
            SFXAudioSourceGlobal.gameObject.SetActive(true);
        }
    }

    [SerializeField] float lastPitch = 1f;
    public float EnginePitchMin;
    public float EnginePitchMax;
    public float EnginePitchScale;

    public float EngineVolumeMin;
    public float EngineVolumeMax;
    void EngineSound()
    {
        TimeSinceLastAudioClip += Time.fixedDeltaTime;

        float pitch = Mathf.Abs(RealSpeed) * EnginePitchScale / 45f;
        pitch = Mathf.Clamp(pitch, EnginePitchMin, EnginePitchMax);
        pitch += 1;
        lastPitch = Mathf.Lerp(lastPitch, pitch, 5 * Time.deltaTime);
        float vol = Mathf.Lerp(EngineVolumeMin, EngineVolumeMax, (lastPitch-1-EnginePitchMin) / EnginePitchMax );
                                            
        if (IsPlayerViewKart)
        {
            EngineAudioSourceLocal.clip = EngineAudio;
            if (!EngineAudioSourceLocal.isPlaying) { EngineAudioSourceLocal.Play(); EngineAudioSourceLocal.loop = true; }
            EngineAudioSourceLocal.pitch = lastPitch;
            EngineAudioSourceLocal.volume = vol * gm.MasterVolume * gm.SFXVolume;
        }
        else
        {
            EngineAudioSourceGlobal.clip = EngineAudio;
            if (!EngineAudioSourceGlobal.isPlaying) { EngineAudioSourceGlobal.Play(); EngineAudioSourceGlobal.loop = true; }
            EngineAudioSourceGlobal.pitch = lastPitch;
            EngineAudioSourceGlobal.volume = vol * gm.MasterVolume * gm.OtherPlayerVolume;
        }
    }

    float TimeSinceLastAudioClip = 0f;
    void PlaySoundEffect(AudioClip clip, float volume)
    {
        if (TimeSinceLastAudioClip < 0.1f) return;

        TimeSinceLastAudioClip = 0f;
        if (IsPlayerViewKart)
        {
            SFXAudioSourceLocal.PlayOneShot(clip, volume * gm.MasterVolume * gm.SFXVolume);
        }
        else
        {
            SFXAudioSourceGlobal.PlayOneShot(clip, volume * gm.MasterVolume * gm.OtherPlayerVolume);
        }
    }

    IEnumerator PlaySFXAfterDelay(AudioClip clip, float volume, float Delay)
    {
        yield return new WaitForSeconds(Delay);
        PlaySoundEffect(clip, volume);
        yield return false;
    }



    #endregion
}
