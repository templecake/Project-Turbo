using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestColourItemChange : MonoBehaviour
{
    public Material m;

    void Update()
    {
        GetComponent<Renderer>().material = m;   
    }
}
