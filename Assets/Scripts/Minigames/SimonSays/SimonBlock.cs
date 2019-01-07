using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonBlock : MonoBehaviour
{
    public int note = 0;
    int transpose = -4;
    Renderer rend;
    AudioSource audioSource;

    void Start ()
	{
        rend = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Mathf.Pow(2, (note + transpose) / 12.0f);
    }

    [ContextMenu("PlaySound")]
    public void PlaySound()
    {
        StartCoroutine(Glow());
    }

    public void Select()
    {
        if(Simon.Instance.checking)
        {
            Simon.Instance.toCheck.Enqueue(this);
            StartCoroutine(Glow());
        }
    }


    IEnumerator Glow()
    {
        audioSource.Play();
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
        audioSource.Stop();
        yield return 0;
    }
}
    