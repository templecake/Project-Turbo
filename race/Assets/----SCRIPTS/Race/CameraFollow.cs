using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameObject mainCamera;
    public bool camFollow;
    [SerializeField] float camFollowSpeed;
    [SerializeField] float camRotateSpeed;
    [SerializeField] float camDistance;

    [SerializeField] GameObject camPos;

    [SerializeField] float rotScale;

    [SerializeField] Vector3 ang;
    [SerializeField] Vector3 offset;

    InputManager im;

    public Camera cam;

    private void FixedUpdate()
    {
        im = GameObject.Find("GameManager").GetComponent<InputManager>();
        if (camFollow) { Travel(); Rotate(); }
    }

    void Travel()
    {
        if(cam!=null) cam.transform.position = Vector3.Lerp(cam.transform.position, camPos.transform.position+offset, camFollowSpeed * Time.fixedDeltaTime);

    }

    void Rotate()
    {
        GameObject target = transform.gameObject;
        if (cam != null)
        {
            Vector3 lookPos = cam.transform.position - target.transform.position;
            lookPos.Normalize();
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, Quaternion.LookRotation(-lookPos), camRotateSpeed * Time.fixedDeltaTime);
        }
        //cam.transform.forward = Vector3.Lerp(cam.transform.up, lookPos, Time.deltaTime);
         ang = new Vector3(0, im.R_XAxis * rotScale, 0);
         camPos.transform.localRotation = Quaternion.Lerp(camPos.transform.localRotation, Quaternion.Euler(ang), 90 * Time.fixedDeltaTime);
    }

}
