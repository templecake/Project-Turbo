using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPowerup : MonoBehaviour
{
    public float RocketSpeed;
    public float RocketTurnSpeed;
    public float RocketPositionCheckRadius;
    public float RocketDetectRadius;

    public GameObject OwnersKart;

    public int CurrentPoint;
    public int NextPoint;

    public GameObject dir;

    bool RouteGot = false;
    AIRoute ThisRoute;

    private void Start()
    {
        GetMissileRoute(); 
    }

    private void Update()
    {
        Move();
        Turn();
        //move along the route
        //if there is a player within certain range, lock on
        //move towards the player
    }

    #region Getting Route
    public void GetMissileRoute()
    {
        MapStorage map = FindObjectOfType<MapStorage>();
        if (map == null) return;

        ThisRoute = map.GetGeneralRoute();
        float closestDistance = float.MaxValue;
        int closest = -1;
        for (int i = 0; i < ThisRoute.positions.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, ThisRoute.positions[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = i;
            }   
        }

        CurrentPoint = closest;
        NextPoint = CurrentPoint == ThisRoute.positions.Count - 1 ? 0 : CurrentPoint+1;
    }

    public void GetNextPoint()
    {
        CurrentPoint = NextPoint;
        NextPoint = CurrentPoint == ThisRoute.positions.Count - 1 ? 0 : CurrentPoint+1;
    }

    #endregion

    #region Move The Missil

    public void Move()
    {
        transform.position += Time.deltaTime * RocketSpeed *  transform.forward;   

        if(Vector3.Distance(transform.position, ThisRoute.positions[NextPoint]) < RocketPositionCheckRadius)
        {
            GetNextPoint();
            return;
        }
    }

    public void Turn()
    {
        dir.transform.position = transform.position;
        if (!GotTarget)
        {
            dir.transform.LookAt(ThisRoute.positions[NextPoint]);
        }
        else
        {
            dir.transform.LookAt(Target.transform.position);
        }

        transform.forward = Vector3.Lerp(transform.forward, dir.transform.forward, RocketTurnSpeed * Time.deltaTime);
    }

    #endregion

    #region CheckRange

    public bool GotTarget;
    public GameObject Target;

    public void CheckNearby()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, RocketDetectRadius);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].GetComponent<CorrespondingKart>())
            {
                if (cols[i].GetComponent<CorrespondingKart>().correspondingKart.gameObject != OwnersKart)
                {
                    Target = cols[i].gameObject;
                    GotTarget = true;
                    return;
                }
            }
        }
    }


    #endregion

    #region Destroying

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CorrespondingKart>())
        {
            if (other.GetComponent<CorrespondingKart>().correspondingKart == OwnersKart) return;
            other.GetComponent<CorrespondingKart>().correspondingKart.GetComponent<KartController3>().ExplodeKart();
        }
    }

    #endregion
}
