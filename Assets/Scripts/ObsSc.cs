using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsSc : MonoBehaviour
{
    GameManager gM;
    public float damage = 50;
    void Awake()
    {
        gM = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gM.HitPlayerAt(transform.position, damage, true);
        }
    }
}
