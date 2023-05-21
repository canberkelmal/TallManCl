using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSc : MonoBehaviour
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
            gM.JumpPlayerTo(transform.GetChild(0).position);
            //Destroy(gameObject);
        }
    }
}
