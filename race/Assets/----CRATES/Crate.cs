using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Crate : ScriptableObject
{
    public string CrateDisplayName;
    public string CrateType; //Block, Customisation
    public CustomisationManager.CustomisationRarity CrateRarity;
    public GameObject CrateObject;
    public AudioClip SpecialAudioClip;
    public float SpecialAudioVolume;

    public float CommonChance;
    public float RareChance;
    public float EpicChance;
    public float MythicChance;
    public float DivineChance;
    public float VintageChance;

    [Header("CUSTOMISATIONS")]
    public int[] CommonCustomisationIDs;
    public int[] RareCustomisationIDs;
    public int[] EpicCustomisationIDs;
    public int[] MythicCustomisationIDs;
    public int[] DivineCustomisationIDs;
    public int[] VintageCustomisationIDs;

    [Header("BLOCKS")]
    public int[] CommonBlockIDs;
    public int[] RareBlockIDs;
    public int[] EpicBlockIDs;
    public int[] MythicBlockIDs;
    public int[] DivineBlockIDs;
    public int[] VintageBlockIDs;

    [Header("AMOUNTS")]
    public int[] CommonBlockAmounts;
    public int[] RareBlockAmounts;
    public int[] EpicBlockAmounts;
    public int[] MythicBlockAmounts;
    public int[] DivineBlockAmounts;
    public int[] VintageBlockAmounts;
}
