using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineSc : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
