using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyreConnectionCheck : MonoBehaviour
{
    public LayerMask CheckLayer;
    public GameObject left;
    public GameObject right;
    public void CheckConnections()
    {
        bool LeftConnect = Physics.Raycast(transform.Find("tyre").Find("connection").position, -transform.Find("tyre").Find("connection").right, 1f, CheckLayer);
        bool RightConnect = Physics.Raycast(transform.Find("tyre").Find("connection").position, transform.Find("tyre").Find("connection").right, 1f, CheckLayer);
    
        left.SetActive(LeftConnect);
        right.SetActive(RightConnect);
    }
}
