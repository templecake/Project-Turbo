using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomisationSlot : MonoBehaviour
{
    public int CustomisationID; 
    public int GeneralCustomisationID;

    public RawImage DisplayImage;
    public Text DisplayText;

    public void RefreshSlot()
    {
        CustomisationManager cM = FindObjectOfType<CustomisationManager>();

        DisplayImage.texture = cM.GetTextureFromItem(GeneralCustomisationID);
        DisplayText.text = cM.AllCustomisations[GeneralCustomisationID].DisplayName;
    }

    public void SlotPressed()
    {
        FindObjectOfType<RealWorldMainMenu>().SelectCustomisation(CustomisationID);
    }
}
