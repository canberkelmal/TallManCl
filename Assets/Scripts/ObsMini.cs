using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObsMini : MonoBehaviour
{    
    public float damage = 50;

    GameManager gM;
    bool isHit = false;
    void Awake()
    {
        gM = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isHit && collision.transform.CompareTag("Player"))
        {
            isHit = true;

            gM.HitPlayerAt(transform.position, damage, false);

            Hit(collision.gameObject);
        }
    }

    void Hit(GameObject player)
    {
        Vector3 hitVector = Vector3.one;

        hitVector.x = transform.position.x - player.transform.position.x;
        if(hitVector.x > 0)
        {
            hitVector.x = 1;
        }
        else
        {
            hitVector.x = -1;
        }

        GetComponent<Rigidbody>().AddForce(hitVector * gM.obsMiniHitForce, ForceMode.Impulse);
    }
}
