using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonBlock : MonoBehaviour
{
    public int note = 0;
    int transpose = -4;
    Renderer rend;
    AudioSource audio;

    void Start ()
	{
        rend = GetComponent<Renderer>();
        audio = GetComponent<AudioSource>();
        audio.pitch = Mathf.Pow(2, (note + transpose) / 12.0f);
    }

    void Update ()
	{
    }

    [ContextMenu("PlaySound")]
    public void PlaySound()
    {
        //rend.material.SetFloat("_RimPower", 1);
        StartCoroutine(Glow());
    }

    IEnumerator Glow()
    {
        audio.Play();
        float glow = 6f;
        while(glow > 1f)
        {
            glow -= Time.deltaTime * 20;
            rend.material.SetFloat("_RimPower", glow);
            yield return 0;
        }

        while (glow < 6f)
        {
            glow += Time.deltaTime * 20;
            rend.material.SetFloat("_RimPower", glow);
            yield return 0;
        }
        audio.Stop();
        yield return 0;
    }
}
    