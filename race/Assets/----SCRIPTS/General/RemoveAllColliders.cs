using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAllColliders : MonoBehaviour
{
    private void Start()
    {
        Collider[] cols = FindObjectsByType<Collider>(FindObjectsSortMode.None);
        for (int i = 0; i < cols.Length; i++)
        {
            Destroy(cols[i]);
        }

    }
}
