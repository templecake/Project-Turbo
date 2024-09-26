using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CustomisationManager : MonoBehaviour
{
    [Header("Storage")]
    public List<Material> Colours;

    public Customisation[] AllCustomisations;

    public Customisation[] AnimationStyles;
    public bool[] AnimationStyleOwned;

    public Customisation[] Heads;
    public bool[] HeadOwned;

    public Customisation[] Bodies;
    public bool[] BodyOwned;

    public Customisation[] Legs;
    public bool[] LegsOwned;

    public CharacterSet[] CharacterSets;
    public bool[] CharacterSetOwned;

    [Header("Selected")]
    public int AnimationStyleSelected;
    public int HeadSelected;
    public int BodySelected;
    public int LegsSelected;

    public int HeadColour;
    public int TopColour;
    public int LegsColour;

    #region Tiers
    public Color[] CustomisationTiers;
    public enum CustomisationRarity
    {
        Common, Rare, Epic, Mythic, Divine, Vintage
    }

    public int GetIdFromRarity(CustomisationRarity rarity)
    {
        switch (rarity)
        {
            case CustomisationRarity.Common:
                return 0;
            case CustomisationRarity.Rare:
                return 1;
            case CustomisationRarity.Epic:
                return 2;
            case CustomisationRarity.Mythic:
                return 3;
            case CustomisationRarity.Divine:
                return 4;
            case CustomisationRarity.Vintage:
                return 5;
        }
        return 0;
    }

    public CustomisationRarity GetRarityFromID(int id)
    {
        switch (id)
        {
            case 0:
                return CustomisationRarity.Common;
            case 1:
                return CustomisationRarity.Rare;
            case 2:
                return CustomisationRarity.Epic;
            case 3:
                return CustomisationRarity.Mythic;
            case 4:
                return CustomisationRarity.Divine;
            case 5:
                return CustomisationRarity.Vintage;

            default:
                return CustomisationRarity.Common;
        }
    }

    public Color GetColorFromRarity(CustomisationRarity CR)
    {
        switch (CR)
        {
            case CustomisationRarity.Common:
                return CustomisationTiers[0];
            case CustomisationRarity.Rare:
                return CustomisationTiers[1];
            case CustomisationRarity.Epic:
                return CustomisationTiers[2];
            case CustomisationRarity.Mythic:
                return CustomisationTiers[3];
            case CustomisationRarity.Divine:
                return CustomisationTiers[4];
            case CustomisationRarity.Vintage:
                return CustomisationTiers[5];
            default:
                return CustomisationTiers[0];
        }
    }
    #endregion

    #region Unlocking

    public void UnlockItem(int customisation)
    {
        for (int i = 0; i < Heads.Length; i++)
        {
            if (Heads[i].GeneralID == customisation)
            {
                HeadOwned[i] = true;
                return;
            }
        }
        for (int i = 0; i < Bodies.Length; i++)
        {
            if (Bodies[i].GeneralID == customisation)
            {
                BodyOwned[i] = true;
                return;
            }
        }
        for (int i = 0; i < Legs.Length; i++)
        {
            if (Legs[i].GeneralID == customisation)
            {
                LegsOwned[i] = true;
                return;
            }
        }
        for (int i = 0; i < AnimationStyles.Length; i++)
        {
            if (AnimationStyles[i].GeneralID == customisation)
            {
                AnimationStyleOwned[i] = true;
                return;
            }
        }
    }

    public void BuyItem(int customisation, out bool Successful, out string failureReason)
    {
        string customtype = "";
        int customid = -1;
        bool alreadyUnlocked = false;
        for (int i = 0; i < Heads.Length; i++)
        {
            if (Heads[i].GeneralID == customisation)
            {
                customtype = "head";
                customid = i;
                alreadyUnlocked = HeadOwned[i];
            }
        }
        for (int i = 0; i < Bodies.Length; i++)
        {
            if (Bodies[i].GeneralID == customisation)
            {
                customtype = "body";
                customid = i;
                alreadyUnlocked = BodyOwned[i];
            }
        }
        for (int i = 0; i < Legs.Length; i++)
        {
            if (Legs[i].GeneralID == customisation)
            {
                customtype = "legs";
                customid = i;
                alreadyUnlocked = LegsOwned[i];
            }
        }
        for (int i = 0; i < AnimationStyles.Length; i++)
        {
            if (AnimationStyles[i].GeneralID == customisation)
            {
                customtype = "animstyle";
                customid = i;
                alreadyUnlocked = AnimationStyleOwned[i];
            }
        }
        Player p = GetComponent<Player>();
        Customisation custom = AllCustomisations[customisation];
        bool success = true;
        Successful = true;
        failureReason = "";

        if (custom == null) { success = false;  Successful = false; failureReason = "No Item Found."; }
        if (!p.HasCubits(custom.PriceCubits)) { success = false; Successful = false; failureReason = "Cannot afford."; }
        if (!p.HasAdventureCubits(custom.PriceAdventureCubits)) { success = false; Successful = false; failureReason = "Cannot afford."; }
        if (!p.HasGoldenCubits(custom.PriceGoldenCubits)) { success = false; Successful = false; failureReason = "Cannot afford."; }
        if (alreadyUnlocked) { success = false; Successful = false; failureReason = "Already Unlocked."; }

        if(success == false) { return; }

        p.RemoveCubits(custom.PriceCubits);
        p.RemoveAdventureCubits(custom.PriceAdventureCubits);
        p.RemoveGoldenCubits(custom.PriceGoldenCubits);

        if (customtype == "head")
        {
            UnlockHead(customid);
        }
        if (customtype == "body")
        {
            UnlockBody(customid);
        }
        if (customtype == "legs")
        {
            UnlockLegs(customid);
        }
        if (customtype == "animstyle")
        {
            UnlockAnimationSet(customid);
        }
        Successful = true; 
    }

    public void UnlockAnimationSet(int animation)
    {
        AnimationStyleOwned[animation] = true;
    }

    public void UnlockHead(int head)
    {
        HeadOwned[head] = true;
    }

    public void UnlockBody(int top)
    {
        BodyOwned[top] = true;
    }

    public void UnlockLegs(int bottom)
    {
        LegsOwned[bottom] = true;
    }



    public void CheckCharactersOwned()
    {
        //check characters which are owned.
        for (int i = 0; i < CharacterSets.Length; i++)
        {
            if (!CharacterSetOwned[i])
            {
                bool characterFinished = CheckOwnedCharacter(i);
                CharacterSetOwned[i] = characterFinished;

                if (characterFinished)
                {
                    //queue character finished animation;
                }
            }

        }
    }

    public string GetTypeFromItem(int customisation)
    {
        for (int i = 0; i < Heads.Length; i++)
        {
            if (Heads[i].GeneralID == customisation)
            {
                return "Head";
            }
        }
        for (int i = 0; i < Bodies.Length; i++)
        {
            if (Bodies[i].GeneralID == customisation)
            {
                return "Body";
            }
        }
        for (int i = 0; i < Legs.Length; i++)
        {
            if (Legs[i].GeneralID == customisation)
            {
                return "Legs";
            }
        }
        for (int i = 0; i < AnimationStyles.Length; i++)
        {
            if (AnimationStyles[i].GeneralID == customisation)
            {
                return "Style";
            }
        }
        return "Customisation";
    }

    public RenderTexture GetTextureFromItem(int customisation)
    {
        return FindObjectOfType<ItemWorldImageRenderManager>().CustomisationItemTextures[customisation];
    }

    public bool CheckOwnedCharacter(int Character)
    {
        CharacterSet c = CharacterSets[Character];

        bool animationO = c.IncludedAnimationStyle == -1 ? true : AnimationStyleOwned[c.IncludedAnimationStyle];
        bool headO = c.IncludedHead == -1 ? true : HeadOwned[c.IncludedHead];
        bool bodyO = c.IncludedBody == -1 ? true : BodyOwned[c.IncludedBody];
        bool legsO = c.IncludedLegs == -1 ? true : LegsOwned[c.IncludedLegs];

        bool OwnedEverything = (animationO && headO && bodyO && legsO);

        return OwnedEverything;
    }

    public bool OwnedCustomisation(int customisation) 
    {
        for (int i = 0; i < Heads.Length; i++)
        {
            if (Heads[i].GeneralID == customisation)
            {
                return HeadOwned[i];
            }
        }
        for (int i = 0; i < Bodies.Length; i++)
        {
            if (Bodies[i].GeneralID == customisation)
            {
                return BodyOwned[i];
            }
        }
        for (int i = 0; i < Legs.Length; i++)
        {
            if (Legs[i].GeneralID == customisation)
            {
                return LegsOwned[i];
            }
        }
        for (int i = 0; i < AnimationStyles.Length; i++)
        {
            if (AnimationStyles[i].GeneralID == customisation)
            {
                return AnimationStyleOwned[i];
            }
        }
        return false;
    }

    public float GetPriceOfCharacterIfUnowned(int Character)
    {
        CharacterSet c = CharacterSets[Character];

        float owned = 0;
        float total = 4;

        bool animationO = AnimationStyleOwned[c.IncludedAnimationStyle];
        if(c.IncludedAnimationStyle != -1)
        {
            animationO = true;
            total += 1;
        }

        bool headO = HeadOwned[c.IncludedHead];
        if (c.IncludedHead != -1)
        {
            headO = true;
            total += 1;
        }

        bool bodyO = BodyOwned[c.IncludedBody];
        if (c.IncludedBody != -1)
        {
            bodyO = true;
            total += 1;
        }

        bool legsO = LegsOwned[c.IncludedLegs];
        if (c.IncludedLegs != -1)
        {
            legsO = true;
            total += 1;
        }

        if (animationO) owned += 1;
        if (headO) owned += 1;
        if (bodyO) owned += 1;
        if (legsO) owned += 1;

        float priceScale = (total - owned) / total;

        float value = c.PriceCubits > 0 ? c.PriceCubits : c.PriceAdventureCubits;

        return value * priceScale;
    }
    #endregion

    #region Selecting

        #region Change Item
    public void SelectHead(int head)
    {
        if(HeadOwned[head])
        HeadSelected = head;
    }
    public void SelectBody(int body)
    {
        if(BodyOwned[body])
        BodySelected = body;
    }
    public void SelectLegs(int legs)
    {
        if(LegsOwned[legs])
            LegsSelected = legs;
    }

    public void SelectAnimationStyle(int style)
    {
        if (AnimationStyleOwned[style])
            AnimationStyleSelected = style;
    }
    #endregion

        #region Change Colour
    public void SelectHeadColour(int colour)
    {
        HeadColour = colour;
    }

    public void SelectTopColour(int colour)
    {
        TopColour = colour;
    }

    public void SelectBottomColour(int colour)
    {
        LegsColour = colour;
    }
    #endregion

    #endregion

}
