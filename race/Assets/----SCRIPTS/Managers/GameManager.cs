using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Car
{
    public string KartName;

    public List<int> blocks;
    public List<Vector3> rotations;
    public List<Vector3> positions;
    public List<int> materials;
    public int seatColour;
    public int SeatSelected;

    public void ReturnStats(BlockStorage blockStorage, out float Weight, out float MaxSpeed, out float Acceleration, out float BoostGain,
       out float BoostCapacity, out float BoostAcceleration, out float BoostUsage, out float BoostMaxSpeed, out float SteerSpeed)
    {
        Weight = 0;
        for (int i = 0; i < blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[blocks[i]];
            Weight += bl.Weight;
        }

        MaxSpeed = 0;
        for (int i = 0; i < blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[blocks[i]];
            MaxSpeed += bl.MaxSpeed;
        }

        Acceleration = 0;
        for (int i = 0; i < blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[blocks[i]];
            Acceleration += bl.Acceleration;
        }

        BoostGain = 0; //average
        int BGs = 0;
        for (int i = 0; i < blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[blocks[i]];
            //if (bl.BoostGain > 0) BGs++;
            //BoostGain += bl.BoostGain;
        }
        BoostGain /= (BGs > 0 ? BGs : 1);

        BoostCapacity = 0;
        for (int i = 0; i < blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[blocks[i]];
            BoostCapacity += bl.BoostCapacity;
        }

        BoostUsage = 0;
        for (int i = 0; i < blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[blocks[i]];
            //BoostUsage += bl.BoostUsage;
        }

        BoostAcceleration = 0; //average
        int BAs = 0;
        for (int i = 0; i < blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[blocks[i]];
            //if (bl.BoostAcceleration > 0) BAs++;
            //BoostAcceleration += bl.BoostAcceleration;
        }
        BoostAcceleration /= (BAs > 0 ? BAs : 1);

        BoostMaxSpeed = 0; //average
        int BMSs = 0;
        for (int i = 0; i < blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[blocks[i]];
            if (bl.BoostMaxSpeed > 0) BMSs++;
            BoostMaxSpeed += bl.BoostMaxSpeed;
        }
        BoostMaxSpeed /= (BMSs > 0 ? BMSs : 1);

        SteerSpeed = 0;
        int SSs = 0;
        for (int i = 0; i < blocks.Count; i++)
        {
            BuildingBlock bl = blockStorage.blocks[blocks[i]];
            if (bl.SteerSpeed > 0) SSs++;
            SteerSpeed += bl.SteerSpeed;
        }
        SteerSpeed /= (SSs > 0 ? SSs : 1);
    }

    public void ReturnVisualStats(BlockStorage blockStorage, out float Weight, out float Speed, out float Acceleration, out float Boost, out float Steering)
    {
        float TempWeight = 0;
        float TempSpeed = 0;
        float TempAccel = 0;
        float TempBoostAccel = 0;
        float TempBoostSpeed = 0;
        float TempSteer = 0;
        float unused = 0;

        ReturnStats(blockStorage, out TempWeight, out TempSpeed, out TempAccel, out unused, out unused, out TempBoostAccel, out unused, out TempBoostSpeed, out TempSteer);

        Weight = Mathf.Clamp(TempWeight / 200f, 0, 1);
        Speed = Mathf.Clamp(TempSpeed / 80f, 0, 1);
        Acceleration = Mathf.Clamp(TempAccel / 50f, 0, 1);
        Boost = Mathf.Clamp((TempBoostAccel + TempBoostSpeed) / 120f, 0, 1);
        Steering = Mathf.Clamp(TempSteer / 10f, 0, 1);
    }

}

[System.Serializable]

public class PlayerCar
{
    public GameObject ingameKart;
    public GameObject playerBlip;

    public Sprite DisplayImage;
    public string PlayerName;

    public Vector3 currentRotation;
    public Vector3 currentPosition;

    public bool isPlayerCar;
    public bool isAI;
    public int aiID;
    public bool Finished;

    public int CurrentCheckpoint;
    public int CurrentLap;
    public float[] LapTime;

    public float Progress;
    public int AIKartOn;

