using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool controller;
    [SerializeField] List<GameManager.ControllerType> selectedControllerType = new List<GameManager.ControllerType>();
    [SerializeField] List<string> controllerNames = new List<string>();

    public float input_LTrigger;
    public float input_RTrigger;
    public float input_DPad_H;
    public float input_DPad_V;
    public float L_XAxis;
    public float L_YAxis;
    public float R_XAxis;
    public float R_YAxis;
    public float input_Button1;
    public float input_Button2;
    public float input_Button3;
    public float input_Button4;
    public float input_RBumper;
    public float input_LBumper;

    public float CarTurn;
    public float CarPivot;
    public float CarAccelerate;
    public float CameraTurnX;
    public float CameraTurnY;
    public float CarDrift;
    public float CarBoost;
    public float CarPowerupUse;

    [SerializeField] bool debugInputs;
    [SerializeField] bool debugXbox;
    [SerializeField] bool debugPlaystation;

    private void Start()
    {
        UpdateControllers();
    }

    private void Update()
    {
        controller = controllerNames.Count > 0;
        if (Input.GetJoystickNames().Length != controllerNames.Count)
        {
            UpdateControllers();  
        }

        GetInputs();

        if (debugInputs) DebugInputs();
    }

    void UpdateControllers()
    {
        for (int i = 0; i < controllerNames.Count; i++)
        {
            controllerNames.RemoveAt(i);
        }

        for (int i = 0; i < selectedControllerType.Count; i++)
        {
            selectedControllerType.RemoveAt(i);
        }

        List<string> newControllerNames = new List<string>();
        List<GameManager.ControllerType> newControllerTypes = new List<GameManager.ControllerType>();

        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (Input.GetJoystickNames()[i] != "")
            {
                newControllerNames.Add(Input.GetJoystickNames()[i]);
                switch (Input.GetJoystickNames()[i])
                {
                    case "Wireless Controller":
                        newControllerTypes.Add(GameManager.ControllerType.Playstation);
                        break;

                    case "Controller (Xbox One For Windows)":
                        newControllerTypes.Add(GameManager.ControllerType.Xbox);
                        break;

                }
            }


        }

        controllerNames = newControllerNames;
        selectedControllerType = newControllerTypes;
        
    }

    void GetInputs()
    {
        if (controller)
        {
            switch (selectedControllerType[0])
            {
                case GameManager.ControllerType.Xbox:
                    //Debug.Log("Xbox Input");

                    input_RTrigger = Input.GetAxis("xbox_trigger") > 0 ? Input.GetAxis("xbox_trigger")*2 - 1 : -1;
                    input_LTrigger = Input.GetAxis("xbox_trigger") > 0 ? -1 : -(Input.GetAxis("xbox_trigger")*2+1);
                    input_DPad_H = Input.GetAxis("xbox_DPadH");
                    input_DPad_V = Input.GetAxis("xbox_DPadV");
                    L_XAxis = Input.GetAxis("Horizontal");
                    L_YAxis = Input.GetAxis("Vertical");
                    R_XAxis = Input.GetAxis("xbox_RStickH");
                    R_YAxis = Input.GetAxis("xbox_RStickV");
                    input_Button1 = Input.GetButton("xbox_AButton") ? 1 : 0;
                    input_Button2 = Input.GetButton("xbox_BButton") ? 1 : 0;
                    input_Button3 = Input.GetButton("xbox_XButton") ? 1 : 0;
                    input_Button4 = Input.GetButton("xbox_YButton") ? 1 : 0;
                    input_LBumper = Input.GetButton("xbox_LBumper") ? 1 : 0;
                    input_RBumper = Input.GetButton("xbox_RBumper") ? 1 : 0;

                    break;

                case GameManager.ControllerType.Playstation:
                    //Debug.Log("Playstation Input");
                    float psTriggers = -Input.GetAxis("ps_Ltrigger") + Input.GetAxis("ps_Rtrigger");
                    psTriggers /= 2;
                    input_LTrigger = Input.GetAxis("ps_Ltrigger");
                    input_RTrigger = Input.GetAxis("ps_Rtrigger");
                    input_DPad_H = Input.GetAxis("ps_DPadH");
                    input_DPad_V = Input.GetAxis("ps_DPadV");
                    L_XAxis = Input.GetAxis("Horizontal");
                    L_YAxis = Input.GetAxis("Vertical");
                    R_XAxis = Input.GetAxis("ps_RStickH");
                    R_YAxis = Input.GetAxis("ps_RStickV");
                    break;

                default:
                    input_RTrigger = Input.GetAxis("xbox_trigger") * 2 - 1;
                    input_LTrigger = Input.GetAxis("xbox_trigger") * 2 + 1;
                    input_DPad_H = Input.GetAxis("xbox_DPadH");
                    input_DPad_V = Input.GetAxis("xbox_DPadV");
                    L_XAxis = Input.GetAxis("Horizontal");
                    L_YAxis = Input.GetAxis("Vertical");
                    R_XAxis = Input.GetAxis("xbox_RStickH");
                    R_YAxis = Input.GetAxis("xbox_RStickV");
                    input_Button1 = Input.GetButton("xbox_AButton") ? 1 : 0;
                    input_Button2 = Input.GetButton("xbox_BButton") ? 1 : 0;
                    input_Button3 = Input.GetButton("xbox_XButton") ? 1 : 0;
                    input_Button4 = Input.GetButton("xbox_YButton") ? 1 : 0;
                    input_LBumper = Input.GetButton("xbox_LBumper") ? 1 : 0;
                    input_RBumper = Input.GetButton("xbox_RBumper") ? 1 : 0;

                    break;

            }

        }
        else
        {
        }

        CarTurn = Input.GetAxis("Horizontal") + L_XAxis; CarTurn = Mathf.Clamp(CarTurn, -1, 1);
        CarPivot = Input.GetAxis("Vertical") + L_YAxis; CarPivot = Mathf.Clamp(CarPivot, -1, 1);
        CarAccelerate = Input.GetAxis("Vertical") + input_RTrigger - input_LTrigger; CarAccelerate = Mathf.Clamp(CarAccelerate, -1, 1);
        CarDrift = (Input.GetKey(KeyCode.LeftShift) ? 1 : 0) + input_RBumper; CarDrift = Mathf.Clamp(CarDrift, -1, 1);
        CarBoost = (Input.GetMouseButton(0) ? 1 : 0) + input_Button2; CarBoost = Mathf.Clamp(CarBoost, -1, 1);
        CarPowerupUse = (Input.GetKey(KeyCode.Space) ? 1 : 0) + input_LBumper; CarPowerupUse = Mathf.Clamp(CarPowerupUse, -1, 1);

    }

    void DebugInputs()
    {
        if (debugXbox)
        {
            Debug.Log("xbox_trigger: " + Input.GetAxis("xbox_trigger"));
            Debug.Log("xbox_DPadH: " + Input.GetAxis("xbox_DPadH"));
            Debug.Log("xbox_DPadV: " + Input.GetAxis("xbox_DPadV"));
            Debug.Log("xbox_RStickH: " + Input.GetAxis("xbox_RStickH"));
            Debug.Log("xbox_RStickV: " + Input.GetAxis("xbox_RStickV"));

        }

        if (debugPlaystation)
        {
            Debug.Log("ps_Ltrigger: " + Input.GetAxis("ps_Ltrigger"));
            Debug.Log("ps_Rtrigger: " + Input.GetAxis("ps_Rtrigger"));
            Debug.Log("ps_DPadH: " + Input.GetAxis("ps_DPadH"));
            Debug.Log("ps_DPadV: " + Input.GetAxis("ps_DPadV"));
            Debug.Log("ps_RStickH: " + Input.GetAxis("ps_RStickH"));
            Debug.Log("ps_RStickV: " + Input.GetAxis("ps_RStickV"));
        }
    }

}
