using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsSc : MonoBehaviour
{
    GameManager gM;
    void Awake()
    {
        gM = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gM.HitPlayerAt(transform.position);
            //Destroy(gameObject);
        }
    }
}