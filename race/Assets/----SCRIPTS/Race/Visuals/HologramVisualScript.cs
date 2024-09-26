using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramVisualScript : MonoBehaviour
{
    public void FinishedAnimation()
    {
        Destroy(gameObject);
    }
}
