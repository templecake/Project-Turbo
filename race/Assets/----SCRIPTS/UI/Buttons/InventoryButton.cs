using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    public int id;
    
    [SerializeField] Text nameText;
    [SerializeField] Text amText;
    [SerializeField] Image Background;
    [SerializeField] Image Outline;
    [SerializeField] Image AmountGotRarityImage;
    [SerializeField] RawImage RawDisplayImage;

    CreationManager cm;
    private void Start()
    {
        cm = FindObjectOfType<CreationManager>();  
    }

    public void FixedUpdate()
    {
        Color w = Color.white;
        w.a = 0.2f;

        Color b = Color.black;
        b.a = 0.5f;

        Background.color = cm.blockCurrentlyPlacing == id ? w : b;
    }

    public void Refresh()
    {
        BlockStorage bs = GameObject.Find("GameManager").GetComponent<BlockStorage>();
        Color c = FindObjectOfType<CustomisationManager>().GetColorFromRarity(bs.blocks[id].Rarity);
        nameText.text = bs.blocks[id].name;
        nameText.color = c;
        Background.color = c;
        if (bs.amountInInventory[id] > 1)
        {
            amText.text = "x" + bs.amountInInventory[id].ToString();
            AmountGotRarityImage.gameObject.SetActive(true);
            AmountGotRarityImage.color = c;
        }
        else
        {
            amText.text = "";
            AmountGotRarityImage.gameObject.SetActive(false);
        }
       

        Outline.color = c; 

        RawDisplayImage.texture = FindObjectOfType<ItemWorldImageRenderManager>().BlockItemTextures[id];
    }

    public void Select()
    {
      GameObject.Find("CreationCanvas").GetComponentInChildren<CreationInventory>().ChangeSelected(id);
    }

    public void RightClicked()
    {
        cm.ShowInformation = !cm.ShowInformation;
    }

    public void MouseEnter()
    {
        if(!cm.BlocksToDisplay.Contains(id)) cm.BlocksToDisplay.Add(id); 
    }

    public void MouseExit()
    {
        if (cm.BlocksToDisplay.Contains(id)) cm.BlocksToDisplay.Remove(id);
        cm.ShowInformation = false;
    }
}
