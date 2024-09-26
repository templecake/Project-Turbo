using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string PlayerUsername;

    public int KartSelected;
    public List<Car> playersKart;

    public void SaveKart(Car kart)
    {
        playersKart.Add(kart);
    }

    public void SaveKartFromVariables(int[] blocks, Vector3[] positions, Vector3[] rotations, int[] materials, int seatColour, int seatSelected)
    {
        if (!(blocks.Length == positions.Length && rotations.Length == materials.Length && positions.Length == rotations.Length)) return;

        Car newCar = new Car();
        for (int i = 0; i < blocks.Length; i++)
        {
            newCar.blocks.Add(blocks[i]);
        }
        for (int i = 0; i < positions.Length; i++)
        {
            newCar.positions.Add(positions[i]);
        }
        for (int i = 0; i < rotations.Length; i++)
        {
            newCar.rotations.Add(rotations[i]);
        }
        for (int i = 0; i < materials.Length; i++)
        {
            newCar.materials.Add(materials[i]);
        }
        newCar.seatColour = seatColour;
        newCar.SeatSelected=seatSelected;

        playersKart.Add(newCar);
    }

    public void RemoveKart(int at)
    {
        playersKart.RemoveAt(at);
    }

    public int Cubits;
    public int AdventureCubits;
    public int GoldenCubits;

    public int MultiplierPoints; 
    //lose 1 every 2 hours, after 12 hours since last one achieved. Every 5 increases multiplier.
    //earned through completing quests which cycle.
    //required for full multiplier
    //----1.25x----1.50x----1.75x----2.00x
    [Range(1f, 2f)] public float Multiplier;

    public void AddCubits(int amount)
    {
        Cubits += amount;
    }

    public bool HasCubits(int amount)
    {
        return Cubits >= amount;
    }

    public bool RemoveCubits(int amount)
    {
        if (HasCubits(amount))
        {
            Cubits -= amount;
            return true;
        }
        return false;
    }


    public void AddAdventureCubits(int amount)
    {
        AdventureCubits += amount;
    }

    public bool HasAdventureCubits(int amount)
    {
        return AdventureCubits >= amount;
    }

    public bool RemoveAdventureCubits(int amount)
    {
        if (HasAdventureCubits(amount))
        {
            AdventureCubits -= amount;
            return true;
        }
        return false;
    }


    public void AddGoldenCubits(int amount)
    {
        GoldenCubits += amount;
    }

    public bool HasGoldenCubits(int amount)
    {
        return GoldenCubits >= amount;
    }

    public bool RemoveGoldenCubits(int amount)
    {
        if (HasGoldenCubits(amount))
        {
            GoldenCubits -= amount;
            return true;
        }
        return false;
    }


}
