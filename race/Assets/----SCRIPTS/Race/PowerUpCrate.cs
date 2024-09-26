using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCrate : MonoBehaviour
{
    [SerializeField] float Cooldown;
    [SerializeField] float radius;
    float t;

    bool active=true;

    [SerializeField] GameObject DisplayObj;

    private void Update()
    {
        t += Time.deltaTime;
        active = t >= Cooldown;
        DisplayObj.SetActive(active);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        if (other.GetComponent<CorrespondingKart>())
        {
            active = false;
            t = 0;
        }
    }
}
