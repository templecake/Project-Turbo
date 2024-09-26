using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{

    private void OnEnable()
    {
        timetoclose = 0; 
    }

    float time;
    float timetoclose = 0;

    void Update()
    {
        time += Time.deltaTime;
        timetoclose += Time.deltaTime;
        if (time > 0.05f)
        {
            //transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
        }

        if (timetoclose >= 0.5f)
        {
            gameObject.SetActive(false);
        }
    }
}
