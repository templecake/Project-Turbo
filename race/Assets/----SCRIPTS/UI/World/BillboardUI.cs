using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Transform MainCamera;
    private void Start()
    {
        MainCamera = Camera.main.transform;      
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + MainCamera.rotation * Vector3.forward, MainCamera.rotation * Vector3.up);
    }
}
