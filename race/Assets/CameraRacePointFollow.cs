using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRacePointFollow : MonoBehaviour
{
    public bool Activated;
    public int PointNum;
    public float TimePanning;
    public GameObject Subject;
    public GameObject ReferenceObject;
    public float FOV;
    public float followSpeed;

    [Range(0, 100)] public float FOVdistanceScale;
    public float FOVdistanceStart;
    public float FOVMin;
    public float FOVMax;

    private void FixedUpdate()
    {
        if (!Activated) return;
        Camera.main.fieldOfView = FOV;
        ReferenceObject.transform.LookAt(Subject.transform);
        Camera.main.transform.position = transform.position;
        Camera.main.transform.forward = Vector3.Lerp(Camera.main.transform.forward, ReferenceObject.transform.forward, followSpeed * Time.fixedDeltaTime);
        //Camera.main.transform.LookAt(Subject.transform);

        float distance = Vector3.Distance(ReferenceObject.transform.position, Subject.transform.position) - FOVdistanceStart;
        distance = Mathf.Clamp(distance, 0, Mathf.Infinity);
        distance /= FOVdistanceScale;
        float f = FOV - distance;
        f = Mathf.Clamp(f, FOVMin, FOVMax);

        Camera.main.fieldOfView = f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CorrespondingKart>())
        {
            if(other.GetComponent<CorrespondingKart>().correspondingKart.gameObject == Subject)
            {
                FindObjectOfType<RaceManager>().UpdateCameraPanView(PointNum, TimePanning); 
            }
        }
    }
}
