using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ColourItem : ScriptableObject
{
    public string DisplayName;
    public Material LinkedMaterial;
    public Sprite DisplayColour;
    public CustomisationManager.CustomisationRarity Rarity;
    public int Cost;
}
