using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeaturedShopButton : MonoBehaviour
{
    string ShopButtonType;
    public int CustomisationID = -1;
    public int CharacterID = -1;
    public int BlockID = -1;
    public int BlockAmount = -1;

    [SerializeField] Text DisplayName;

    [SerializeField] Image CubitLogo;
    [SerializeField] Text CostText;

    [SerializeField] Image CostGlow;
    [SerializeField] Image RarityGlow;
    [SerializeField] Image DisplayImage;

    [SerializeField] GameObject ownedCover;

    bool AlreadyOwned = false;

    // Start is called before the first frame update
    void Start()
    {
        RefreshVisuals(); 
    }

    public void RefreshVisuals()
    {
        ShopButtonType = CustomisationID > -1 ? "Customisation" : CharacterID > -1 ? "Character" : "Block";
        string dName = "";
        string costType = "";
        int cost = 0;
        CustomisationManager.CustomisationRarity rarity = CustomisationManager.CustomisationRarity.Common;
        Sprite dImage = null;

        if(ShopButtonType == "Block")
        {
            BuildingBlock c = FindObjectOfType<BlockStorage>().blocks[BlockID];
            dName = c.DisplayName;
            costType = c.PriceCubits > 0 ? "Cubit" : c.PriceAdventureCubits > 0 ? "Adbit" : "Golden Cubit";
            cost = c.PriceCubits > 0 ? c.PriceCubits : c.PriceAdventureCubits > 0 ? c.PriceAdventureCubits : c.PriceGoldenCubits;
            rarity = c.Rarity;
            dImage = c.DisplayImage;
        }
        if (ShopButtonType == "Customisation")
        {
            Customisation c = FindObjectOfType<CustomisationManager>().AllCustomisations[CustomisationID];
            dName = c.DisplayName;
            costType = c.PriceCubits > 0 ? "Cubit" : c.PriceAdventureCubits > 0 ? "Adbit" : "Golden Cubit";
            cost = c.PriceCubits > 0 ? c.PriceCubits : c.PriceAdventureCubits > 0 ? c.PriceAdventureCubits : c.PriceGoldenCubits;
            rarity = c.Rarity;
            dImage = c.DisplayImage;
            AlreadyOwned = FindObjectOfType<CustomisationManager>().OwnedCustomisation(CustomisationID);
        }

        GameManager gm = FindObjectOfType<GameManager>();
        CustomisationManager cm = FindObjectOfType<CustomisationManager>();
        DisplayName.text = dName;
        DisplayName.color = cm.GetColorFromRarity(rarity)*1.5f;
        CubitLogo.sprite = gm.ReturnCubitSpriteFromName(costType);

        CostText.text = BlockAmount>0 ? (cost*BlockAmount).ToString() : cost.ToString();
        CostText.color = gm.ReturnCubitColorFromName(costType) * 2f;
        CostGlow.color = gm.ReturnCubitColorFromName(costType);
        RarityGlow.color = cm.GetColorFromRarity(rarity);
        DisplayImage.sprite = dImage;

        if (AlreadyOwned)
        {
            GetComponent<Button>().interactable = false;
            CostText.gameObject.SetActive(false);
            CostGlow.gameObject.SetActive(false);
            CubitLogo.gameObject.SetActive(false);
            ownedCover.SetActive(true);
        }

    }

    public void ButtonPressed()
    {
        FindObjectOfType<GameUIManager>().AddEvent("ShopPreview", CustomisationID, CharacterID, BlockID, BlockAmount, -1, false);
    }

    public void PointerEnter()
    {
        if(!AlreadyOwned)
        FindObjectOfType<GameManager>().PlaySoundEffect(3);
    }
}
