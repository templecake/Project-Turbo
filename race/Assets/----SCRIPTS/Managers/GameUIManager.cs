using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    ItemWorldImageRenderManager Images;
    public List<AnimationEvent> EventsInQueue = new List<AnimationEvent>();
    bool running;

    public GameObject[] UIScreens;
    [SerializeField] Text fpsText;

    float up = 0;

    GameManager gm;

    private void Start()
    {
        Images = GetComponent<ItemWorldImageRenderManager>();
        gm = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        up += Time.deltaTime;
        if (up > 0.5f)
        {
            up = 0;
            int fps = (int)Mathf.Floor(1 / Time.deltaTime);
            fpsText.text = fps + " FPS";
        }

        if (Input.GetKeyUp(KeyCode.Y))
        {
            AddEvent("ItemGot", -1, -1, Random.Range(0,13), Random.Range(1,10), -1, false);
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            AddEvent("CharacterUnlock", -1, 0, -1, -1, -1, false);
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            if (Random.Range(0, 100) < 50)
            {
                AddEvent("ShopPreview", Random.Range(0, 16), -1, -1, -1, -1, false);
            }
            else
            {
                AddEvent("ShopPreview", -1 , -1, Random.Range(0, 14), Random.Range(0, 13), -1, false);
            }
        }  

        if(Input.GetKeyUp(KeyCode.Escape))
        {
            if (EventsInQueue.Count > 0)
            {
                EventFinished();
            }
            else if(!PauseMenu.activeSelf)
            {
                OpenPauseMenu();
            }
            else
            {
                ClosePauseMenu();
            } 

        }


        if (Input.GetMouseButtonDown(0))
        {
            CrackedAmount++;
        }
    }

    private void CheckEvents()
    {
        if(EventsInQueue.Count>0 && !running)
        {
            RunEvent();
        }
    }

    public void AddEvent(string EventType, int CustomisationReceived, int CharacterUnlockedID, int BlockUnlockedID, int BlockGotAmount, int Rarity, bool AlreadyOwned)
    {
        AnimationEvent av = new AnimationEvent();
        av.EventType = EventType;
        av.CustomisationReceived = CustomisationReceived;
        av.CharacterUnlockedID = CharacterUnlockedID;
        av.BlockUnlockedID = BlockUnlockedID;
        av.BlockGotAmount = BlockGotAmount;
        av.ItemRarity = Rarity;
        av.AlreadyOwned = AlreadyOwned;
        EventsInQueue.Add(av);

        CheckEvents();
    }

    public void RunEvent()
    {
        running = true;
        StartCoroutine("Run" + EventsInQueue[0].EventType);
    }

    void CleanUp()
    {
        for (int i = 0; i < UIScreens.Length; i++)
        {
            UIScreens[i].SetActive(false);
        }
    }

    public void EventFinished()
    {
       CleanUp();
       EventsInQueue.RemoveAt(0);
        running = false;
        CheckEvents();
    }

    IEnumerator RunCharacterUnlock()
    {
        CustomisationManager cm = GetComponent<CustomisationManager>();
        int currentChar = EventsInQueue[0].CharacterUnlockedID;
        Image[] streaks = UIScreens[0].transform.Find("Background").Find("ColouredStreaks").GetComponentsInChildren<Image>();

        for (int i = 0; i < streaks.Length; i++)
        {
            streaks[i].color = cm.CharacterSets[currentChar].unlockStreakColour;
        }

        UIScreens[0].transform.Find("Background").GetComponent<Image>().color = cm.CharacterSets[currentChar].unlockBackgroundColour;
        UIScreens[0].transform.Find("Background").Find("CharacterImage").GetComponent<Image>().sprite = cm.CharacterSets[currentChar].CharacterImage;

        UIScreens[0].transform.Find("Background").Find("StreakAndTitle").Find("StreakMain").GetComponent<Image>().color = cm.CharacterSets[currentChar].unlockBarColourMain;
        UIScreens[0].transform.Find("Background").Find("StreakAndTitle").Find("StreakAccent").GetComponent<Image>().color = cm.CharacterSets[currentChar].unlockBarColourAccent;

        UIScreens[0].transform.Find("Background").Find("StreakAndTitle").Find("CharacterNameText").GetComponent<Text>().color = cm.CharacterSets[currentChar].unlockTextColour;
        UIScreens[0].transform.Find("Background").Find("StreakAndTitle").Find("CharacterNameText").GetComponent<Text>().text = cm.CharacterSets[currentChar].DisplayName;
        UIScreens[0].transform.Find("Background").Find("StreakAndTitle").Find("CharacterDescription").GetComponent<Text>().text = cm.CharacterSets[currentChar].Description;


        UIScreens[0].SetActive(true);

        yield return false;
    }

    IEnumerator RunItemGot()
    {
        string type = EventsInQueue[0].CustomisationReceived > -1 ? "Customisation" : "Block";
        Transform screen = UIScreens[1].transform;

        CustomisationManager.CustomisationRarity rarity = CustomisationManager.CustomisationRarity.Common;
        Color rarityColour = Color.white;
        string itemType = "";
        string dName = "";
        Texture displayImage = null;
        int amount = EventsInQueue[0].BlockGotAmount;
        int RarityNum = 1;

        if (type == "Customisation")
        {
            CustomisationManager cm = GetComponent<CustomisationManager>();
            int current = EventsInQueue[0].CustomisationReceived;
            Customisation thisCustom = cm.AllCustomisations[current];
            itemType = cm.GetTypeFromItem(current);
            rarity = thisCustom.Rarity;
            rarityColour = FindObjectOfType<CustomisationManager>().GetColorFromRarity(rarity);
            RarityNum = FindObjectOfType<CustomisationManager>().GetIdFromRarity(rarity) + 1;
            displayImage = Images.CustomisationItemTextures[current];
            dName = thisCustom.DisplayName;
            Transform ItemStats = screen.Find("Stats").Find("ItemStats");
            ItemStats.Find("Weight").gameObject.SetActive(false);
            ItemStats.Find("Speed").gameObject.SetActive(false);
            ItemStats.Find("Boost").gameObject.SetActive(false);
            ItemStats.Find("Steering").gameObject.SetActive(false);

            screen.Find("Background").GetComponent<Image>().color = rarityColour;
        }
        if(type == "Block") 
        {
            BlockStorage bs = GetComponent<BlockStorage>();
            int currentBlock = EventsInQueue[0].BlockUnlockedID;
            BuildingBlock thisBlock = bs.blocks[currentBlock];

            itemType = bs.GetNameFromBlockType(thisBlock.blockType);
            rarityColour = FindObjectOfType<CustomisationManager>().GetColorFromRarity(thisBlock.Rarity);
            RarityNum = FindObjectOfType<CustomisationManager>().GetIdFromRarity(thisBlock.Rarity) + 1;
            displayImage = Images.BlockItemTextures[currentBlock];
            dName = thisBlock.DisplayName;
            Transform ItemStats = screen.Find("Stats").Find("ItemStats");

            ItemStats.Find("Weight").Find("Slider").GetComponent<Slider>().value = thisBlock.Weight / 40f;
            ItemStats.Find("Speed").gameObject.SetActive(false);
            ItemStats.Find("Boost").gameObject.SetActive(false);
            ItemStats.Find("Steering").gameObject.SetActive(false);
            if (itemType == "Tyre")
            {
                ItemStats.Find("Steering").gameObject.SetActive(true);
                ItemStats.Find("Steering").Find("Slider").GetComponent<Slider>().value = thisBlock.SteerSpeed / 10f;
            }

            if (itemType == "Booster")
            {
                ItemStats.Find("Boost").gameObject.SetActive(true);
                ItemStats.Find("Boost").Find("Slider").GetComponent<Slider>().value = thisBlock.BoostMaxSpeed / 10f;
            }

            if (itemType == "Engine")
            {
                ItemStats.Find("Speed").gameObject.SetActive(true);
                ItemStats.Find("Speed").Find("Slider").GetComponent<Slider>().value = thisBlock.MaxSpeed / 20f;
            }
        }
        
        UIScreens[1].SetActive(true);

       

        screen.Find("Background").GetComponent<Image>().color = rarityColour;

        screen.Find("ItemImage").GetComponent<RawImage>().texture = displayImage;
        screen.Find("TypeName").GetComponent<Text>().text = (rarity.ToString() + " " + itemType);

        screen.Find("DisplayName").GetComponent<Text>().text = dName;

        screen.Find("ItemImage").GetComponentInChildren<Text>().text = amount > 1 ? ("x" + amount) : "";

        screen.Find("Stars").Find("StarColoured1").gameObject.SetActive(RarityNum > 0);
        screen.Find("Stars").Find("StarColoured2").gameObject.SetActive(RarityNum > 1);
        screen.Find("Stars").Find("StarColoured3").gameObject.SetActive(RarityNum > 2);
        screen.Find("Stars").Find("StarColoured4").gameObject.SetActive(RarityNum > 3);
        screen.Find("Stars").Find("StarColoured5").gameObject.SetActive(RarityNum > 4);
        screen.Find("Stars").Find("StarColoured6").gameObject.SetActive(RarityNum > 5);
        screen.Find("Stars").Find("StarFaded1").gameObject.SetActive(RarityNum <= 0);
        screen.Find("Stars").Find("StarFaded2").gameObject.SetActive(RarityNum <= 1);
        screen.Find("Stars").Find("StarFaded3").gameObject.SetActive(RarityNum <= 2);
        screen.Find("Stars").Find("StarFaded4").gameObject.SetActive(RarityNum <= 3);
        screen.Find("Stars").Find("StarFaded5").gameObject.SetActive(RarityNum <= 4);
        screen.Find("Stars").Find("StarFaded6").gameObject.SetActive(RarityNum <= 5);

        screen.Find("Stars").Find("StarColoured1").Find("Star").GetComponent<Image>().color = rarityColour*1.25f;
        screen.Find("Stars").Find("StarColoured2").Find("Star").GetComponent<Image>().color = rarityColour * 1.25f;
        screen.Find("Stars").Find("StarColoured3").Find("Star").GetComponent<Image>().color = rarityColour * 1.25f;
        screen.Find("Stars").Find("StarColoured4").Find("Star").GetComponent<Image>().color = rarityColour * 1.25f;
        screen.Find("Stars").Find("StarColoured5").Find("Star").GetComponent<Image>().color = rarityColour * 1.25f;
        screen.Find("Stars").Find("StarColoured6").Find("Star").GetComponent<Image>().color = rarityColour * 1.25f;

        

        UIScreens[1].transform.localScale = Vector3.zero;
        FindObjectOfType<GameManager>().PlaySoundEffect(0);

        float t=0;
        while (t < 1)
        {
            t += Time.deltaTime*10;
            screen.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            screen.Find("Background").transform.localScale = Vector3.one * 100000;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        screen.transform.localScale = Vector3.one;
        screen.Find("Background").transform.localScale = Vector3.one;

        yield return false;
    }


    Vector3 mp1;
    Vector3 mp2;

    [SerializeField] float PreviewCamSpinSpeed;
    [SerializeField] GameObject PreviewCamObject;
    GameObject PreviewObject = null;
    [SerializeField] Transform PreviewObjectParent;
    [SerializeField] Material DefaultMaterial;

    public void PurchaseDisplaying()
    {
        string type = EventsInQueue[0].CustomisationReceived > -1 ? "Customisation" : "Block";
        if (type == "Customisation")
        {
            bool success = false;
            string reason = "";

            GetComponent<CustomisationManager>().BuyItem(EventsInQueue[0].CustomisationReceived, out success, out reason);

            if (success)
            {
                AddEvent("ItemGot", EventsInQueue[0].CustomisationReceived, -1, -1, -1, -1, false);
                EventFinished();
            }
            Debug.Log(reason);
        }
        if (type == "Block")
        {
            bool Success = FindObjectOfType<BlockStorage>().BuyBlock(EventsInQueue[0].BlockUnlockedID, EventsInQueue[0].BlockGotAmount);
            if (Success)
            {
                AddEvent("ItemGot", -1, -1, EventsInQueue[0].BlockUnlockedID, EventsInQueue[0].BlockGotAmount, -1, false);
                EventFinished();
            }
        }
    }

    IEnumerator RunShopPreview()
    {
        string type = EventsInQueue[0].CustomisationReceived > -1 ? "Customisation" : "Block";

        GameObject screen = UIScreens[2];

        string dName = "";
        string description = "";
        string CostType = "";
        int Cost = 0;
        GameObject preview = null;
        CustomisationManager.CustomisationRarity rarity = CustomisationManager.CustomisationRarity.Common;

        if(type == "Customisation")
        {
            CustomisationManager cM = GetComponent<CustomisationManager>();
            Customisation c = cM.AllCustomisations[EventsInQueue[0].CustomisationReceived];
            dName = c.DisplayName;
            description = c.Description;
            CostType = c.PriceCubits > 0 ? "Cubit" : c.PriceAdventureCubits > 0 ? "Adbit" : "Golden Cubit";
            Cost = c.PriceCubits > 0 ? c.PriceCubits : c.PriceAdventureCubits > 0 ? c.PriceAdventureCubits : c.PriceGoldenCubits;
            preview = c.InWorldModel;
            rarity = c.Rarity;
        }
        if(type == "Block")
        {
            BlockStorage bS = GetComponent<BlockStorage>();
            BuildingBlock c = bS.blocks[EventsInQueue[0].BlockUnlockedID];
            int amount = EventsInQueue[0].BlockGotAmount;
            dName = c.DisplayName;
            description = c.Description;
            CostType = c.PriceCubits > 0 ? "Cubit" : c.PriceAdventureCubits > 0 ? "Adbit" : "Golden Cubit";
            Cost = c.PriceCubits > 0 ? c.PriceCubits* amount : c.PriceAdventureCubits > 0 ? c.PriceAdventureCubits* amount : c.PriceGoldenCubits* amount;
            preview = c.buildingPrefab;
            rarity = c.Rarity;
        }

        if (PreviewObject != null)
        {
            Destroy(PreviewObject);
        }
        PreviewObject = Instantiate(preview, PreviewObjectParent);
        PreviewObject.transform.localScale = Vector3.one;
        PreviewObject.transform.localRotation = Quaternion.identity;
        PreviewObject.transform.localPosition = Vector3.zero;
        if (PreviewObject.transform.Find("paintable"))
        {
            MeshRenderer[] paint = PreviewObject.transform.Find("paintable").GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < paint.Length; i++)
            {
                paint[i].material = DefaultMaterial;
            }
        }

        GameManager gm = GetComponent<GameManager>();

        screen.transform.Find("DisplayName").GetComponent<Text>().text = dName;
        screen.transform.Find("DisplayDescription").GetComponent<Text>().text = description;
        screen.transform.Find("CubitLogo").GetComponent<Image>().sprite = gm.ReturnCubitSpriteFromName(CostType);

        screen.transform.Find("CostText").GetComponent<Text>().color = gm.ReturnCubitColorFromName(CostType)*1.5f;
        screen.transform.Find("CostText").GetComponent<Text>().text = Cost.ToString();

        screen.transform.Find("CubitGlow").GetComponent<Image>().color = gm.ReturnCubitColorFromName(CostType);
        screen.transform.Find("Background").Find("RarityGlow").GetComponent<Image>().color = GetComponent<CustomisationManager>().GetColorFromRarity(rarity);

        screen.transform.Find("AmountText").GetComponent<Text>().text = EventsInQueue[0].BlockGotAmount > 0 ? "x" + EventsInQueue[0].BlockGotAmount : "";

        UIScreens[2].SetActive(true);

        float t = 0;
        Vector3 lastRot = Vector3.zero;
        Vector3 rotTo = new Vector3(20, 150, 0);
        PreviewCamObject.transform.rotation = Quaternion.Euler(rotTo);
        while (running)
        {
            t += Time.deltaTime;
            rotTo += Time.deltaTime * new Vector3(0, 15, 0);
            if (Input.GetMouseButton(0))
            {
                t = 0;
                if (mp1 == new Vector3(0, 0, 0))
                {
                    mp1 = Input.mousePosition;
                }
                mp2 = Input.mousePosition;
                float xdifference = mp1.x - mp2.x;
                float ydifference = mp1.y - mp2.y;
                PreviewCamObject.transform.Rotate(new Vector3(ydifference * Time.deltaTime * PreviewCamSpinSpeed, -xdifference * Time.deltaTime * PreviewCamSpinSpeed, 0));
                mp1 = mp2;
                Vector3 rot = new Vector3(PreviewCamObject.transform.localEulerAngles.x, PreviewCamObject.transform.localEulerAngles.y, 0);
                PreviewCamObject.transform.rotation = Quaternion.Euler(rot);
                rot.x = Mathf.Clamp(rot.x, -50, 50);
                lastRot = rot;
                rotTo.y = rot.y;

            }
            else
            {
                Vector3 rot = Vector3.Lerp(lastRot, rotTo, t * 10);
                PreviewCamObject.transform.rotation = Quaternion.Lerp(PreviewCamObject.transform.rotation, Quaternion.Euler(rot), Time.deltaTime*2.5f);
            }

            if (Input.GetMouseButtonUp(0))
            {
                mp1 = new Vector3(0, 0, 0);
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return false;
    }

    [Header("Crate Open")]
    int CrackedAmount = 0;
    bool CrateRunning = false;
    public Color CrateBackgroundColour;
    int CrateLastOpened = -1;
    IEnumerator RunCrateOpen()
    {
        CustomisationManager cM = GetComponent<CustomisationManager>();
        BlockStorage bS = GetComponent<BlockStorage>();
        int CrateID = EventsInQueue[0].CharacterUnlockedID;
        int ItemRarity = EventsInQueue[0].ItemRarity;
        Crate crate = FindObjectOfType<CrateManager>().AllCrates[CrateID];
        GameObject panel = UIScreens[3];
        panel.SetActive(true);
        CrateRunning = true;
        Color startColour = CrateBackgroundColour;

        GameObject crateObject = Instantiate(crate.CrateObject);
        crateObject.transform.SetParent(panel.transform.Find("CrateSpawn").Find("CrateObject").Find("CrateParent"));
        crateObject.transform.localScale = Vector3.one;
        crateObject.transform.localPosition = Vector3.zero;
        crateObject.transform.localEulerAngles = Vector3.zero;


        CrackedAmount = 0;
        int lastCrack = 0;
        int lastanimationchosen = 0;
        int lastaudiochosen = 0;

        CrateLastOpened = CrateID;
        while (CrackedAmount < 5)
        {
            if(lastCrack != CrackedAmount)
            {
                int chosen = lastanimationchosen;
                while (chosen == lastanimationchosen)
                {
                    chosen = Random.Range(1, 5);
                }
                lastanimationchosen = chosen;

                int audioChosen = lastaudiochosen;
                while (audioChosen == lastaudiochosen)
                {
                    audioChosen = Random.Range(5, 11);
                }
                lastaudiochosen = audioChosen;

                if(crate.SpecialAudioClip != null)
                {
                    GetComponent<GameManager>().PlaySoundClip(crate.SpecialAudioClip, crate.SpecialAudioVolume);
                }

                panel.transform.Find("CrateSpawn").Find("CrateObject").GetComponent<Animator>().SetTrigger(chosen.ToString());
                GetComponent<GameManager>().PlaySoundEffect(audioChosen);
                lastCrack = CrackedAmount;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (EventsInQueue[0].ItemRarity < 5)
        {
            GetComponent<GameManager>().PlaySoundEffect(11);
        }
        else
        {
            GetComponent<GameManager>().PlaySoundEffect(12);
        }
        float scale = 0;
        while (scale < 1)
        {
            scale += Time.deltaTime * 5;
            panel.transform.Find("OpenedFlash").localScale = Vector3.one * scale;
            CrackedAmount = 5;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        ItemWorldImageRenderManager renderer = GetComponent<ItemWorldImageRenderManager>();

        int UnlockedID = EventsInQueue[0].CustomisationReceived > -1 ? EventsInQueue[0].CustomisationReceived : EventsInQueue[0].BlockUnlockedID;
        string type = EventsInQueue[0].CustomisationReceived > -1 ? "Customisation" : "Block";
        
        string ObjectDisplayName = type == "Customisation" ? cM.AllCustomisations[UnlockedID].DisplayName : bS.blocks[UnlockedID].DisplayName;
        string ObjectType = type == "Customisation" ? cM.GetTypeFromItem(UnlockedID) : bS.GetTypeFromBlock(UnlockedID);

        CustomisationManager.CustomisationRarity rarity = cM.GetRarityFromID(ItemRarity);
        string RarityDisplay = rarity.ToString();

        Texture ItemImage = type == "Customisation" ? renderer.CustomisationItemTextures[UnlockedID] : renderer.BlockItemTextures[UnlockedID];
        //show crate stuff

        panel.transform.Find("GotInformation").gameObject.SetActive(true);
        panel.transform.Find("GotInformation").Find("ItemName").GetComponent<Text>().text = ObjectDisplayName;
        panel.transform.Find("GotInformation").Find("ItemDescription").GetComponent<Text>().text = RarityDisplay + " " + ObjectType;
        panel.transform.Find("GotInformation").Find("ItemAmount").GetComponent<Text>().text = type == "Customisation" ? "" : "x" + EventsInQueue[0].BlockGotAmount;
        panel.transform.Find("GotInformation").Find("ItemAmountImage").gameObject.SetActive(type != "Customisation");
        panel.transform.Find("CrateImage").gameObject.SetActive(false);
        panel.transform.Find("DisplayImage").gameObject.SetActive(true);
        panel.transform.Find("DisplayImage").GetComponent<RawImage>().texture = ItemImage;
        Color RarityColour = cM.GetColorFromRarity(rarity);

        panel.transform.Find("below").transform.Find("Background").GetComponent<Image>().color = RarityColour;

        panel.transform.Find("GotInformation").Find("ItemName").GetComponent<Text>().color = RarityColour * 2;
        panel.transform.Find("GotInformation").Find("ItemDescription").GetComponent<Text>().color = RarityColour*2f;

        panel.transform.Find("SpinGlowHighlight").GetComponent<Image>().color = RarityColour * 2;
        panel.transform.Find("SpinGlowHighlight").gameObject.SetActive(true);
        panel.transform.Find("SpinGlowWhite").gameObject.SetActive(true);
        panel.transform.Find("SpinGlowColour").GetComponent<Image>().color = RarityColour;
        RarityColour.a = 0.2f;


        if (GetComponent<CrateManager>().HasCrate(CrateID))
        {
            int amountGot = GetComponent<CrateManager>().AmountInInventory(CrateID);
            panel.transform.Find("buttons").transform.Find("Claim").gameObject.SetActive(true);
            panel.transform.Find("buttons").transform.Find("OtherwiseClaim").gameObject.SetActive(false);
            panel.transform.Find("buttons").transform.Find("OpenAnother").gameObject.SetActive(true);
            panel.transform.Find("buttons").transform.Find("OpenAnother").Find("label").GetComponent<Text>().text = "Open Another (" + amountGot + " left)";
            if (EventsInQueue[0].AlreadyOwned)
            {
                panel.transform.Find("buttons").transform.Find("AlreadyOwnedLabelNormal").gameObject.SetActive(false);
                panel.transform.Find("buttons").transform.Find("AlreadyOwnedAnother").gameObject.SetActive(true);
                Customisation item = cM.AllCustomisations[UnlockedID];
                string Currency = item.PriceCubits > 0 ? "Cubits" : item.PriceAdventureCubits > 0 ? "Adbit" : "Golden Cubit";
                int amount = (int)(item.PriceCubits * 0.25f + item.PriceAdventureCubits * 0.25f + item.PriceGoldenCubits * 0.5f);
                panel.transform.Find("buttons").transform.Find("AlreadyOwnedAnother").Find("Return").Find("amount").GetComponent<Text>().color = GetComponent<GameManager>().ReturnCubitColorFromName(Currency);
                panel.transform.Find("buttons").transform.Find("AlreadyOwnedAnother").Find("Return").Find("amount").GetComponent<Text>().text = amount.ToString();
                panel.transform.Find("buttons").transform.Find("AlreadyOwnedAnother").Find("Return").Find("cubitlogo").GetComponent<Image>().sprite = GetComponent<GameManager>().ReturnCubitSpriteFromName(Currency);
            }
        }
        else
        {
            panel.transform.Find("buttons").transform.Find("Claim").gameObject.SetActive(false);
            panel.transform.Find("buttons").transform.Find("OtherwiseClaim").gameObject.SetActive(true);
            panel.transform.Find("buttons").transform.Find("OpenAnother").gameObject.SetActive(false);
            if (EventsInQueue[0].AlreadyOwned)
            {
                panel.transform.Find("buttons").transform.Find("AlreadyOwnedLabelNormal").gameObject.SetActive(true);
                panel.transform.Find("buttons").transform.Find("AlreadyOwnedAnother").gameObject.SetActive(false);
                Customisation item = cM.AllCustomisations[UnlockedID];
                string Currency = item.PriceCubits > 0 ? "Cubits" : item.PriceAdventureCubits > 0 ? "Adbit" : "Golden Cubit";
                int amount = (int)(item.PriceCubits * 0.25f + item.PriceAdventureCubits * 0.25f + item.PriceGoldenCubits * 0.5f);
                panel.transform.Find("buttons").transform.Find("AlreadyOwnedLabelNormal").Find("Return").Find("amount").GetComponent<Text>().color = GetComponent<GameManager>().ReturnCubitColorFromName(Currency);
                panel.transform.Find("buttons").transform.Find("AlreadyOwnedLabelNormal").Find("Return").Find("amount").GetComponent<Text>().text = amount.ToString();
                panel.transform.Find("buttons").transform.Find("AlreadyOwnedLabelNormal").Find("Return").Find("cubitlogo").GetComponent<Image>().sprite = GetComponent<GameManager>().ReturnCubitSpriteFromName(Currency);
            }
        }


        panel.transform.Find("below").transform.Find("Checkers").GetComponent<Image>().color = RarityColour;
        yield return new WaitForSeconds(0.5f);

       
        float opacity = 1;
        while (opacity > 0)
        {
            Color c = panel.transform.Find("OpenedFlash").GetComponent<Image>().color;
            c.a = opacity;
            panel.transform.Find("OpenedFlash").GetComponent<Image>().color = c;
            opacity -= Time.deltaTime;
            CrackedAmount = 5;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        panel.transform.Find("OpenedFlash").localScale = Vector3.zero;


        while (!CrateDone)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        CrateDone = false;
        panel.transform.Find("GotInformation").gameObject.SetActive(false);
        CrateRunning = true;
        panel.transform.Find("CrateImage").gameObject.SetActive(true);
        panel.transform.Find("DisplayImage").gameObject.SetActive(false);
        panel.transform.Find("OpenedFlash").localScale = Vector3.zero;
        panel.transform.Find("OpenedFlash").GetComponent<Image>().color = Color.white;
        panel.transform.Find("below").transform.Find("Background").GetComponent<Image>().color = startColour;
        startColour.a = 0.075f;
        panel.transform.Find("below").transform.Find("Checkers").GetComponent<Image>().color = startColour;
        Color w = Color.white; w.a = 0.5f;
        panel.transform.Find("SpinGlowColour").GetComponent<Image>().color = w;
        panel.transform.Find("SpinGlowHighlight").gameObject.SetActive(false);
        panel.transform.Find("SpinGlowColour").gameObject.SetActive(false);
        panel.transform.Find("buttons").transform.Find("Claim").gameObject.SetActive(false);
        panel.transform.Find("buttons").transform.Find("OtherwiseClaim").gameObject.SetActive(false);
        panel.transform.Find("buttons").transform.Find("OpenAnother").gameObject.SetActive(false);
        panel.transform.Find("buttons").transform.Find("AlreadyOwnedLabelNormal").gameObject.SetActive(false);
        panel.transform.Find("buttons").transform.Find("AlreadyOwnedAnother").gameObject.SetActive(false);

        Destroy(crateObject);
        EventFinished();
        yield return false;
    }

    bool CrateDone = false;
    public void CrateClaim()
    {
        CrateDone = true;
    }

    public void OpenAnotherCrate()
    {
        GetComponent<CrateManager>().OpenCrateID(CrateLastOpened);
        CrateDone = true;
    }


    #region Main Menu

    public GameObject PauseMenu;
    public GameObject[] PausePanels; //0 - main, 1 - options, 2 - video, 3 - audio 
    public bool PauseMenuOpen;

    public void OpenPausePanel(int panel)
    {
        ClosePausePanels();
        PausePanels[panel].SetActive(true);
        PauseMenuOpen = true;
    }

    public void ClosePausePanels()
    {
        for (int i = 0; i < PausePanels.Length; i++)
        {
            PausePanels[i].SetActive(false);
        }
    }

    public void OpenPauseMenu()
    {
        PauseMenu.SetActive(true);
        OpenPausePanel(0);
    }

    public void ClosePauseMenu()
    {
        PauseMenu.SetActive(false);
        PauseMenuOpen = false;
    }

    public void PauseReturnToHub()
    {
        FindObjectOfType<GameManager>().ReturnToMainMenu();
        ClosePauseMenu();
    }

    public void PauseReturn()
    {
        ClosePauseMenu();
    }

    #region Options
    public void PauseOpenOptions()
    {
        OpenPausePanel(1);
    }

    public void PauseOpenVideo()
    {
        OpenPausePanel(2);
    }

    public Slider MainVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SFXVolumeSlider;
    public Slider OtherPlayerVolumeSlider;
    public void PauseOpenAudio()
    {
        OpenPausePanel(3);
        MainVolumeSlider.value = gm.MasterVolume;
        MusicVolumeSlider.value = gm.MusicVolume;
        SFXVolumeSlider.value = gm.SFXVolume;
        OtherPlayerVolumeSlider.value = gm.OtherPlayerVolume;
    }

    public void SaveAudioOptions()
    {
        gm.MasterVolume = MainVolumeSlider.value;
        gm.MusicVolume = MusicVolumeSlider.value;
        gm.SFXVolume = SFXVolumeSlider.value;
        gm.OtherPlayerVolume = OtherPlayerVolumeSlider.value;
        OpenPausePanel(1);
    }

    public void AudioSliderUpdated()
    {
        MainVolumeSlider.transform.Find("Amount").GetComponent<Text>().text = ((int)(MainVolumeSlider.value * 100)).ToString();
        MusicVolumeSlider.transform.Find("Amount").GetComponent<Text>().text = ((int)(MusicVolumeSlider.value * 100)).ToString();
        SFXVolumeSlider.transform.Find("Amount").GetComponent<Text>().text = ((int)(SFXVolumeSlider.value * 100)).ToString();
        OtherPlayerVolumeSlider.transform.Find("Amount").GetComponent<Text>().text = ((int)(OtherPlayerVolumeSlider.value * 100)).ToString();
    }


    #endregion

    public void PauseOpenCredits()
    {

    }

    public void PauseOpenCharities()
    {

    }

    public void PauseQuitGame()
    {
        FindObjectOfType<GameManager>().QuitGame();
    }
    #endregion

}


[System.Serializable]
public class AnimationEvent
{
    public string EventType;

    //character piece reveived
    public int CustomisationReceived; //General ID

    //character unlocked
    public int CharacterUnlockedID;

    //block unlocked
    public int BlockUnlockedID;
    public int BlockGotAmount;

    //crate information
    public int ItemRarity;
    public bool AlreadyOwned;
}
