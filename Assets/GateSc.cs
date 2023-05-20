using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using static Sirenix.OdinInspector.Editor.Internal.FastDeepCopier;
using Unity.VisualScripting;

public class GateSc : MonoBehaviour
{
    public float gateValue;

    GameManager gM;


    [EnumToggleButtons]
    public Function GateFunction;
    [EnumToggleButtons]
    public Effect GateEffect;
    public enum Function
    {
        Width, Height
    }
    public enum Effect
    {
        Positive, Negative
    }


    // Start is called before the first frame update 
    void Start()
    {
        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GateFunction == Function.Width && GateEffect == Effect.Positive)
            {
                gM.ChangePlayerWidth(true, gateValue);
            }
            else if (GateFunction == Function.Width && GateEffect == Effect.Negative)
            {
                gM.ChangePlayerWidth(false, gateValue);
            }
            else if (GateFunction == Function.Height && GateEffect == Effect.Positive)
            {
                gM.ChangePlayerHeight(true, gateValue);
            }
            else if (GateFunction == Function.Height && GateEffect == Effect.Negative)
            {
                gM.ChangePlayerHeight(false, gateValue);
            }

        }
    }
}
