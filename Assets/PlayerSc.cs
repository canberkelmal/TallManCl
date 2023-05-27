using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSc : MonoBehaviour
{
    public float scaleFactor = 1f;
    public float scaleFactorY = 1f;

    // Update is called once per frame
    void Update()
    {
        transform.localScale = (Vector3.right + Vector3.forward) * scaleFactor + Vector3.up * scaleFactorY;
    }
}
