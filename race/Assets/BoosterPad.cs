using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterPad : MonoBehaviour
{
    public float BoostTime = 0.5f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CorrespondingKart>())
        {
            other.GetComponent<CorrespondingKart>().correspondingKart.GetComponent<KartController3>().SuperBoostHit(BoostTime);
        }
    }
}
