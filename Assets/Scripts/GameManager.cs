using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;
using System;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.GraphicsBuffer;
using Unity.Mathematics;

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
    [Title("Reshape")]
    [TabGroup("GamePlay")]
    public float widthCons = 8f;
    [TabGroup("GamePlay")]
    public float heigthCons = 0.01f;


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

    [Title("Player Shape Objects")]
    [TabGroup("Objects")]
    [SceneObjectsOnly]
    public GameObject arms;
    [TabGroup("Objects")]
    [SceneObjectsOnly]
    public GameObject body;
    [TabGroup("Objects")]
    [SceneObjectsOnly]
    public GameObject hips;
    [TabGroup("Objects")]
    [SceneObjectsOnly]
    public GameObject legs;
    [TabGroup("Objects")]
    [SceneObjectsOnly]
    public GameObject spine;

    [Title("UI")]
    [TabGroup("Objects")]
    [SceneObjectsOnly]
    public GameObject failPanel;
    [TabGroup("Objects")]
    [SceneObjectsOnly]
    public GameObject finishPanel;

    [Title("Materials")]
    [TabGroup("Objects")]
    [AssetsOnly]
    public Material playerMat;

    [Title("Assets")]
    [TabGroup("Objects")]
    [AssetsOnly]
    public GameObject brokenCyl;

    [Title("Particles")]
    [TabGroup("Objects")]
    [AssetsOnly]
    public GameObject takeDiaParticle;
    [TabGroup("Objects")]
    [AssetsOnly]
    public GameObject finishConfettis;


    [Title("Player animator")]
    [TabGroup("Animations")]
    public Animator playerAnimator;

    [Title("Player color change")]
    [TabGroup("Animations")]
    public float playerColorAnimDur = 0.5f;
    [TabGroup("Animations")]
    public Color playerColorPositive, playerColorNegative, playerMainColor;

    [Title("Player reshape")]
    [TabGroup("Animations")]
    public float brokenPartForce = 1f;
    [TabGroup("Animations")]
    public float obsMiniHitForce = 1f;
    [TabGroup("Animations")]
    public float heightAnimSens = 1f;
    [TabGroup("Animations")]
    public float widthAnimSens = 1f;

    [Title("Door")]
    [TabGroup("Animations")]
    public float doorSlideSens = 1f;

    [Title("Camera")]
    [TabGroup("Animations")]
    public float cameraFinalPosSens = 1f;
    [TabGroup("Animations")]
    public float cameraFinalYOffs = 4f;

    [Title("Bonus poin animation")]
    [TabGroup("Animations")]
    public float bonusAnimDuration = 1f;

    int diaCount, finalDiaCount;
    float finalMultiplier = 1f;
    Vector3 diaFrameDefScale;

    [NonSerialized]
    public DG.Tweening.Sequence jumpTweener;
    public bool controller = true;
    bool isFinished = false;

    float bonusAnimElapsedT = 0f;    

    Color playerStartColor, playerTargetColor, playerCurrentColor;
    float playerColorElapsedT = 0f;
    bool isPlayerColorAnimating = false, playerColorAnimatingPhase = false;

    float directorOffsZ = 1f;
    float directorOffsY = 1f;
    float playerCurrentSpeed = 0f;
    Vector3 camOffset;


    [NonSerialized]
    public float jumpStartTime;
    [NonSerialized]
    public float jumpDuration = 2f;
    float startYPos, targetYPos, targetZPos, startZPos;

    public float thicknessShapeKey = 0;
    public float height = 0;
    public float width = 0;
    public float defHeight, defScale;

    Vector3 newCamOffset;

    bool isRunning = false;

    void Start()
    {
        height = spine.transform.localPosition.y;
        width = arms.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(0);
        defHeight = -0.4f;
        defScale = width;
        finalDiaCount = 0;

        failPanel.SetActive(false);
        //Time.timeScale = 1f;

        directorOffsY = player.transform.position.y - director.transform.position.y;

        playerMat.color = playerMainColor;
        playerStartColor = playerMainColor;

        camOffset = player.transform.position - cam.transform.position;
        newCamOffset = new Vector3(camOffset.x - 2.7f, camOffset.y, camOffset.z);

        diaCount = PlayerPrefs.GetInt("diaCount", 0);
        diaFrame.transform.GetChild(1).GetComponent<Text>().text = diaCount.ToString();
        diaFrameDefScale = diaFrame.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && controller)
        {
            UpdatePlayerRotationY();
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

    public void TakeDiamond(Vector3 diaPos, bool isFinal)
    {
        finalDiaCount += isFinal ? 1 : 0;
        IncreaseDiamondCount();
        GameObject takeEffect = Instantiate(takeDiaParticle, diaPos, Quaternion.identity);
        Destroy(takeEffect, 1f);
        
    }
    void IncreaseDiamondCount()
    {
        diaCount++;
        PlayerPrefs.SetInt("diaCount", diaCount);
        diaFrame.transform.GetChild(1).GetComponent<Text>().text = diaCount.ToString();

        diaFrame.transform.DOScale(diaFrameDefScale * 1.5f, 0.15f).OnComplete(SetDiaFrameScaleDef);
    }
    void SetDiaFrameScaleDef()
    {
        diaFrame.transform.DOScale(diaFrameDefScale, 0.15f);
    }

    public void SetFinalMultiplier(float multiplier)
    {
        finalMultiplier = multiplier;
    }

    public void HitPlayerAt(Vector3 hitPoint, float damage, bool brokePart)
    {
        if(brokePart)
        {
            ThrowBroken(hitPoint);
        }

        if (spine.transform.localPosition.y-(damage * heigthCons) >= defHeight)
        {
            //print(spine.transform.position.y - (damage * heigthCons) + "--" + defHeight);
            ChangePlayerHeight(false, damage);
        }
        else
        {
            height = defHeight;
            StartCoroutine(HeightAnim(false, height));
            StartPlayerColorAnim(false);

            ChangePlayerWidth(false, damage);
        }
    }

    void ThrowBroken(Vector3 spawnPoint)
    {
        spawnPoint.x = player.transform.position.x;
        spawnPoint.z = player.transform.position.z;

        GameObject brokenPart = Instantiate(brokenCyl, spawnPoint, Quaternion.identity);

        float r = arms.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(0) > 0 ? arms.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(0) * 0.003118f : 0.25f;

        brokenPart.transform.localScale = new Vector3(r, brokenPart.transform.localScale.y, r);

        brokenPart.AddComponent<Rigidbody>().AddForce(-Vector3.forward * brokenPartForce, ForceMode.Impulse);
        brokenPart.transform.DORotate(new Vector3(179f, 0, 179f), 3.0f);

        Destroy(brokenPart, 3f);
    }

    public void JumpPlayerTo(Vector3 jumpPoint, float dur)
    {
        player.GetComponent<Rigidbody>().useGravity = false;

        controller = false;
        playerCurrentSpeed = 0;
        player.transform.rotation = Quaternion.Euler(Vector3.zero);

        jumpPoint.y = player.transform.position.y+ 0.01f;

        jumpTweener = player.transform.DOJump(jumpPoint, playerJumpPower, 1, dur, false)
            .SetEase(Ease.Linear)
            .OnComplete(WaitForJump);
    }

    public void WaitForJump()
    {
        controller = true;
        director.transform.position = new Vector3(director.transform.position.x, player.transform.position.y + directorOffsY, player.transform.position.z + directorOffsZ);
        player.GetComponent<Rigidbody>().useGravity = true;
    }

    public void ChangePlayerWidth(bool increase, float value)
    {
        if (increase)
        {
            width += value * widthCons;
        }
        else if (arms.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(0) - value * widthCons >= defScale)
        {
            width -= value * widthCons;
        }
        else
        {
            width = defScale;

            Failed();
        }

        StartCoroutine(WidthAnim(increase, width));

        StartPlayerColorAnim(increase);
    }

    void ReshapePlayer(float key)
    {
        arms.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, key);
        body.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, key);
        hips.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, key);
        legs.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, key);
    }
    IEnumerator WidthAnim(bool inc, float targetK)
    {
        if (inc)
        {
            while (arms.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(0) < targetK)
            {
                float tempKey = Mathf.MoveTowards(arms.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(0), targetK, widthAnimSens * Time.deltaTime);
                ReshapePlayer(tempKey);
                yield return null;
            }
        }
        else
        {
            while (arms.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(0) > targetK)
            {
                float tempKey = Mathf.MoveTowards(arms.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(0), targetK, widthAnimSens * Time.deltaTime);
                ReshapePlayer(tempKey);
                yield return null;
            }
        }
    }

    public void ChangePlayerHeight(bool increase, float value)
    {
        if (increase)
        {
            height += value * heigthCons;
            SetPlayerCollider(value * heigthCons);
        }
        else if (spine.transform.localPosition.y - value * heigthCons >= defHeight)
        {
            height -= value * heigthCons;
            SetPlayerCollider(-value * heigthCons);
        }
        else
        {
            height = defHeight;

            Failed();
        }

        StartCoroutine(HeightAnim(increase, height));
        StartPlayerColorAnim(increase);
    }

    void SetPlayerCollider(float addValue)
    {
        player.GetComponent<CapsuleCollider>().height += addValue;
        player.GetComponent<CapsuleCollider>().center += Vector3.up * addValue/2;
    }

    IEnumerator HeightAnim(bool inc, float targetY)
    {
        if(inc)
        {
            while (spine.transform.localPosition.y < targetY)
            {
                Vector3 spineTargetPos = new Vector3(spine.transform.localPosition.x, targetY, spine.transform.localPosition.z);

                spine.transform.localPosition = Vector3.MoveTowards(spine.transform.localPosition, spineTargetPos, heightAnimSens * Time.fixedDeltaTime);

                yield return null;
            }
        }
        else
        {
            while (spine.transform.localPosition.y > targetY)
            {
                Vector3 spineTargetPos = new Vector3(spine.transform.localPosition.x, targetY, spine.transform.localPosition.z);

                spine.transform.localPosition = Vector3.MoveTowards(spine.transform.localPosition, spineTargetPos, heightAnimSens * Time.fixedDeltaTime);

                yield return null;
            }
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
        if(!isFinished)
        {
            player.transform.LookAt(director.transform.position);
        }
        else
        {
            player.transform.eulerAngles = Vector3.zero;
        }
    }

    void MoveCamera()
    {
        Vector3 camTargetPoint = player.transform.position - camOffset;
        cam.transform.position = camTargetPoint;
    }

    void Failed()
    {
        controller = false;
        //Time.timeScale = 0.5f;

        if(isFinished)
        {
            finishPanel.SetActive(true);
            InvokeRepeating("BonusAnim", 0, Time.fixedDeltaTime);
        }
        else
        {
            failPanel.SetActive(true);
        }
    }

    void BonusAnim()
    {
        int bonusPoint = (int)(finalDiaCount * finalMultiplier);
        bonusAnimElapsedT += Time.deltaTime;

        if (bonusAnimElapsedT < bonusAnimDuration)
        {
            float newValue = Mathf.Lerp(0, bonusPoint, bonusAnimElapsedT / bonusAnimDuration);
            finishPanel.transform.GetChild(3).GetComponent<Text>().text = ((int)newValue).ToString();
        }
        else
        {
            finishPanel.transform.GetChild(3).GetComponent<Text>().text = bonusPoint.ToString();
            CancelInvoke("BonusAnim");
        }
    }

    public void FinishJumped()
    {
        isFinished = true;
        InvokeRepeating("SetCameraPositionToFinish", 0, Time.deltaTime);
    }

    void SetCameraPositionToFinish()
    {
        camOffset = Vector3.MoveTowards(camOffset, newCamOffset, cameraFinalPosSens * Time.deltaTime);
        cam.transform.LookAt(player.transform.position + Vector3.up * cameraFinalYOffs);

        if(camOffset == newCamOffset)
        {
            CancelInvoke("SetCameraPositionToFinish");
        }
    }

    void AnimatorController(int action)
    {
        switch (action)
        {
            case 0: // idle
                playerAnimator.SetTrigger("Idle");
                break;
            case 1: // drop 
                playerAnimator.SetTrigger("Drop");
                break;
            case 2: // run 
                playerAnimator.SetTrigger("Run");
                break;
        }
    }

    // Reload the current scene to restart the game
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
