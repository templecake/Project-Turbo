using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class StandardShopButton : MonoBehaviour
{
    public int CustomisationID;
    public int BlockID;
    public int BlockAmount;

    public Image RarityGlow;
    public Image Checkers;
    public RawImage DisplayImage;
    public Text DisplayNameText;
    public Text CostText;
    public Image CubitLogo;

    public bool AlreadyOwned;

    private void Start()
    {
        RefreshVisuals();
    }

    public void RefreshVisuals()
    {
        string ShopButtonType = CustomisationID > -1 ? "Customisation" : "Block";

        string dName = "";
        string costType = "";
        int cost = 0;
        CustomisationManager.CustomisationRarity rarity = CustomisationManager.CustomisationRarity.Common;
        Texture dImage = null;

        if (ShopButtonType == "Block")
        {
            BuildingBlock c = FindObjectOfType<BlockStorage>().blocks[BlockID];
            dName = c.DisplayName;
            costType = c.PriceCubits > 0 ? "Cubit" : c.PriceAdventureCubits > 0 ? "Adbit" : "Golden Cubit";
            cost = c.PriceCubits > 0 ? c.PriceCubits : c.PriceAdventureCubits > 0 ? c.PriceAdventureCubits : c.PriceGoldenCubits;
            rarity = c.Rarity;
            dImage = FindObjectOfType<ItemWorldImageRenderManager>().BlockItemTextures[BlockID];
        }
        if (ShopButtonType == "Customisation")
        {
            Customisation c = FindObjectOfType<CustomisationManager>().AllCustomisations[CustomisationID];
            dName = c.DisplayName;
            costType = c.PriceCubits > 0 ? "Cubit" : c.PriceAdventureCubits > 0 ? "Adbit" : "Golden Cubit";
            cost = c.PriceCubits > 0 ? c.PriceCubits : c.PriceAdventureCubits > 0 ? c.PriceAdventureCubits : c.PriceGoldenCubits;
            rarity = c.Rarity;
            dImage = FindObjectOfType<ItemWorldImageRenderManager>().CustomisationItemTextures[CustomisationID];
            AlreadyOwned = FindObjectOfType<CustomisationManager>().OwnedCustomisation(CustomisationID);
        }

        GameManager gm = FindObjectOfType<GameManager>();
        CustomisationManager cm = FindObjectOfType<CustomisationManager>();
        DisplayNameText.text = dName;
        DisplayNameText.color = cm.GetColorFromRarity(rarity) * 1.5f;
        CubitLogo.sprite = gm.ReturnCubitSpriteFromName(costType);

        CostText.text = BlockAmount > 0 ? (cost * BlockAmount).ToString() : cost.ToString();
        CostText.color = gm.ReturnCubitColorFromName(costType) * 2f;
        RarityGlow.color = cm.GetColorFromRarity(rarity);
        Color col = RarityGlow.color;
        col.a = 0.05f;
        Checkers.color = col;
        DisplayImage.texture = dImage;
    }

    public void ButtonPressed()
    {
        FindObjectOfType<GameUIManager>().AddEvent("ShopPreview", CustomisationID, -1, BlockID, BlockAmount, -1, false);
    }

    public void PointerEnter()
    {
        if (!AlreadyOwned)
            FindObjectOfType<GameManager>().PlaySoundEffect(3);
    }
}
