using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuildingBlock : ScriptableObject
{
    public GameObject buildingPrefab;
    public GameManager.BlockType blockType;
    public int placementSound;
    public string DisplayName;
    public string Description;
    public Sprite DisplayImage;
    public AudioClip BlockAudio;
    public CustomisationManager.CustomisationRarity Rarity;

    public float SizeX;
    public float SizeY;
    public float SizeZ;
    public bool EvenSized;
    public Vector3 blockOffset;
    public bool CanBeRotated = true;

    public bool canBeColoured;
    public Material baseMaterial;

    public int PriceCubits;
    public int PriceAdventureCubits;
    public int PriceGoldenCubits;

    [Header("General Stats")]
    public float Weight;

    [Header("Engine Stats")]
    public float MaxSpeed;
    public float Acceleration;

    [Header("Booster Stats")]
    public float BoostCapacity;
    public float BoostMaxSpeed;

    [Header("Wheel Stats")]
    public float SteerSpeed;


}

