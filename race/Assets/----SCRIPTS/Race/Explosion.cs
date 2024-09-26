using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    float t = 0;
    void Start()
    {
        transform.Find("yellow").GetComponent<ParticleSystem>().Play();
        transform.Find("orange").GetComponent<ParticleSystem>().Play();
        transform.Find("red").GetComponent<ParticleSystem>().Play();
        transform.Find("smoke").GetComponent<ParticleSystem>().Play();
    }

    private void Update()
    {
        t += Time.deltaTime;

        if (t > 30)
        {
            Destroy(gameObject);
        }
        else if (t > 5)
        {
            transform.Find("smoke").GetComponent<ParticleSystem>().Stop();
        }
        else if (t > .5f)
        {
            transform.Find("smoke").GetComponent<ParticleSystem>().Play();
        }
        
    }
}
