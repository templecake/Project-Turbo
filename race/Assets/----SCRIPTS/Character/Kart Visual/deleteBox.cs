using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deleteBox : MonoBehaviour
{
    public float sizeX;
    public float sizeY;
    public float sizeZ;

    public float width;

    public GameObject xTopRight;
    public GameObject xTopLeft;
    public GameObject xBottomRight;
    public GameObject xBottomLeft;
    public GameObject yTopRight;
    public GameObject yTopLeft;
    public GameObject yBottomRight;
    public GameObject yBottomLeft;
    public GameObject zTopRight;
    public GameObject zTopLeft;
    public GameObject zBottomRight;
    public GameObject zBottomLeft;



    void FixedUpdate()
    {
        SetSizes();
        SetPositions();
    }

    void SetSizes()
    {
        xTopRight.transform.localScale = new Vector3(sizeX, width, width);
        xTopLeft.transform.localScale = new Vector3(sizeX, width, width);
        xBottomRight.transform.localScale = new Vector3(sizeX, width, width);
        xBottomLeft.transform.localScale = new Vector3(sizeX, width, width);

        yTopRight.transform.localScale = new Vector3(width, sizeY, width);
        yTopLeft.transform.localScale = new Vector3(width, sizeY, width);
        yBottomRight.transform.localScale = new Vector3(width, sizeY, width);
        yBottomLeft.transform.localScale = new Vector3(width, sizeY, width);

        zTopRight.transform.localScale = new Vector3(width, width, sizeZ);
        zTopLeft.transform.localScale = new Vector3(width, width, sizeZ);
        zBottomRight.transform.localScale = new Vector3(width, width, sizeZ);
        zBottomLeft.transform.localScale = new Vector3(width, width, sizeZ);
    }

    void SetPositions()
    {
        xTopRight.transform.localPosition = new Vector3(0, sizeY/2f, -sizeZ/2f);
        xTopLeft.transform.localPosition = new Vector3(0, sizeY / 2f, sizeZ / 2f);
        xBottomRight.transform.localPosition = new Vector3(0, -sizeY / 2f, -sizeZ / 2f);
        xBottomLeft.transform.localPosition = new Vector3(0, -sizeY / 2f, sizeZ / 2f);

        yTopRight.transform.localPosition = new Vector3(sizeX / 2, 0, -sizeZ / 2);
        yTopLeft.transform.localPosition = new Vector3(sizeX / 2, 0, sizeZ / 2);
        yBottomRight.transform.localPosition = new Vector3(-sizeX / 2, 0, -sizeZ / 2);
        yBottomLeft.transform.localPosition = new Vector3(-sizeX / 2, 0, sizeZ / 2);

        zTopRight.transform.localPosition = new Vector3(sizeX / 2, sizeY / 2, 0);
        zTopLeft.transform.localPosition = new Vector3(-sizeX / 2, sizeY / 2, 0);
        zBottomRight.transform.localPosition = new Vector3(sizeX / 2, -sizeY / 2, 0);
        zBottomLeft.transform.localPosition = new Vector3(-sizeX / 2, -sizeY / 2, 0);
    }

    public void UpdateMaterial(Material m)
    {
        xTopRight.GetComponent<MeshRenderer>().material = m;
        xTopLeft.GetComponent<MeshRenderer>().material = m;
        xBottomRight.GetComponent<MeshRenderer>().material = m;
        xBottomLeft.GetComponent<MeshRenderer>().material = m;

        yTopRight.GetComponent<MeshRenderer>().material = m;
        yTopLeft.GetComponent<MeshRenderer>().material = m;
        yBottomRight.GetComponent<MeshRenderer>().material = m;
        yBottomLeft.GetComponent<MeshRenderer>().material = m;

        zTopRight.GetComponent<MeshRenderer>().material = m;
        zTopLeft.GetComponent<MeshRenderer>().material = m;
        zBottomRight.GetComponent<MeshRenderer>().material = m;
        zBottomLeft.GetComponent<MeshRenderer>().material = m;
    }


}
