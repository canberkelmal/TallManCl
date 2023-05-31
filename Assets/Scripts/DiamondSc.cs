using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSc : MonoBehaviour
{
    GameManager gM;
    public bool finalDia = false;
    void Awake()
    {
        gM = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gM.TakeDiamond(transform.position, finalDia);

            Destroy(gameObject);
        }
    }
}
