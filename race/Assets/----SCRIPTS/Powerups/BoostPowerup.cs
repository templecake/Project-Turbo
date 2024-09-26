using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPowerup : MonoBehaviour
{
    void Start()
    {
        TrackManager tm = GameObject.FindGameObjectWithTag("TrackManager").GetComponent<TrackManager>();
        PowerUpSettings pus = GetComponent<PowerUpSettings>();
        tm.carsInLobby[pus.PowerUpOwner].ingameKart.GetComponent<KartController>().ExtraBoostTime += 2;
        Destroy(gameObject);
    }

}
