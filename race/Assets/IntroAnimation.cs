using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroAnimation : MonoBehaviour
{
    public AudioClip clickAudio;

    public void Click(float empty)
    {
        FindObjectOfType<GameManager>().PlaySoundClip(clickAudio, .5f);
    }


    public void Finished(float empty)
    {
        FindObjectOfType<IntroManager>().IntroAnimationFinishedFunction();
    }
}
