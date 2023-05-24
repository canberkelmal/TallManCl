using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using static Sirenix.OdinInspector.Editor.Internal.FastDeepCopier;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using DG.Tweening;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;
using System;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    [Title("Movement")]
    [TabGroup("GamePlay")]
    public float playerMoveSens = 1f;
    [TabGroup("GamePlay")]
    public float playerMaxSpeed = 1f;
    [TabGroup("GamePlay")]
    public float playerRotateSens = 1f;
    [TabGroup("GamePlay")]
    public float xMax = 1f;
    [TabGroup("GamePlay")]
    public float xMin = 1f;
    [TabGroup("GamePlay")]
    public float playerJumpPower = 1f;


    [Title("Scene Objects")]
    [TabGroup("Objects")]
    [SceneObjectsOnly]
    public GameObject cam;
    [TabGroup("Objects")]
    [SceneObjectsOnly]
    public GameObject player;
    [TabGroup("Objects")]
    [SceneObjectsOnly]
    public GameObject director;
    [TabGroup("Objects")]
    [SceneObjectsOnly]
    public GameObject diaFrame;

    [Title("UI")]
    [TabGroup("Objects")]
    [SceneObjectsOnly]
    public GameObject failPanel;

    [Title("Materials")]
    [TabGroup("Objects")]
    [AssetsOnly]
    public Material playerMat;

    [Title("Particles")]
    [TabGroup("Objects")]
    [AssetsOnly]
    public GameObject takeDiaParticle;


    [Title("Player color change")]
    [TabGroup("Animations")]
    public float playerColorAnimDur = 0.5f;
    [TabGroup("Animations")]
    public Color playerColorPositive, playerColorNegative, playerMainColor;

    [Title("Player hit and lose part")]
    [TabGroup("Animations")]
    public float brokenPartForce = 1f;
    [TabGroup("Animations")]
    public float obsMiniHitForce = 1f;

    [Title("Door")]
    [TabGroup("Animations")]
    public float doorSlideSens = 1f;

    int diaCount;
    Vector3 diaFrameDefScale;

    [NonSerialized]
    public DG.Tweening.Sequence jumpTweener;
    public bool controller = true;

    Color playerStartColor, playerTargetColor, playerCurrentColor;
    float playerColorElapsedT = 0f;
    bool isPlayerColorAnimating = false, playerColorAnimatingPhase = false;

    float directorOffsZ = 1f;
    float directorOffsY = 1f;
    float playerCurrentSpeed = 0f;
    Vector3 camOffset;

    void Start()
    {
        failPanel.SetActive(false);
        Time.timeScale = 1f;

        directorOffsY = player.transform.position.y - director.transform.position.y;

        playerMat.color = playerMainColor;
        playerStartColor = playerMainColor;

        camOffset = player.transform.position - cam.transform.position;

        diaCount = PlayerPrefs.GetInt("diaCount", 0);
        diaFrame.transform.GetChild(1).GetComponent<Text>().text = diaCount.ToString();
        diaFrameDefScale = diaFrame.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && controller)
        {
            UpdateDirectorPositionX();
            MovePlayer(true);
        }
        else if(playerCurrentSpeed > 0 && controller)
        {
            director.transform.position = new Vector3(director.transform.position.x, director.transform.position.y, player.transform.position.z + directorOffsZ);
            UpdatePlayerRotationY();
            MovePlayer(false);
        }

        if(isPlayerColorAnimating)
        {
            playerColorElapsedT += Time.deltaTime;
            float t = Mathf.Clamp01(playerColorElapsedT / playerColorAnimDur);

            playerCurrentColor = Color.Lerp(playerStartColor, playerTargetColor, t);
            SetPlayerColor(playerCurrentColor);
            if (t >= 1f && !playerColorAnimatingPhase)
            {
                playerStartColor = playerCurrentColor;
                playerTargetColor = playerMainColor;
                playerColorElapsedT = 0f;
                playerColorAnimatingPhase = true;

                isPlayerColorAnimating = false;
                Invoke("SetColorAnimTrig", playerColorAnimDur);
            }
            else if(t >= 1f && playerColorAnimatingPhase)
            {
                isPlayerColorAnimating = false;
                playerColorAnimatingPhase = false;
                playerColorElapsedT = 0f;
                SetPlayerColor(playerMainColor);
            }
        }

        MoveCamera();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    public void TakeDiamond(Vector3 diaPos)
    {
        diaCount++;
        PlayerPrefs.SetInt("diaCount", diaCount);

        GameObject takeEffect = Instantiate(takeDiaParticle, diaPos, Quaternion.identity);
        Destroy(takeEffect, 1f);
        
        diaFrame.transform.GetChild(1).GetComponent<Text>().text = diaCount.ToString();

        diaFrame.transform.DOScale(diaFrameDefScale * 1.5f, 0.15f).OnComplete(SetDiaFrameScaleDef);
    }
    void SetDiaFrameScaleDef()
    {
        diaFrame.transform.DOScale(diaFrameDefScale, 0.15f);
    }

    public void HitPlayerAt(Vector3 hitPoint, float damage, bool brokePart)
    {
        if(brokePart)
        {
            ThrowBroken(hitPoint, damage);
        }

        if (player.transform.GetChild(1).localScale.y - damage * 0.01f >= 1f)
        {
            ChangePlayerHeight(false, damage);
        }
        else
        {
            ChangePlayerWidth(false, damage);
        }
    }

    void ThrowBroken(Vector3 spawnPoint, float amount)
    {
        spawnPoint.x = player.transform.position.x;
        spawnPoint.z = player.transform.position.z;

        GameObject brokenPart = Instantiate(player.transform.GetChild(1).gameObject, spawnPoint, Quaternion.identity);

        brokenPart.transform.localScale = new Vector3(brokenPart.transform.localScale.x, amount * 0.01f, brokenPart.transform.localScale.z);

        brokenPart.AddComponent<Rigidbody>().AddForce(-Vector3.forward * brokenPartForce, ForceMode.Impulse);
        brokenPart.transform.DORotate(new Vector3(179f, 0, 179f), 3.0f);

        Destroy(brokenPart, 3f);
    }

    public void JumpPlayerTo(Vector3 jumpPoint, float dur)
    {
        controller = false;
        player.transform.rotation = Quaternion.identity;

        jumpPoint.y = player.transform.position.y;
        jumpTweener = player.transform.DOJump(jumpPoint, playerJumpPower, 1, dur).SetEase(Ease.Linear).OnComplete(WaitForJump);
    }

    public void WaitForJump()
    {
        controller = true;
        director.transform.position = new Vector3(director.transform.position.x, player.transform.position.y + directorOffsY, player.transform.position.z + directorOffsZ);
    }
    public void ChangePlayerWidth(bool increase, float value)
    {
        if(increase)
        {
            player.transform.GetChild(1).localScale += new Vector3(value * 0.01f, 0, value * 0.01f);
            player.transform.GetChild(2).localScale += new Vector3(value * 0.01f, 0, value * 0.01f);

            StartPlayerColorAnim(increase);
        }
        else if(player.transform.GetChild(1).localScale.x - value * 0.01f >= 0.2f)
        {
            player.transform.GetChild(1).localScale -= new Vector3(value * 0.01f, 0, value * 0.01f);
            player.transform.GetChild(2).localScale -= new Vector3(value * 0.01f, 0, value * 0.01f);

            StartPlayerColorAnim(increase);
        }
        else
        {
            player.transform.GetChild(1).localScale = new Vector3(0.2f, player.transform.GetChild(1).localScale.y, 0.2f);
            player.transform.GetChild(2).localScale = new Vector3(0.2f, player.transform.GetChild(2).localScale.y, 0.2f);

            StartPlayerColorAnim(increase);

            Failed();
        }

    }

    public void ChangePlayerHeight(bool increase, float value)
    {
        if (increase)
        {
            player.transform.GetChild(1).localScale += new Vector3(0, value * 0.01f, 0);

            StartPlayerColorAnim(increase);
        }
        else if (player.transform.GetChild(1).localScale.y - value * 0.01f >= 1f)
        {
            player.transform.GetChild(1).localScale -= new Vector3(0, value * 0.01f, 0);

            StartPlayerColorAnim(increase);
        }
        else
        {
            player.transform.GetChild(1).localScale = new Vector3(player.transform.GetChild(1).localScale.x, 1, player.transform.GetChild(1).localScale.z);

            StartPlayerColorAnim(increase);

            Failed();
        }
    }

    void StartPlayerColorAnim(bool positive)
    {
        if(positive)
        {
            playerTargetColor = playerColorPositive;
            playerStartColor = playerMainColor;
            isPlayerColorAnimating = true;
        }
        else
        {
            playerTargetColor = playerColorNegative;
            playerStartColor = playerMainColor;
            isPlayerColorAnimating = true;
        }
    }

    void SetPlayerColor(Color setColor)
    {
        playerMat.color = setColor;
    }

    void SetColorAnimTrig()
    {
        isPlayerColorAnimating = true;
    }


    void UpdateDirectorPositionX()
    {
        UpdatePlayerRotationY();

        if (Input.GetAxis("Mouse X") != 0)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float moveAmount = mouseX * playerRotateSens;
            Vector3 newPosX = director.transform.position + director.transform.right * moveAmount;
            newPosX.x = Mathf.Clamp(newPosX.x, xMin, xMax);
            newPosX.z = player.transform.position.z + directorOffsZ;
            newPosX.y = player.transform.position.y;
            director.transform.position = newPosX;
        }
        else
        {
            director.transform.position = Vector3.Lerp(director.transform.position, player.transform.position + Vector3.forward * directorOffsZ, playerRotateSens * Time.deltaTime);
            director.transform.position = new Vector3(director.transform.position.x, director.transform.position.y, player.transform.position.z + directorOffsZ);
        }
    }

    void MovePlayer(bool direction)
    {
        if(direction)
        {
            playerCurrentSpeed += playerMoveSens * Time.deltaTime;
        }
        else
        {
            playerCurrentSpeed -= playerMoveSens * 5 * Time.deltaTime;
        }
        playerCurrentSpeed = Mathf.Clamp(playerCurrentSpeed, 0f, playerMaxSpeed);
        player.transform.position += player.transform.forward * playerCurrentSpeed * Time.deltaTime;
    }

    void UpdatePlayerRotationY()
    {
        player.transform.LookAt(director.transform.position);
    }

    void MoveCamera()
    {
        Vector3 camTargetPoint = player.transform.position - camOffset;
        cam.transform.position = camTargetPoint;
    }

    void Failed()
    {
        controller = false;
        Time.timeScale = 0.5f;
        failPanel.SetActive(true);
    }

    // Reload the current scene to restart the game
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
