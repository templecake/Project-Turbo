using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuPlayer : MonoBehaviour
{
    public bool Active = true;

    public float Speed;
    public float Acceleration;
    public float TurnSpeed;
    public Vector3 inputDirection;
    Rigidbody rb;

    public bool TurnMoveMode = false;

    public bool TouchingGround;
    float gravity;
    // Start is called before the first frame update
    float startSpeed;
    float startAccel;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startSpeed = Speed;
        startAccel= Acceleration;
    }

    // Update is called once per frame
    void Update()
    {
        Drive();
        CheckTouchingGround();
    }

    private void FixedUpdate()
    {
        if(Active)
        CameraMove();
    }

    Vector3 input;

    [SerializeField] KartDisplayer kD;
    float rotation;
    float rotationLerp;

    [SerializeField] float TyreRotation;
    [SerializeField] float tyreRot;
    [SerializeField] float tyreRotLerp;
    [SerializeField] GameObject tyreLookAtSpin;
    [SerializeField] GameObject tyreLookAt;

    [SerializeField] float SpinSpeed;
    float spin;

    void Drive()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Speed = startSpeed * 1.5f;
            Acceleration = Acceleration * 1.5f;
        }
        else
        {
            Speed = startSpeed;
            Acceleration = startAccel;
        }

        if (TurnMoveMode)
        {
            inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            float inputScale = input.z;
            inputScale *= 2;
            inputScale = inputScale > 0 ? Mathf.Clamp(inputScale, 0.25f, 1) : Mathf.Clamp(inputScale, -1, -.25f);
            rotation += inputDirection.x * Time.deltaTime * TurnSpeed * inputScale;
            rotationLerp = Mathf.Lerp(rotationLerp, rotation, Time.deltaTime * TurnSpeed);

            transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
            input = Vector3.Lerp(input, inputDirection, Time.deltaTime * Acceleration);

            tyreRot = input.x < 0 ? Mathf.Lerp(0, -TyreRotation, -input.x) : Mathf.Lerp(0, TyreRotation, input.x);
            tyreRot += 90f;
            tyreRotLerp = Mathf.Lerp(tyreRotLerp, tyreRot, 90 * Time.deltaTime);
            tyreLookAtSpin.transform.localRotation = Quaternion.Euler(new Vector3(0, tyreRotLerp, 0));

            Vector3 velocity = transform.forward * input.z * Speed;
            velocity.y = gravity;
            rb.velocity = velocity;


        }
        else
        {
            inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            inputDirection.Normalize();

            input = Vector3.Lerp(input, -inputDirection, Time.deltaTime * Acceleration);
            rb.velocity = input * Speed;
            if (input.magnitude != 0)
                transform.forward = Vector3.Lerp(transform.forward, input, TurnSpeed * Time.deltaTime);
        }

    }

    [SerializeField] float floorCheck;
    [SerializeField] LayerMask raycastMask;

    Quaternion lastMainRotation;

    void CheckTouchingGround()
    {
        GetComponent<Collider>().enabled = false;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, floorCheck, raycastMask))
        {
            if (hit.transform != transform)
            {
                transform.rotation = Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation;
                TouchingGround = true;
            }
        }
        else
        {
            TouchingGround = false;
        }
        GetComponent<Collider>().enabled = true;

        lastMainRotation = Quaternion.Lerp(lastMainRotation, transform.rotation, 10 * Time.deltaTime);
        transform.Find("main").rotation = lastMainRotation;

        if (!TouchingGround)
        {
            gravity -= 9.81f;
        }
        else
        {
            gravity = 0;
        }
    }

    [SerializeField] Vector3 cameraOffset;
    [SerializeField] float cameraFollowSpeed;
    void CameraMove()
    {
        Vector3 posTo = transform.position + cameraOffset;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, posTo, Time.fixedDeltaTime * cameraFollowSpeed);
        Camera.main.transform.LookAt(transform);
    }
}
