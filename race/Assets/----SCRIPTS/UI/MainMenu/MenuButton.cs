using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public void HoverEnter()
    {
        GetComponent<Animator>().SetBool("Hovering", true);
        FindObjectOfType<GameManager>().PlaySoundEffect(3);
    }

    public void HoverExit()
    {
        GetComponent<Animator>().SetBool("Hovering", false);
    }

}
