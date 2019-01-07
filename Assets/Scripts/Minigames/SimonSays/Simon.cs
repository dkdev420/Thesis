using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Simon : Thesis.SingletonMonobehaviour<Simon>
{
    public SimonBlock[] blocks;
    System.Random random = new System.Random();

    public bool checking = false;
    public Queue<SimonBlock> toCheck = new Queue<SimonBlock>();

    void Start()
    {
        count = 4;
        StartCoroutine(SimonSays());
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
        StartCoroutine(CheckForSequence());
        yield return 0;
    }

    IEnumerator CheckForSequence()
    {
        checking = true;
        int curr = 0;
        while (true)
        {
            if(curr == 4)
            {
                Debug.Log("WIN");
                break;
            }
            SimonBlock b;
            //if ((b = toCheck.FirstOrDefault()) != null)
            if (toCheck.Count > 0 && (b = toCheck.Dequeue()) != null)
            {
                if (b == beeps[curr])
                {
                    curr++;
                    continue;
                }
                else
                {
                    Debug.Log("LOSE");
                    break;
                }
            }
            yield return 0;
        }
        checking = false;
        yield return new WaitForSeconds(2f);
        StartCoroutine(SimonSays());
        yield break;
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
