using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Float : MonoBehaviour
{
    public Vector3 Direction;
    public float Speed;
    public float StartDelay;
    float timePassed;

    Vector3 StartPosition;

    private void Start()
    {
        StartPosition = transform.localPosition;
        timePassed = -StartDelay;
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed < 0) return;
        float scale = 1;
        scale = Mathf.Sin(timePassed * Mathf.PI * Speed);

        transform.localPosition = StartPosition + Direction * scale;
    }
}
