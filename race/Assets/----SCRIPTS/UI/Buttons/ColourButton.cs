using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourButton : MonoBehaviour
{
    public int id;
    [SerializeField] Text MaterialNameText;
    [SerializeField] Image MaterialDisplayImage;
    [SerializeField] Image RarityGradientImage;
    [SerializeField] RawImage MaterialImage;

    public void Select()
    {
        GameObject.Find("CreationManager").GetComponentInChildren<CreationManager>().ChangeCurrentPaint(id);
    }

    public void Refresh()
    {
        BlockStorage bs = FindObjectOfType<BlockStorage>();
        MaterialNameText.text = bs.materials[id].DisplayName;
        //MaterialDisplayImage.sprite = bs.materials[id].DisplayColour;
        MaterialImage.texture = FindObjectOfType<ItemWorldImageRenderManager>().PaintItemTextures[id];
        RarityGradientImage.color = FindObjectOfType<CustomisationManager>().GetColorFromRarity(bs.materials[id].Rarity);
    }
}
