using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class AnimationStyle : ScriptableObject
{
    public string DisplayName;
    public string Description;
    public Sprite DisplayImage;
    public CustomisationManager.CustomisationRarity Rarity;

    public string AnimationPrefix;

    public float PriceCubits;
    public float PriceAdventureCubits;
}
                                                                                                   