using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadySteadyRace : MonoBehaviour
{
    public int TextNum;
    public void CloseAllTexts(float f)
    {
        FindObjectOfType<RaceManager>().DisableStartNumbers(TextNum);
    }
}
