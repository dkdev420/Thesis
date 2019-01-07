using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioBlock : MonoBehaviour
{
    [SerializeField] AudioSource coinSound;
    [SerializeField] GameObject coin;

    private void OnCollisionEnter(Collision collision) { ShotCoin(); }

    [ContextMenu("Shot Coin")]
    void ShotCoin()
    {
        GameObject c = Instantiate(coin, transform.position + Vector3.up, Quaternion.Euler(90f, 0f, 0f));
        c.SetActive(true);
        coinSound.Play();
    }

}
