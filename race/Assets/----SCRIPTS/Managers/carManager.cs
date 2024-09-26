using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carManager : MonoBehaviour
{
    InputManager im;

    public float Weight;
    public float Accelaration;
    public float TopSpeed;
    [SerializeField] float Speed;
    public float TurnPower;
    public int Boost;


  

    [SerializeField] GameObject body;
    [SerializeField] GameObject driveDirObject;
    [SerializeField] GameObject turnDirObject;

   

    [SerializeField] float drive;
    [SerializeField] float turn;

    [SerializeField] GameObject[] wheels;
    [SerializeField] float wheelTurnDisplay;

    [SerializeField] float JumpPower;
    private void Start()
    {
        im = GameObject.Find("GameManager").GetComponent<InputManager>();
    }

    private void Update()
    {
        GetDriveInput();

        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].transform.localRotation = Quaternion.Euler(0, turn * wheelTurnDisplay, 0);
        }
        
    }

    private void FixedUpdate()
    {
        DriveForce();
    }

    void GetDriveInput()
    {

        float inp = 0;
        float turnin = 0;
        if (im.controller)
        {
            inp = -im.input_LTrigger + im.input_RTrigger;
            inp /= 2;
            turnin = im.L_XAxis;
        }
        else
        {
            inp = Input.GetAxis("Vertical");
            turnin = Input.GetAxis("Horizontal");
        }

        if (inp > 0.1f || inp < -0.1f)
        {
            drive += inp * Time.deltaTime;
        }
        else
        {
            if (drive > 0.005f)
            {
                drive -= Time.deltaTime * 0.5f;
            }
            else if (drive < -0.005f)
            {
                drive += Time.deltaTime * 0.5f;
            }
        }

        if(turnin > 0.1 || turnin < -0.1f)
        {
            turn += turnin * Time.deltaTime * 2;
        }
        else
        {
            if (turn > 0.05f)
            {
                turn -= Time.deltaTime;
            }
            else if(turn < -0.05f)
            {
                turn += Time.deltaTime;
            }
        }

        drive = Mathf.Clamp(drive, -0.5f, 1f);
        turn = Mathf.Clamp(turn, -1, 1);
    }

    void DriveForce()
    {
        Vector3 driveDir = driveDirObject.transform.position - body.transform.position;
        driveDir.Normalize();
        

        Vector3 force = drive * driveDir * Accelaration;
        //GetComponent<Rigidbody>().AddForce(force, ForceMode.Acceleration);


        Vector3 turnDir = turnDirObject.transform.position - body.transform.position;
        Vector3 torque = new Vector3(0, turn * TurnPower, 0);
        torque = turnDir * turn * TurnPower;
        //GetComponent<Rigidbody>().AddTorque(torque, ForceMode.Force);

        transform.position += force*Time.fixedDeltaTime;
        float turnLerp = Mathf.Lerp(drive*2, 0.05f, 1f);
        turnLerp = drive < 0 ? -turnLerp : turnLerp;
        transform.Rotate(torque*Time.fixedDeltaTime * drive);
    }

    
}
