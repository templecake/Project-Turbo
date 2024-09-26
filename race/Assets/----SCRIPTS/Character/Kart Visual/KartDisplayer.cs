using Formatting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartDisplayer : MonoBehaviour
{
    public bool DisplayCharacter;
    public bool PlayerKart;
    public bool CreateOnStart= false;
    public bool RemoveColliders = false;
    public int PlayerKartID;
    public int AIKartID;

    public bool EngineSound;

    [SerializeField] Transform blockParent;
    [SerializeField] GameObject PreviewCharacter;

    public List<GameObject> CarBlocks = new List<GameObject>();
    public List<GameObject> FrontTyres = new List<GameObject>();
    public List<GameObject> BackTyres = new List<GameObject>();
    public List<GameObject> AllTyres = new List<GameObject>();

    List<ParticleSystem> BoostParticles = new List<ParticleSystem>();
    List<ParticleSystem> StageOneDriftParticles = new List<ParticleSystem>();
    List<ParticleSystem> StageTwoDriftParticles = new List<ParticleSystem>();
    List<ParticleSystem> StageThreeDriftParticles = new List<ParticleSystem>();

    private void Start()
    {
        if (CreateOnStart)
        {
            CreateCarBlocks();
        }
    }

    private void Update()
    {
        WheelVisual();
        DriftVisual();
    }

    public Car ThisCar = new Car();
    public void CreateCarBlocks()
    {
        BlockStorage bM = FindObjectOfType<BlockStorage>();
        GameManager gM = FindObjectOfType<GameManager>();
        Player player = FindObjectOfType<Player>();
        AIManager aM = FindObjectOfType<AIManager>();

        float LowestY = 0;

        for (int i = 0; i < CarBlocks.Count; i++)
        {
            Destroy(CarBlocks[i]);
        }
        CarBlocks.Clear();

        if (PlayerKart && PlayerKartID == -1) return;
        if (!PlayerKart && AIKartID == -1) return;

        if (PlayerKart)
        {
          if(player.playersKart.Count <= 0) { Debug.LogError("Player Has No Karts!"); return; }

          if(player.playersKart[PlayerKartID] == null) { Debug.LogError("Player Kart does not exist."); return; }

            ThisCar = Format.CopyCar(player.playersKart[PlayerKartID]);
        }
        else
        {
            if (AIKartID >= aM.AICharacters.Count) { Debug.LogError("No AI with ID " + AIKartID); return; }

            if (aM.AICharacters[AIKartID].charactersCar == null) { Debug.LogError("No car for ID " + AIKartID); return; }

            ThisCar = Format.CopyCar(aM.AICharacters[AIKartID].charactersCar);
        }

        ClearLists();

        for (int i = 0; i < ThisCar.blocks.Count; i++)
        {
            BuildingBlock bl = bM.blocks[ThisCar.blocks[i]];
            GameObject block = Instantiate(bl.buildingPrefab);
            block.transform.SetParent(blockParent.transform);
            block.transform.localScale = new Vector3(bl.SizeX, bl.SizeY, bl.SizeZ);
            block.transform.localPosition = ThisCar.positions[i];
            block.transform.localRotation = Quaternion.Euler(ThisCar.rotations[i]);

            if (bl.canBeColoured)
            {
                MeshRenderer[] meshes = block.GetComponentsInChildren<MeshRenderer>();
                for (int mesh = 0; mesh < meshes.Length; mesh++) { if (meshes[mesh].transform.parent.name == "paintable") meshes[mesh].material = bM.materials[ThisCar.materials[i]].LinkedMaterial; }
            }

            if (RemoveColliders)
            {
                Collider[] cols = block.GetComponentsInChildren<Collider>();
                for (int x = 0; x < cols.Length; x++)
                {
                    Destroy(cols[x]);
                }
            }

            if (bl.blockType == GameManager.BlockType.Tyre)
            {
                AllTyres.Add(block);
                if (ThisCar.positions[i].z > 0)
                {
                    FrontTyres.Add(block);
                }
                if (ThisCar.positions[i].z < 0)
                {
                    BackTyres.Add(block);
                }
                if (ThisCar.positions[i].x > 0)
                {
                    block.transform.Find("tyre").Find("driftParticles").localScale = new Vector3(1, 1, 1);
                }
                if (ThisCar.positions[i].x < 0)
                {
                    block.transform.Find("tyre").Find("driftParticles").localScale = new Vector3(-1, 1, 1);
                }
            }

            block.transform.name = i.ToString();
            block.layer = LayerMask.NameToLayer("Kart");
            CarBlocks.Add(block);

            if (ThisCar.positions[i].y < LowestY) LowestY = ThisCar.positions[i].y;

            if (bl.blockType == GameManager.BlockType.Booster)
            {
                ParticleSystem[] parts = block.transform.Find("boostParticles").GetComponentsInChildren<ParticleSystem>();
                for (int x = 0; x < parts.Length; x++)
                {
                    BoostParticles.Add(parts[x]);
                }
            }
        }

        for (int i = 0; i < BackTyres.Count; i++)
        {
            StageOneDriftParticles.Add(BackTyres[i].transform.Find("tyre").Find("driftParticles").Find("StageOne").GetComponent<ParticleSystem>());
            StageTwoDriftParticles.Add(BackTyres[i].transform.Find("tyre").Find("driftParticles").Find("StageTwo").GetComponent<ParticleSystem>());
            StageThreeDriftParticles.Add(BackTyres[i].transform.Find("tyre").Find("driftParticles").Find("StageThree").GetComponent<ParticleSystem>());
        }

        for (int i = 0; i < AllTyres.Count; i++)
        {
            AllTyres[i].GetComponent<TyreConnectionCheck>().CheckConnections();
        }


        int SelectedSeat = ThisCar.SeatSelected;

        GameObject seat = Instantiate(bM.seats[SelectedSeat]);
        seat.transform.SetParent(blockParent);
        seat.transform.localPosition = Vector3.zero;
        seat.transform.localRotation = Quaternion.identity;
        seat.transform.localScale = Vector3.one;


        MeshRenderer[] seatPaintables = seat.transform.Find("paintable").GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < seatPaintables.Length; i++)
        {
            seatPaintables[i].material = bM.materials[ThisCar.seatColour].LinkedMaterial;
        }

        CarBlocks.Add(seat);

        if (DisplayCharacter)
        {
            GameObject character = Instantiate(PreviewCharacter);
            character.transform.SetParent(blockParent);
            character.transform.localScale = Vector3.one;
            character.transform.position = seat.transform.Find("driverPosition").position;
            character.transform.localRotation = Quaternion.identity;

            CustomisationManager cM = FindObjectOfType<CustomisationManager>();

            if (PlayerKart)
            {
                character.GetComponent<DisplayCharacterScript>().HeadChosen = cM.HeadSelected;
                character.GetComponent<DisplayCharacterScript>().BodyChosen = cM.BodySelected;
                character.GetComponent<DisplayCharacterScript>().LegsChosen = cM.LegsSelected;
                character.GetComponent<DisplayCharacterScript>().AnimationStyleChosen = cM.AnimationStyleSelected;
            }
            else
            {
                character.GetComponent<DisplayCharacterScript>().HeadChosen = aM.AICharacters[AIKartID].CharacterHead;
                character.GetComponent<DisplayCharacterScript>().BodyChosen = aM.AICharacters[AIKartID].CharacterBody;
                character.GetComponent<DisplayCharacterScript>().LegsChosen = aM.AICharacters[AIKartID].CharacterLegs;
                character.GetComponent<DisplayCharacterScript>().AnimationStyleChosen = aM.AICharacters[AIKartID].CharacterStyle;
            }


            character.GetComponent<DisplayCharacterScript>().RefreshCharacter();

            character.GetComponent<DisplayCharacterScript>().PlayAnimation("DRIVE_IDLE");

            CarBlocks.Add(character);
        }

        blockParent.transform.localPosition = new Vector3(0, -1.5f - LowestY, 0);
    }

    void ClearLists()
    {
        for (int x = 0; x < FrontTyres.Count; x++)
        {
            Destroy(FrontTyres[x]);
        }
        FrontTyres.Clear();

        for (int x = 0; x < BackTyres.Count; x++)
        {
            Destroy(BackTyres[x]);
        }
        BackTyres.Clear();

        for (int x = 0; x < AllTyres.Count; x++)
        {
            Destroy(AllTyres[x]);
        }
        AllTyres.Clear();
    }

    #region Visual

    public float DisplayDriveSpeed;

    void WheelVisual()
    {
        for (int i = 0; i < AllTyres.Count; i++)
        {
            float spinSpeed = (DisplayDriveSpeed / 60f);
            spinSpeed = Mathf.Clamp(spinSpeed, 0, 1f);
            if (Mathf.Abs(DisplayDriveSpeed) < 0.5f) spinSpeed = 0;
            AllTyres[i].transform.Find("tyre").Find("tyremain").Find("wheel").Rotate(new Vector3(-spinSpeed * 360 * 8 * Time.deltaTime, 0, 0), Space.Self);
        }
    }

    public bool IsDrifting;
    public float TimeDriftingFor;
    int BoostLevel = 0;
    void DriftVisual()
    {
        TimeDriftingFor = IsDrifting ? TimeDriftingFor += Time.deltaTime : 0;
        BoostLevel = TimeDriftingFor >= 3 ? 3 : TimeDriftingFor >= 2 ? 2 : TimeDriftingFor >= 1 ? 1 : 0;

        for (int i = 0; i < StageOneDriftParticles.Count; i++)
        {
            if (!StageOneDriftParticles[i]) continue;
            ParticleSystem StageOne = StageOneDriftParticles[i];
            if (BoostLevel < 1) StageOne.Stop();
            if (BoostLevel >= 1 && !StageOne.isPlaying) StageOne.Play();
        }

        for (int i = 0; i < StageTwoDriftParticles.Count; i++)
        {
            if (!StageTwoDriftParticles[i]) continue;
            ParticleSystem StageTwo = StageTwoDriftParticles[i];
            if (BoostLevel < 2) StageTwo.Stop();
            if (BoostLevel >= 2 && !StageTwo.isPlaying) StageTwo.Play();
        }

        for (int i = 0; i < StageThreeDriftParticles.Count; i++)
        {
            if (!StageThreeDriftParticles[i]) continue;
            ParticleSystem StageThree = StageThreeDriftParticles[i];
            if (BoostLevel < 3) StageThree.Stop();
            if (BoostLevel >= 3 && !StageThree.isPlaying) StageThree.Play();
        }
    }

    #endregion
}
