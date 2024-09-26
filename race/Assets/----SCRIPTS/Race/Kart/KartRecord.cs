using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartRecord : MonoBehaviour
{
    private TrackManager tm;
    public List<Vector3> positions;
    public List<Vector3> rotations;
    public bool Recording;
    public float updatespeed;
    float passed;
    
    // Start is called before the first frame update
    void Start()
    {
       tm=GameObject.FindGameObjectWithTag("TrackManager").GetComponent<TrackManager>(); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(tm.RaceStarted && Recording)
        {
            passed += Time.deltaTime;
            if (passed < updatespeed) { return; }

            Vector3 pos = transform.position;
            Vector3 rot = transform.Find("main").eulerAngles;
            positions.Add(pos);
            rotations.Add(rot);
            passed = 0;
        }
    }
}
