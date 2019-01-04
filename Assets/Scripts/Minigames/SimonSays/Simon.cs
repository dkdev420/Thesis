using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simon : Thesis.SingletonMonobehaviour<Simon>
{
    public SimonBlock[] blocks;
    System.Random random = new System.Random();

    void Start()
    {
    }

    void Update()
    {

    }

    List<SimonBlock> beeps = new List<SimonBlock>();
    IEnumerator SimonSays()
    {
        SetSequence();
        foreach (SimonBlock b in beeps)
        {
            b.PlaySound();
            yield return new WaitForSeconds(1f);
        }
        yield return 0;
    }

    int count = 1;
    void SetSequence()
    {
        beeps.Clear();
        for (int i = 0; i < count; i++) beeps.Add(blocks[random.Next(0, blocks.Length)]);
    }

    [ContextMenu("Test")]
    void Test()
    {
        count = 4;
        StartCoroutine(SimonSays());
    }
}
