using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using static Sirenix.OdinInspector.Editor.Internal.FastDeepCopier;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;

public class GateSc : MonoBehaviour
{
    GameManager gM;

    public float gateValue;
    public bool isMoving = false;
    public float gateTarget = 5f;
    public float gateStopLimit = 2.8f;


    bool movingRight = true;
    Vector3 startPos, rightPos, leftPos;


    [EnumToggleButtons]
    public Function GateFunction;
    [EnumToggleButtons]
    public Effect GateEffect;

    [HorizontalGroup("Group1", LabelWidth = 85)]
    public Color colorPositive, colorNegative;
    public enum Function
    {
        Width, Height
    }
    public enum Effect
    {
        Positive, Negative
    }


    void Awake()
    {
        gM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        SetGateUIs();
        startPos = transform.position;
        rightPos = new Vector3(gateTarget, startPos.y, startPos.z);
        leftPos = new Vector3(-gateTarget, startPos.y, startPos.z);
    }

    void Update()
    {
        if(isMoving)
        {
            SlideTheGate();
        }
    }

    private void SlideTheGate()
    {
        Vector3 target = movingRight ? rightPos : leftPos;
        transform.position = Vector3.Lerp(transform.position, target, gM.doorSlideSens * Time.deltaTime);
        if(transform.position.x > gateStopLimit)
        {
            movingRight = false;
        }
        else if(transform.position.x < -gateStopLimit)
        {
            movingRight = true;
        }
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

            DestroyGate();
        }
    }

    void DestroyGate()
    {
        Destroy(gameObject);
    }

    void SetGateUIs()
    {
        if (GateFunction == Function.Width && GateEffect == Effect.Positive)
        {
            transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = "+" + gateValue.ToString();
            transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Text>().text = "< >";

            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().color = colorPositive;
            transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<RawImage>().color = colorPositive;
        }
        else if (GateFunction == Function.Width && GateEffect == Effect.Negative)
        {
            transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = "-" + gateValue.ToString();
            transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Text>().text = "> <";

            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().color = colorNegative;
            transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<RawImage>().color = colorNegative;
        }

        else if (GateFunction == Function.Height && GateEffect == Effect.Positive)
        {
            transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = "+" + gateValue.ToString();
            transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Text>().text = ">";

            transform.GetChild(0).GetChild(0).GetChild(3).rotation = Quaternion.Euler(new Vector3(0,0,90f));

            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().color = colorPositive;
            transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<RawImage>().color = colorPositive;

        }
        else if (GateFunction == Function.Height && GateEffect == Effect.Negative)
        {
            transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = "-" + gateValue.ToString();
            transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Text>().text = "<";

            transform.GetChild(0).GetChild(0).GetChild(3).rotation = Quaternion.Euler(new Vector3(0, 0, 90f));

            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().color = colorNegative;
            transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<RawImage>().color = colorNegative;
        }
    }
}
