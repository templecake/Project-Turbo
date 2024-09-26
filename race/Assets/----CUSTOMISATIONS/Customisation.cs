using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Customisation : ScriptableObject
{
    public string DisplayName;
    public string Description;
    public Sprite DisplayImage;
    public CustomisationManager.CustomisationRarity Rarity;

    public GameObject InWorldModel;
    public string AnimationPrefix;
    public int AnimationID;

    public bool Customisable;

    public int PriceCubits;
    public int PriceAdventureCubits;
    public int PriceGoldenCubits;

    public int GeneralID;
}