    public int Place;
    public int FinishPlace;

    public void UpdateCheckpoint(int newc, RaceManager r){
        CurrentCheckpoint = newc;
        if (newc == 0) { UpdateLap(r); }
    }

    public void UpdatePosition(Vector3 newpos)
    {
        currentPosition = newpos;
    }

    public void UpdateRotation(Vector3 newRot)
    {
        currentRotation = newRot;
    }

    public void UpdateLapTime(float time)
    {
        if(!Finished)
        LapTime[CurrentLap] += time;
    }

    public void UpdateLap(RaceManager r)
    {
        if (Finished) { CurrentLap = r.Laps; return; }
        if (CurrentLap == r.Laps - 1) { Finished = true; return; }
        CurrentLap++;
    }

    public void UpdateProgress(float prog)
    {
        Progress = prog;
    }

    public void UpdatePlace(int place)
    {
        Place = place;
    }

    public void UpdateFinishPlace(int place)
    {
        Place = place;
    }

    public float GetTotalTime()
    {
        float t = 0;
        for (int i = 0; i < LapTime.Length; i++)
        {
            t += LapTime[i];
        }
        return t;
    }
}

namespace Formatting{

    public static class Format
    {
        public static string FormatTime(float input)
        {
            float InputTime = input;

            float mins = InputTime / 60; mins = Mathf.Floor(mins);
            float secs = InputTime - (mins * 60); secs = Mathf.Floor(secs);
            float millisecs = InputTime - (mins * 60) - (secs); millisecs *= 1000; millisecs = Mathf.Floor(millisecs);

            string m = mins >= 10 ? mins.ToString() : "0" + mins;
            string s = secs >= 10 ? secs.ToString() : "0" + secs;
            string ms = millisecs >= 100 ? millisecs.ToString() : millisecs >= 10 ? "0" + millisecs : "00" + millisecs;

            string output = m + ":" + s + ":" + ms;
            return output;
        }

        public static string FormatPosition(int input)
        {
            string r = "N/A";
            switch (input)
            {
                case 1:
                    r = "1st";
                    break;

                case 2:
                    r = "2nd";
                    break;

                case 3:
                    r = "3rd";
                    break;

                case 4:
                    r = "4th";
                    break;

                case 5:
                    r = "5th";
                    break;

                case 6:
                    r = "6th";
                    break;

                case 7:
                    r = "7th";
                    break;

                case 8:
                    r = "8th";
                    break;
            }
            return r;
        }

        public static Car CopyCar(Car c)
        {
            Car thisCar = new Car();

            thisCar.blocks = new List<int>();
            thisCar.positions = new List<Vector3>();
            thisCar.rotations = new List<Vector3>();
            thisCar.materials = new List<int>();

            for (int i = 0; i < c.blocks.Count; i++)
            {
                thisCar.blocks.Add(c.blocks[i]);
            }

            for (int i = 0; i < c.positions.Count; i++)
            {
                thisCar.positions.Add(c.positions[i]);
            }

            for (int i = 0; i < c.rotations.Count; i++)
            {
                thisCar.rotations.Add(c.rotations[i]);
            }

            for (int i = 0; i < c.materials.Count; i++)
            {
                thisCar.materials.Add(c.materials[i]);
            }

            thisCar.seatColour = c.seatColour;

            thisCar.SeatSelected = c.SeatSelected;

            thisCar.KartName = c.KartName;

            return thisCar;
        }

        public static string FormatFloat(float number)
        {
            return number.ToString("N0");
        }

        public static string FormatInt(int number)
        {
            return number.ToString("N0");
        }

        public static float RoundToDecimal(float number, int decimalPlaces)
        {
            float multiplier = Mathf.Pow(10, decimalPlaces);
            return Mathf.Round(number * multiplier) / multiplier;
        }
    }
}


[System.Serializable]
public class AIRoute
{
    public List<Vector3> positions;
    public List<float> CompleteRadius;
    public List<int> num;
    public Color RouteColour;
    public bool Displaying;
    public bool ShowOverlay;
}

[System.Serializable]
public class PowerUp{

    public string PowerUpName;
    public GameObject PowerUpObject;
    public GameManager.PowerUpType powerUpType;
}

