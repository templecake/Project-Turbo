using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandminePowerup : MonoBehaviour
{
    public GameObject OwnersKart;

    public float Duration;
    float t;
    void Update()
    {
        t += Time.deltaTime;
        if (t > Duration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CorrespondingKart>())
        {
            if (other.GetComponent<CorrespondingKart>().correspondingKart == OwnersKart) return;
            other.GetComponent<CorrespondingKart>().correspondingKart.GetComponent<KartController3>().ExplodeKart();
        }
    }
}
