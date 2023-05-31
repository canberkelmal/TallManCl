using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObsMini : MonoBehaviour
{    
    public float damage = 50;
    public bool finishObs = false;
    public float multiplier = 1f;

    public RawImage UIObj1, UIObj2;
    public float colorAnimSens = 0.4f;
    Color UIColor;

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

            ChangeLayerRecursively(gameObject.transform, "ObsMini");

            if(finishObs)
            {
                gM.SetFinalMultiplier(multiplier);
                UIColor = UIObj1.color;
                UIObj1.DOColor(Color.white, colorAnimSens).OnComplete(SetColorBack);
                UIObj2.DOColor(Color.white, colorAnimSens);
            }
        }
    }

    void SetColorBack()
    {
        UIObj1.DOColor(UIColor, colorAnimSens);
        UIObj2.DOColor(UIColor, colorAnimSens);
    }

    void Hit(GameObject player)
    {
        Vector3 hitVector = Vector3.one;

        hitVector.x = finishObs ? transform.position.x : transform.position.x - player.transform.position.x;
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
    void ChangeLayerRecursively(Transform targetTransform, string layerName)
    {
        targetTransform.gameObject.layer = LayerMask.NameToLayer(layerName);

        foreach (Transform child in targetTransform)
        {
            ChangeLayerRecursively(child, layerName);
        }
    }
}
