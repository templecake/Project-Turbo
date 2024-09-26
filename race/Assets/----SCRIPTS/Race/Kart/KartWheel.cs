using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KartWheel : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    public bool WheelFrontLeft;
    public bool WheelFrontRight;
    public bool WheelBackLeft;
    public bool WheelBackRight;

    public float SuspensionRestLength;
    public float SuspensionSpringTravel;
    public float SuspensionSpringStiffness;
    public float DamperStiffness;

    float MinLength;
    float MaxLength;
    float LastLength;

    [SerializeField] float SpringLength;
    float SpringVelocity;
    float SpringForce;

    float DamperForce;

    Vector3 SuspensionForce;
    Vector3 WheelVelocityLocal;
    float ForceX;
    float ForceY;

    public float WheelRadius;

    public float steerAngle;
    public float SteerTime;
    float wheelAngle;

    [SerializeField] Transform WheelDisplay;

    void Start()
    {
 //rb = GetComponent<Rigidbody>();

        MinLength = SuspensionRestLength - SuspensionSpringTravel;
        MaxLength = SuspensionRestLength + SuspensionSpringTravel;
    }

    private void Update()
    {
        wheelAngle = Mathf.Lerp(wheelAngle, steerAngle, Time.deltaTime * SteerTime);
        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y + wheelAngle, transform.localRotation.z);
        if (WheelDisplay != null) UpdateDisplayWheel();
    }

    void FixedUpdate()
    {
        if(Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, MaxLength + WheelRadius))
        {
            LastLength = SpringLength;
            SpringLength = hit.distance - WheelRadius;
            SpringLength = Mathf.Clamp(SpringLength, MinLength, MaxLength);

            SpringVelocity = (LastLength - SpringLength) / Time.fixedDeltaTime;

            SpringForce = SuspensionSpringStiffness * (SuspensionRestLength - SpringLength);
            DamperForce = DamperStiffness * SpringVelocity;
            SuspensionForce = (SpringForce+DamperForce) * transform.up;

            WheelVelocityLocal = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));

            ForceX = Input.GetAxis("Vertical") * SpringForce;
            ForceY = WheelVelocityLocal.x * SpringForce;


            rb.AddForceAtPosition(SuspensionForce+ (ForceX*transform.forward) + (ForceY*-transform.right) , hit.point);
        }
        else
        {
            SpringLength = Mathf.Lerp(SpringLength, 0, Time.deltaTime*10f);
        } 
    }

    void UpdateDisplayWheel()
    {
        WheelDisplay.transform.localRotation = Quaternion.Euler(0, Mathf.Rad2Deg * transform.localRotation.y + 90, 0);
        float positionY = SpringLength;
        //WheelDisplay.transform.localPosition = new Vector3(positionY, 0, 0);
    }
}
