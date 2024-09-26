using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperJumpZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CorrespondingKart>())
        {
            other.GetComponent<CorrespondingKart>().correspondingKart.GetComponent<KartController3>().CanSuperJump = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CorrespondingKart>())
        {
            other.GetComponent<CorrespondingKart>().correspondingKart.GetComponent<KartController3>().CanSuperJump = false;
        }
    }
}
