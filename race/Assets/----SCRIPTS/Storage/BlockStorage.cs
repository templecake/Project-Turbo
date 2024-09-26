using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Block
{
    public BuildingBlock block;
}

public class BlockStorage : MonoBehaviour
{
    public List<BuildingBlock> blocks;
    public List<int> amountInInventory;

    public List<GameObject> seats;
    public List<bool> seatUnlocked;

    public List<ColourItem> materials;
    public List<bool> materialUnlocked;

    public string GetTypeFromBlock(int block)
    {
        return GetNameFromBlockType(blocks[block].blockType);
    }

    public string GetNameFromBlockType(GameManager.BlockType bT)
    {
        switch (bT)
        {
            case GameManager.BlockType.Tyre:
                return "Tyre";

            case GameManager.BlockType.Engine:
                return "Engine";

            case GameManager.BlockType.Booster:
                return "Booster";

            default:
                return "Block";
        }
    }


    public void SubtractBlock(int block, int amount)
    {
        if(CanSubtractBlock(block, amount))
        amountInInventory[block]-=amount;
    }

    bool CanSubtractBlock(int block, int amount)
    {
        return (amountInInventory[block] >= amount);
    }

    public bool HasBlock(int block)
    {
        return amountInInventory[block] > 0;
    }

    public bool AddBlock(int block, int amount)
    {
        BuildingBlock thisBlock = blocks[block];
        Player p = GetComponent<Player>();
        amountInInventory[block] += amount;
        return true;
    }

    public bool BuyBlock(int block, int amount)
    {
        BuildingBlock thisBlock = blocks[block];
        Player p = GetComponent<Player>();
        if (p.RemoveCubits(thisBlock.PriceCubits * amount) && p.RemoveAdventureCubits(thisBlock.PriceAdventureCubits * amount) && p.RemoveGoldenCubits(thisBlock.PriceGoldenCubits * amount))
        {
            amountInInventory[block] += amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UnlockSeat(int seat)
    {
        seatUnlocked[seat] = true;
    }

    public void UnlockedMaterial(int mat)
    {
        materialUnlocked[mat] = true;
    }
}
