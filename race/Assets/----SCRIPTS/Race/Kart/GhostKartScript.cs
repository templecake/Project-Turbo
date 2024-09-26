using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKartScript : MonoBehaviour
{
    private TrackManager tm;

    public List<Vector3> positions;
    public List<Vector3> rotations;
    Vector3 headingTo;
    Vector3 rotAim;
    public float updateSpeed;
    float timepassed;
    int on = 1;

    void Start()
    {
        tm = GameObject.FindGameObjectWithTag("TrackManager").GetComponent<TrackManager>();
    }

    void Update()
    {
        if (tm.RaceStarted)
        {
            MoveTo();

            timepassed += Time.deltaTime;
            if (timepassed < updateSpeed) { return; }
            timepassed = 0;
            on++;
        }
    }

    void MoveTo()
    {
        float am = timepassed / updateSpeed;
        headingTo = Vector3.Lerp(positions[on-1], positions[on], am);
        rotAim = Vector3.Lerp(rotations[on - 1], rotations[on], am);
        transform.position = headingTo;
        Quaternion rotto = Quaternion.Euler(rotAim);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotto, 90 * Time.deltaTime);
    }
}
