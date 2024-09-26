using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuOpenZone : MonoBehaviour
{
    RealWorldMainMenu mm;

    public int MenuToOpen;
    public float EnterRange;
    public Transform Player;
    public string EnterName;

    void Start()
    {
        mm = FindObjectOfType<RealWorldMainMenu>();
    }

    bool InRange;

    void Update()
    {
        CheckInRange();
        if (InRange)
        {
            CheckInputs();
        }
    }

    void CheckInputs()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            mm.ChangeMenuOpen(MenuToOpen);    
        }
    }

    private void CheckInRange()
    {
        InRange = false;
        Collider[] cols = Physics.OverlapSphere(transform.position, EnterRange);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].name == "KartMovementSphere")
            {
                InRange = true;
                break;
            }
        }

        if (InRange)
        {
            if (!mm.EnterNames.Contains(EnterName)) mm.EnterNames.Add(EnterName);
        }
        else
        {
            if (mm.EnterNames.Contains(EnterName)) mm.EnterNames.Remove(EnterName);
        }
    }
}
