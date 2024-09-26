using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuEnterZone : MonoBehaviour
{
    RealWorldMainMenu rrm;

    public string AssosciatedSceneName;
    public Transform player;

    public float EnterRange;
    public int LastMenuID;

    public string EnterName;

    bool InRange;

    public int EnterSoundEffectID;

    private void Start()
    {
        rrm = FindObjectOfType<RealWorldMainMenu>();
    }

    private void Update()
    {
        if (InRange)
        {
            CheckInputs();
        }
    }

    void FixedUpdate()
    {
        CheckInRange();
        
    }

    //move and park up

    void CheckInputs()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (EnterSoundEffectID > -1) FindObjectOfType<GameManager>().PlaySoundEffect(EnterSoundEffectID);
          FindObjectOfType<GameManager>().LoadScene(AssosciatedSceneName);
            FindObjectOfType<GameManager>().LastMenuVisited = LastMenuID;
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
          if(!rrm.EnterNames.Contains(EnterName)) rrm.EnterNames.Add(EnterName);
        }
        else
        {
          if (rrm.EnterNames.Contains(EnterName)) rrm.EnterNames.Remove(EnterName);
        }
    }

    
}
