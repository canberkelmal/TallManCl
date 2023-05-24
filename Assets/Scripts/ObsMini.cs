using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        if (!isHit && other.CompareTag("Player"))
        {
            isHit = true;
            gM.ChangePlayerHeight(false, damage);

            Vector3 hitDir = new Vector3(transform.position.x, Mathf.Abs(transform.position.x), 1);
            GetComponent<Rigidbody>().AddForce(hitDir * gM.obsMiniHitForce, ForceMode.Impulse);
            GetComponent<Rigidbody>().useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isHit && collision.transform.CompareTag("Player"))
        {
            isHit = true;

            gM.HitPlayerAt(transform.position, damage, false);
        }
    }
}
