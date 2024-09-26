using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CorrespondingKart : MonoBehaviour
{
    public Transform correspondingKart;

    public GameObject[] CrashComicEffects;

    public float DifferenceNeeded;
    float currentSpeed;
    float speedLastCheck;
    float t;
    float cooldown;

    List<float> speeds;
    private void Start()
    {
        speeds = new List<float>(5);
    }
    
    private void Update()
    {

        currentSpeed = correspondingKart.GetComponent<KartController3>().RealSpeed;
        t += Time.deltaTime;
        cooldown += Time.deltaTime;
        if(t > 0.1f)
        {
            if(speeds.Count > 4)
            {
                speeds.RemoveAt(0);
            }
            speeds.Add(currentSpeed);
            t = 0;
        }
    }

    Vector3 hitpoint;
    private void OnCollisionEnter(Collision collision)
    {
            Debug.LogError(Mathf.Abs(currentSpeed - speeds[4]));
        if (Mathf.Abs(currentSpeed - speeds[4]) > DifferenceNeeded && collision.gameObject.layer != LayerMask.NameToLayer("Ground"))
        {
            hitpoint = collision.GetContact(0).point;
            PlayComicEffect();
        }
    }

    void PlayComicEffect()
    {
        if (cooldown < 1) return;
        cooldown = 0;
        GameObject comicChosen = CrashComicEffects[Random.Range(0, CrashComicEffects.Length)];

        GameObject effect = Instantiate(comicChosen);
        effect.transform.position = hitpoint;

    }
}
