using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScrollingTextController : MonoBehaviour
{
    public GameObject TextToScroll;
    public float ScrollSpeed;

    public List<GameObject> ScrollingTexts = new List<GameObject>();

    private void Start()
    {
        StartPosition = TextToScroll.transform.localPosition;
        size = TextToScroll.GetComponent<RectTransform>().sizeDelta;

        for (int i = 0; i < 3; i++)
        {
            CreateNewText();   
        }
    }

    Vector3 StartPosition;
    Vector3 size;

    void CreateNewText()
    {
        GameObject newText = Instantiate(ScrollingTexts[0], transform);
        newText.transform.localScale = Vector3.one;
        newText.transform.localPosition = StartPosition + new Vector3(size.x*ScrollingTexts.Count, 0, 0);
        newText.transform.SetSiblingIndex(0);
        newText.transform.name = ScrollingTexts[0].name;
        ScrollingTexts.Add(newText);
    }   

    float Progress;

    void Update()
    {
        Progress += Time.deltaTime * ScrollSpeed;
        for (int i = 0; i < ScrollingTexts.Count; i++)
        {
            ScrollingTexts[i].GetComponent<RectTransform>().localPosition -= new Vector3(size.x * Time.deltaTime * ScrollSpeed, 0, 0);
        }
        if(Progress >= 1)
        {
            Destroy(ScrollingTexts[0]);
            ScrollingTexts.RemoveAt(0);
            CreateNewText();
            Progress = 0;
        }
    }
}
