using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CrateManager : MonoBehaviour
{
    public Crate[] AllCrates;
    public List<int> PlayerCrateInventory;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.K)) { AddCrate(Random.Range(0, AllCrates.Length)); }
        if(Input.GetKeyUp(KeyCode.O)) { OpenCrateID(PlayerCrateInventory[Random.Range(0, PlayerCrateInventory.Count)]); }
    }

    BlockStorage bS;
    CustomisationManager cM;

    private void Start()
    {
        bS = GetComponent<BlockStorage>();
        cM = GetComponent<CustomisationManager>();
    }

    #region Adding and Removing
    public void AddCrate(int crate)
    {
        PlayerCrateInventory.Add(crate);

        //SORT ALPHABETICALLY

        List<int> SortedIDs = PlayerCrateInventory.OrderBy(id => GetCrateNameFromID(id)).ToList();

        PlayerCrateInventory = SortedIDs;
    }   

    public int AmountInInventory(int ID)
    {
        int amount = 0;
        for (int i = 0; i < PlayerCrateInventory.Count; i++)
        {
            if (PlayerCrateInventory[i] == ID)
            {
                amount++;
            }
        }
        return amount;
    }

    public void OpenAllCrates()
    {
        StartCoroutine(OpenAllCoro());
    }

    IEnumerator OpenAllCoro()
    {
        while(PlayerCrateInventory.Count > 0)
        {
            OpenCrateID(PlayerCrateInventory[0]);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return false;
    }
    
    public bool HasCrate(int CrateID)
    {
        return PlayerCrateInventory.Contains(CrateID);
    }
    
    public void RemoveCrate(int CrateID)
    {
        PlayerCrateInventory.Remove(CrateID);
    }

    string GetCrateNameFromID(int CrateID)
    {
        return AllCrates[CrateID].CrateDisplayName;
    }
    #endregion

    #region Opening

    //already owned rewards player with 25% of item's worth.
    public void OpenCrateID(int CrateID)
    {
        Crate CrateOpening = AllCrates[CrateID];

        if (CrateOpening == null) return;

        if (!HasCrate(CrateID)) return;

        float RandomSelected = Random.Range(0, 1000);

        int rarity = -1;
        int ItemChosen = -1;
        int Amount = 0;

        if(RandomSelected <= CrateOpening.CommonChance * 10)
        {
            rarity = 0;
            //common item
          if (CrateOpening.CrateType == "Block")
          {
                int numChosen = Random.Range(0, CrateOpening.CommonBlockIDs.Length);
                ItemChosen = CrateOpening.CommonBlockIDs[numChosen];
                Amount = CrateOpening.CommonBlockAmounts[numChosen];
          }
          else
          {
                int numChosen = Random.Range(0, CrateOpening.CommonCustomisationIDs.Length);
                ItemChosen = CrateOpening.CommonCustomisationIDs[numChosen];
          }
        }
        else if (RandomSelected <= (CrateOpening.CommonChance+CrateOpening.RareChance) * 10)
        {
            rarity = 1;
            //rare item
            if (CrateOpening.CrateType == "Block")
            {
                int numChosen = Random.Range(0, CrateOpening.RareBlockIDs.Length);
                ItemChosen = CrateOpening.RareBlockIDs[numChosen];
                Amount = CrateOpening.RareBlockAmounts[numChosen];
            }
            else
            {
                int numChosen = Random.Range(0, CrateOpening.RareCustomisationIDs.Length);
                ItemChosen = CrateOpening.RareCustomisationIDs[numChosen];
            }
        }
        else if (RandomSelected <= (CrateOpening.CommonChance + CrateOpening.RareChance + CrateOpening.EpicChance) * 10)
        {
            rarity = 2;
            //epic item
            if (CrateOpening.CrateType == "Block")
            {
                int numChosen = Random.Range(0, CrateOpening.EpicBlockIDs.Length);
                ItemChosen = CrateOpening.EpicBlockIDs[numChosen];
                Amount = CrateOpening.EpicBlockAmounts[numChosen];
            }
            else
            {
                int numChosen = Random.Range(0, CrateOpening.EpicCustomisationIDs.Length);
                ItemChosen = CrateOpening.EpicCustomisationIDs[numChosen];
            }
        }
        else if (RandomSelected <= (CrateOpening.CommonChance + CrateOpening.RareChance + CrateOpening.EpicChance + CrateOpening.MythicChance) * 10)
        {
            rarity = 3;
            //mythic item
            if (CrateOpening.CrateType == "Block")
            {
                int numChosen = Random.Range(0, CrateOpening.MythicBlockIDs.Length);
                ItemChosen = CrateOpening.MythicBlockIDs[numChosen];
                Amount = CrateOpening.MythicBlockAmounts[numChosen];
            }
            else
            {
                int numChosen = Random.Range(0, CrateOpening.MythicCustomisationIDs.Length);
                ItemChosen = CrateOpening.MythicCustomisationIDs[numChosen];
            }
        }
        else if (RandomSelected <= (CrateOpening.CommonChance + CrateOpening.RareChance + CrateOpening.EpicChance + CrateOpening.MythicChance + CrateOpening.DivineChance) * 10)
        {
            rarity = 4;
            //divine item
            if (CrateOpening.CrateType == "Block")
            {
                int numChosen = Random.Range(0, CrateOpening.DivineBlockIDs.Length);
                ItemChosen = CrateOpening.DivineBlockIDs[numChosen];
                Amount = CrateOpening.DivineBlockAmounts[numChosen];
            }
            else
            {
                int numChosen = Random.Range(0, CrateOpening.DivineCustomisationIDs.Length);
                ItemChosen = CrateOpening.DivineCustomisationIDs[numChosen];
            }
        }
        else if (RandomSelected <= (CrateOpening.CommonChance + CrateOpening.RareChance + CrateOpening.EpicChance + CrateOpening.MythicChance + CrateOpening.DivineChance + CrateOpening.VintageChance) * 10)
        {
            rarity = 5;
            //vintage item
            if (CrateOpening.CrateType == "Block")
            {
                int numChosen = Random.Range(0, CrateOpening.VintageBlockIDs.Length);
                ItemChosen = CrateOpening.VintageBlockIDs[numChosen];
                Amount = CrateOpening.VintageBlockAmounts[numChosen];
            }
            else
            {
                int numChosen = Random.Range(0, CrateOpening.VintageCustomisationIDs.Length);
                ItemChosen = CrateOpening.VintageCustomisationIDs[numChosen];
            }
        }

        if (ItemChosen == -1) return;

        if(CrateOpening.CrateType == "Block")
        {
            BuildingBlock blockGot = bS.blocks[ItemChosen];
            bS.AddBlock(ItemChosen, Amount);
            FindObjectOfType<GameUIManager>().AddEvent("CrateOpen", -1, CrateID, ItemChosen, Amount, rarity, false);
            Debug.Log("Unboxed " + blockGot.DisplayName);
        }
        else
        {
            Customisation item = cM.AllCustomisations[ItemChosen];
            if (cM.OwnedCustomisation(ItemChosen))
            {
                Player p = FindObjectOfType<Player>();
                p.AddAdventureCubits((int)(item.PriceAdventureCubits * 0.25f));
                p.AddCubits((int)(item.PriceCubits * 0.25f));
                p.AddGoldenCubits((int)(item.PriceGoldenCubits * 0.5f));
                FindObjectOfType<GameUIManager>().AddEvent("CrateOpen", ItemChosen, CrateID, -1, -1, rarity, true);
                //reimburse player
            }
            else
            {
                cM.UnlockItem(ItemChosen);
                FindObjectOfType<GameUIManager>().AddEvent("CrateOpen", ItemChosen, CrateID, -1, -1, rarity, false);
            }
            Debug.Log("Unboxed " + item.DisplayName);
        }

        Debug.Log(rarity);
        RemoveCrate(CrateID);

    }

    #endregion
}
