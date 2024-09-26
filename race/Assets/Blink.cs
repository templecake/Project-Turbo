using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    public float BlinkPower;
    public float BlinkSpeed;
    public float MinSize;

    float TimePassed = 0;
    void Update()
    {
        TimePassed += Time.deltaTime; 
        float val = BlinkSpeed * Mathf.PI * (TimePassed-.5f);
        float scale = 1 - Mathf.Pow(Mathf.Sin(val), 2*BlinkPower);
        scale = Mathf.Clamp(scale, MinSize, 1);
        Vector3 size = Vector3.one; size.y = scale;
        transform.localScale = size;
    }
}
