using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class PowerupManager : MonoBehaviour
{
    GameManager gm;

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
}
