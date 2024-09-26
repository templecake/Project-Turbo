using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AICharacter
{
    public string CharacterName;
    public GameManager.AIDifficulty aiDifficulty;

    //characters car
    public Car charactersCar;

    //characters customisation;
    public int CharacterHead;
    public int CharacterBody;
    public int CharacterLegs;
    public int CharacterStyle;
}



public class AIManager : MonoBehaviour
{
    public List<AICharacter> AICharacters;
}
