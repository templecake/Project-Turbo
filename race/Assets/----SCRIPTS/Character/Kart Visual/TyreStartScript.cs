using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyreStartScript : MonoBehaviour
{
    [SerializeField] float CheckLength;
    public bool isStatic;
    public string TyreType;
    [SerializeField] GameObject leftConnect;
    [SerializeField] GameObject rightConnect;
    [SerializeField] LayerMask l;

    // Start is called before the first frame update
    void FixedUpdate()
    {
        TyreType = "";
        Vector3 checkSize = new Vector3(0.6f, 0.6f, 0.6f);
        Vector3 leftDist = -transform.forward * CheckLength;
        Collider[] leftColls = Physics.OverlapBox(transform.position + leftDist, checkSize, Quaternion.identity, l);
        bool blockToLeft=false;
        for (int i = 0; i < leftColls.Length; i++)
        {
            if(leftColls[i].tag == "buildingBlock" && leftColls[i].transform.name != transform.name) {
                Debug.Log("Collisions found to left");
                blockToLeft = true;
                break;
            }
        }

        Vector3 rightDist = transform.forward * CheckLength;
        Collider[] rightColls = Physics.OverlapBox(transform.position + rightDist, checkSize, Quaternion.identity, l);
        bool blockToRight=false;
        for (int i = 0; i < rightColls.Length; i++)
        {
            if (rightColls[i].tag == "buildingBlock" && rightColls[i].transform.name != transform.name)
            {
                Debug.Log("Collisions found to right");
                blockToRight = true;
                break;
            }
        }

        if(blockToLeft && !blockToRight)
        {
            TyreType = "left";
        }else if(blockToRight && !blockToLeft)
        {
            TyreType = "right";
        }
        else if(blockToLeft && blockToRight)
        {
            TyreType = "both";
        }

        rightConnect.SetActive(true);
        leftConnect.SetActive(true);
        switch (TyreType)
        {
            case "both":
                break;

            case "right":
                rightConnect.SetActive(false);
                break;

            case "left":
                leftConnect.SetActive(false);
                break;

        }


    }

    private void OnDrawGizmos()
    {
        Vector3 checkSize = new Vector3(0.3f, 0.3f, 0.3f);
        Vector3 leftDist = -transform.forward * CheckLength;
      Vector3 rightDist = transform.forward * CheckLength;

        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + leftDist, checkSize);
        Gizmos.DrawCube(transform.position + rightDist, checkSize);
    }


}
