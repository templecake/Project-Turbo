using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TreeSpawner : MonoBehaviour
{
    public GameObject[] trees;
    public bool Generate;
    public int AmountToSpawn;
    public int xRadius;
    public int zRadius;
    public float xRotateAmount;
    public float yRotateAmount;
    public float zRotateAmount;


    List<Vector3> positionsSpawned = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < AmountToSpawn; i++)
        {
            GameObject treeToSpawn = trees[Random.Range(0, trees.Length)];
            GameObject tree =Instantiate(treeToSpawn);
            tree.transform.SetParent(transform);
            tree.transform.localScale = Vector3.one;
            bool gotpos = false;
            int attempts = 40;

            Vector3 position = Vector3.zero;
            while (!gotpos && attempts > 0)
            {
                int xPosChosen = Random.Range(-xRadius, xRadius);
                int zPosChosen = Random.Range(-zRadius, zRadius);
                position = new Vector3(xPosChosen, 0, zPosChosen);
                if (!positionsSpawned.Contains(position))
                {
                    position.x = position.x * 4 - Mathf.Sign(position.x) * 2;
                    position.z = position.z * 4;

                    Collider[] cols = Physics.OverlapSphere(position, .1f);
                    positionsSpawned.Add(position);
                    gotpos = true;
                }
            }

            if (gotpos) {

                tree.transform.localPosition = position;

                tree.transform.localEulerAngles = new Vector3(Random.Range(-xRotateAmount, xRotateAmount), Random.Range(-yRotateAmount, yRotateAmount), Random.Range(-zRotateAmount, zRotateAmount));

            }
            else
            {
                Destroy(tree);
            }
        }  
    }

}
