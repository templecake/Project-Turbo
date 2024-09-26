using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewKart : MonoBehaviour
{
    public float weight;
    public float speed;
    public float acceleration;
    public float steering;

    public Vector3 steeringOffset;

    private Rigidbody kartRigidbody;
    private float currentSpeed;
    private float currentSteering;

    void Start()
    {
        kartRigidbody = GetComponent<Rigidbody>();
    }

    [SerializeField] float accelerationInput = 0;
    [SerializeField] Vector3 currentVelocity = Vector3.zero;

    void Update()
    {
        Drive();
        Steer();
        Drift();
    }

    void Drive()
    {
        // Acceleration input
        accelerationInput = Input.GetAxis("Vertical");
        // Steering input
        

        currentSpeed += accelerationInput * acceleration * Time.deltaTime;
        if (accelerationInput == 0) currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 5);
        currentSpeed = Mathf.Clamp(currentSpeed, -5f, speed);
        currentVelocity = currentSpeed * transform.forward;

        kartRigidbody.velocity = currentVelocity;
    }

    [SerializeField] float steeringInput;
    float steerAmount;
    void Steer()
    {
        steerAmount = Drifting ? steering : steering * 1.5f;
       steeringInput = Input.GetAxis("Horizontal");

        transform.RotateAround(transform.position + steeringOffset, Vector3.up, steeringInput * steering * Time.deltaTime);
    }

    [SerializeField] bool Drifting = false;
    [SerializeField] float OutwardsDriftForce;
    void Drift()
    {
        Drifting = Input.GetKey(KeyCode.LeftShift);

        if (Drifting)
        {
            float outForce = OutwardsDriftForce * currentSpeed / speed;

            kartRigidbody.velocity = kartRigidbody.velocity + transform.right * outForce * steeringInput;
        }
    }
}
