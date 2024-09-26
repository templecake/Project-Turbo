using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapeFlow : MonoBehaviour
{
    public float CurrentSpeed;

    Vector3 positionLastFrame;
    public Vector3 RotateStationary;
    public Vector3 RotateFull;
    public float RotateDropoff;
    float RotateAmount;

    private void Start()
    {
        positionLastFrame = transform.position;
    }
    void Update()
    {
        CurrentSpeed = Vector3.Distance(transform.position, positionLastFrame) / Time.deltaTime;
        positionLastFrame = transform.position;

        RotateAmount = Mathf.Lerp(RotateAmount, CurrentSpeed / RotateDropoff, Time.deltaTime*3);
        RotateAmount = Mathf.Clamp(RotateAmount, 0, 1);
        transform.localEulerAngles = Vector3.Lerp(RotateStationary, RotateFull, RotateAmount);
    }
}
