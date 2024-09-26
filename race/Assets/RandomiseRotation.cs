using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomiseRotation : MonoBehaviour
{
    public float Xrange;
    public float Yrange;
    public float Zrange;
    // Start is called before the first frame update
    void Start()
    {
        float x = Random.Range(-Xrange, Xrange);
        float y = Random.Range(-Yrange, Yrange);
        float z = Random.Range(-Zrange, Zrange);

        transform.rotation = Quaternion.Euler(x, y, z); 
    }

}
