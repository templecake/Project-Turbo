using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterSet : ScriptableObject
{
    public string DisplayName;
    public string Description;
    public Sprite DisplayImage;
    public CustomisationManager.CustomisationRarity Rarity;

    public int IncludedHead;
    public int IncludedBody;
    public int IncludedLegs;
    public int IncludedAnimationStyle;

    public float PriceCubits;
    public float PriceAdventureCubits;
    public float DisplayDiscount;

    public Color unlockBackgroundColour;
    public Color unlockStreakColour;
    public Color unlockTextColour;
    public Color unlockBarColourMain;
    public Color unlockBarColourAccent;
    public Sprite CharacterImage;
}
