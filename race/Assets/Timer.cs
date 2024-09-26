using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    float TimeSince = 0;
    private void Update()
    {
        TimeSince += Time.deltaTime;   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CorrespondingKart>())
        {
            Debug.Log($"Lap Completed: Time {TimeSince.ToString()}s");
            TimeSince = 0;
        }
    }
}
