using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditModeButton : MonoBehaviour
{
    public int id;
    public void Select()
    {
        GameObject.Find("CreationManager").GetComponentInChildren<CreationManager>().ChangeMode(id);
    }
}
