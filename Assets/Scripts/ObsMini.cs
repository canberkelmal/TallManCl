using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsMini : MonoBehaviour
{
    GameManager gM;
    bool isHit = false;

    public float damage = 50;
    void Awake()
    {
        gM = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!isHit && collision.transform.CompareTag("Player"))
        {
            isHit = true;
            gM.ChangePlayerHeight(false, damage);
        }
    }
}
