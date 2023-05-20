using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using static Sirenix.OdinInspector.Editor.Internal.FastDeepCopier;
using Unity.VisualScripting;
using UnityEngine.UI;

public class GateSc : MonoBehaviour
{
    GameManager gM;

    public float gateValue;

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


    void Awake()
    {
        gM = GameObject.Find("Game Manager").GetComponent<GameManager>();
        SetGateUIs();
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

    void SetGateUIs()
    {
        if (GateFunction == Function.Width && GateEffect == Effect.Positive)
        {
            transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = "+" + gateValue.ToString();
            transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Text>().text = "< >";

            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().color = new Color32(0x2F, 0xBF, 0xFF, 0xFF);
            transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<RawImage>().color = new Color32(0x2F, 0xBF, 0xFF, 0xFF);
        }
        else if (GateFunction == Function.Width && GateEffect == Effect.Negative)
        {
            transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = "-" + gateValue.ToString();
            transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Text>().text = "> <";

            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().color = new Color32(0xFF, 0x36, 0x36, 0xFF);
            transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<RawImage>().color = new Color32(0xFF, 0x36, 0x36, 0xFF);
        }

        else if (GateFunction == Function.Height && GateEffect == Effect.Positive)
        {
            transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = "+" + gateValue.ToString();
            transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Text>().text = ">";

            transform.GetChild(0).GetChild(0).GetChild(3).rotation = Quaternion.Euler(new Vector3(0,0,90f));

            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().color = new Color32(0x2F, 0xBF, 0xFF, 0xFF);
            transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<RawImage>().color = new Color32(0x2F, 0xBF, 0xFF, 0xFF);

        }
        else if (GateFunction == Function.Height && GateEffect == Effect.Negative)
        {
            transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = "-" + gateValue.ToString();
            transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Text>().text = "<";

            transform.GetChild(0).GetChild(0).GetChild(3).rotation = Quaternion.Euler(new Vector3(0, 0, 90f));

            transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RawImage>().color = new Color32(0xFF, 0x36, 0x36, 0xFF);
            transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<RawImage>().color = new Color32(0xFF, 0x36, 0x36, 0xFF);
        }
    }
}
