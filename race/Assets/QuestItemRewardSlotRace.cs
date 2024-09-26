using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestItemRewardSlotRace : MonoBehaviour
{
    public int BlockID = -1;
    public int BlockAmount;
    public int CustomisationID = -1;
    public int CrateID = -1;

    public RawImage DisplayImage;
    public Text AmountText;
    public Image AmountBack;

    float scale = 0;
    private void Update()
    {
        if (scale < 1)
        {
            scale += Time.deltaTime * 10;
            transform.localScale = Vector3.one * scale;
        }
        transform.localScale = Vector3.one;
    }

    public void StartUp()
    {
        CustomisationManager cm = FindObjectOfType<CustomisationManager>();
        BlockStorage bs = FindObjectOfType<BlockStorage>();
        ItemWorldImageRenderManager renderer = FindObjectOfType<ItemWorldImageRenderManager>();


        if (BlockID > -1)
        {
            BuildingBlock thisblock = bs.blocks[BlockID];
            Color rarityColour = cm.GetColorFromRarity(thisblock.Rarity);
            DisplayImage.texture = renderer.BlockItemTextures[BlockID];
            AmountText.text = BlockAmount.ToString();
            AmountBack.color = rarityColour;
        }  
        else if(CustomisationID > -1)
        {
            AmountText.gameObject.SetActive(false);
            AmountBack.gameObject.SetActive(false);
            DisplayImage.texture = renderer.CustomisationItemTextures[CustomisationID];

        }
        else if(CrateID > -1)
        {
            AmountText.gameObject.SetActive(false);
            AmountBack.gameObject.SetActive(false);
            DisplayImage.texture = renderer.CrateItemTextures[CrateID];
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
