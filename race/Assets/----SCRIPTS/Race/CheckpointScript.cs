using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    public GameObject[] NextCheckpoints;
    public int CheckpointNum;

    RaceManager rm;
    private void Start()
    {
        rm = FindObjectOfType<RaceManager>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CorrespondingKart>())
        {
            rm.PlayerHitCheckpoint(other.GetComponent<CorrespondingKart>().correspondingKart.gameObject, CheckpointNum);   
        }    
    }
}
