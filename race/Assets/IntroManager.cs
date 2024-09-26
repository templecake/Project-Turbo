using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Intro());
    }

    void Update()
    {
        if ( (Input.anyKeyDown || Input.GetMouseButtonUp(0)) && IntroFinished)
        {
            FindObjectOfType<GameManager>().ReturnToMainMenu();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Skip = true;
        }
    }

    bool IntroFinished = true;
    public GameObject IntroAnimation;
    bool Skip = false;
    
    public void IntroAnimationFinishedFunction()
    {
        IntroAnimationFinished = true;
    }


    bool IntroAnimationFinished = false;

    public Image FadeOutImage;
    IEnumerator Intro()
    {
        IntroAnimationFinished = false;
        while (!IntroAnimationFinished)
        {
            if (Skip) IntroAnimationFinished = true;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        IntroAnimation.SetActive(false);
        
        Color c = Color.black; c.a = 1;
        
        yield return new WaitForSeconds(0.5f);

        float t = 0;
        while (t < 1 && !Skip)
        {
            c = Color.black; c.a = 1 - t;
            FadeOutImage.color = c;
            t += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        IntroFinished = true;
        c.a = 0;
        FadeOutImage.color = c;

        yield return false;
    }
}
