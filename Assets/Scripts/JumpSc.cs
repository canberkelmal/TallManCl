using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSc : MonoBehaviour
{
    GameManager gM;
    bool isJumped = false;
    public bool endJump = false;

    public float jumpDur = 2.5f;
    void Awake()
    {
        gM = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isJumped)
        {
            isJumped = true;
            gM.JumpPlayerTo(transform.parent.GetChild(1).position, jumpDur);
            if(endJump)
            {
                gM.FinishJumped();
            }
        }
    }
}
