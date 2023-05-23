using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSc : MonoBehaviour
{
    GameManager gM;
    public bool isEnd = false;
    void Awake()
    {
        gM = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gM.JumpPlayerTo(transform.parent.GetChild(1).position, isEnd);
            //Destroy(gameObject);
        }
    }
}
