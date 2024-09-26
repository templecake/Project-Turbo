using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public Vector3 rotate;
    public bool pulse;
    public bool boomerang;
    public float pulseSpeed;
    float timePassed;

    public bool LocalRotate;

    Vector3 startRotaion;

    private void Start()
    {
        startRotaion = transform.eulerAngles;
    }

    void Update()
    {
        timePassed += Time.deltaTime;
        float scale = 1;
        if (pulse) scale = Mathf.Clamp(Mathf.Abs(Mathf.Sin(timePassed * Mathf.PI * pulseSpeed)), 0.25f, 1);
        transform.Rotate(rotate * Time.deltaTime * scale);
        if (boomerang)
        {
            scale = Mathf.Sin(timePassed * Mathf.PI * pulseSpeed);
            if (LocalRotate)
            transform.localEulerAngles = startRotaion + rotate * scale;
            else
            transform.eulerAngles = startRotaion + rotate * scale;
        }
    }
}