[System.Serializable]
public class SoundEffect
{
    public AudioClip clip;
    public float clipVolume;
}


public class GameManager : MonoBehaviour
{
    public List<GameObject> Maps;
    public List<PowerUp> Powerups;
    public List<SoundEffect> soundEffects;

    [Header("Audio")]
    public float MasterVolume;
    public float SFXVolume;
    public float MusicVolume;
    public float OtherPlayerVolume;

    public bool StartedEditingKart;
    public int EditingKartID = 0;

    public bool StartedSoloRaceVariables;
    public int SoloRace_MapChosen;
    public int SoloRace_RaceType; //0 - standard, 1 - time trial
    public int SoloRace_RaceDifficulty; //0 - Easy, 1 - Normal, 2 - Hard, 3 - Expert
    public int SoloRace_AIAmount;                                
    public int SoloRace_KartType; //0 - custom, 1 - standardised

    [Header("Temp")]
    public int LastMenuOpened;
    public int LastMenuVisited; //0 - garage, 1 - block shop, 2 - crate shop, 3 - daily shop. 

    public enum BlockType
    {
        BuildingBlock, Tyre, Engine, Booster
    }

    public enum ControllerType
    {
        Xbox, Playstation
    }

    public enum PowerUpType
    {
        Personal, NoTargetting, TargettingSingle, TargettingFirst, TargettingAll
    }

    public enum AIDifficulty
    {
        Basic, Easy, Medium, Hard
    }

    public enum RaceType
    {
        Standard, TimeTrial
    }

    public GameObject GetMap(int map)
    {
        return Maps[map];
    }

    float TimeSinceLastSoundEffect = 0f;
    public void PlaySoundEffect(int soundEffect)
    {
        if (TimeSinceLastSoundEffect < 0.05f) return;
        GetComponent<AudioSource>().PlayOneShot(soundEffects[soundEffect].clip, soundEffects[soundEffect].clipVolume * MasterVolume * SFXVolume);
        TimeSinceLastSoundEffect = 0;
    }

    public void PlaySoundClip(AudioClip clip, float volume)
    {
        GetComponent<AudioSource>().PlayOneShot(clip, volume * MasterVolume * SFXVolume);
    }

    bool Loading;
    [SerializeField] GameObject LoadPanel;
    public IEnumerator LoadSceneCoro(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
        while (SceneManager.GetActiveScene().name != SceneName)
        {
            LoadPanel.SetActive(true);
            yield return new WaitForSeconds(Time.deltaTime); //Wait until scene is loaded
        }
        LoadPanel.SetActive(false);
        yield return false;
    }

    public void LoadScene(string SceneName)
    {
        StartCoroutine(LoadSceneCoro(SceneName));
    }

    public void ReturnToMainMenu()
    {
        LoadScene("MAINMENU");
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 144;
        LoadScene("INTROSCENE");
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyUp(KeyCode.P))
        {
            StartCoroutine(StartNormalRace());
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyUp(KeyCode.M))
        {
            ReturnToMainMenu();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyUp(KeyCode.I))
        {
            LoadScene("CUSTOMISATION");
        }
        TimeSinceLastSoundEffect += Time.deltaTime;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public IEnumerator StartNormalRace()
    {
        LoadScene("RACE");

        while (SceneManager.GetActiveScene().name != "RACE")
        {
            LoadPanel.SetActive(true);
            yield return new WaitForSeconds(Time.deltaTime); //Wait until scene is loaded
        }

        yield return false;
    }

    [SerializeField] Sprite[] CurrencySprites;
    [SerializeField] Color[] CurrencyColours;
    public Sprite ReturnCubitSpriteFromName(string cname)
    {
        switch (cname)
        {
            case "Adbit":
                return CurrencySprites[1];
            case "Golden Cubit":
                return CurrencySprites[2];
            default:
                return CurrencySprites[0];
        }
    }

    public Color ReturnCubitColorFromName(string cname)
    {
        switch (cname)
        {
            case "Adbit":
                return CurrencyColours[1];
            case "Golden Cubit":
                return CurrencyColours[2];
            default:
                return CurrencyColours[0];
        }
    }
}
