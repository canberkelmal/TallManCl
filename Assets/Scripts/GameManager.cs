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
    [TabGroup("GamePlay")]
    public float playerJumpDur = 0.8f;


    [Title("Camera")]
    [TabGroup("GameSettings")]
    public float camMoveSens = 1f;


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
    [Title("Materials")]
    [TabGroup("Objects")]
    [AssetsOnly]
    public Material playerMat;


    [Title("Player color change")]
    [TabGroup("Animations")]
    public float playerColorAnimDur = 0.5f;
    [TabGroup("Animations")]
    public Color playerColorPositive, playerColorNegative;
    [Title("Player hit and lose part")]
    [TabGroup("Animations")]
    public float brokenPartForce = 1f;

    public bool controller = true;

    Color playerStartColor, playerTargetColor, playerCurrentColor, playerMainColor;
    float playerColorElapsedT = 0f;
    bool isPlayerColorAnimating = false, playerColorAnimatingPhase = false;

    float directorOffsZ = 1f;
    float playerCurrentSpeed = 0f;
    float cameraOffsZ;
    Vector3 camOffset;

    void Start()
    {
        playerMainColor = playerMat.color;
        playerStartColor = playerMainColor;

        cameraOffsZ = player.transform.position.z - cam.transform.position.z;
        camOffset = player.transform.position - cam.transform.position;
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
            //player.GetComponent<Renderer>().material.color = playerCurrentColor;
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
                //player.GetComponent<Renderer>().material.color = playerMainColor;
                SetPlayerColor(playerMainColor);
            }
        }

        MoveCamera();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    public void HitPlayerAt(Vector3 hitPoint)
    {
        GameObject brokenPart = Instantiate(player.transform.GetChild(1).gameObject, hitPoint, Quaternion.identity);
        brokenPart.transform.localScale = new Vector3(brokenPart.transform.localScale.x, 0.5f, brokenPart.transform.localScale.z);
        brokenPart.AddComponent<Rigidbody>().AddForce(-Vector3.forward * brokenPartForce, ForceMode.Impulse);
        //brokenPart.GetComponent<Rigidbody>().AddForce(-Vector3.forward * brokenPartForce, ForceMode.Impulse);
        brokenPart.transform.DORotate(new Vector3(179f, 0, 179f), 3.0f);
    }

    public void JumpPlayerTo(Vector3 jumpPoint)
    {
        Vector3 targetPoint = new Vector3 (jumpPoint.x, player.transform.position.y, jumpPoint.z);
        controller = false;
        director.transform.position = targetPoint;
        player.transform.DOJump(targetPoint, playerJumpPower, 1, playerJumpDur).OnComplete(WaitForJump);
    }

    void WaitForJump()
    {
        controller = true;
    }

    void SetColorAnimTrig()
    {
        isPlayerColorAnimating = true;
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
        UpdatePlayerRotationY();
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
        }
    }

    void MoveCamera()
    {
        //Vector3 camTargetPoint = new Vector3(player.transform.position.x, cam.transform.position.y, player.transform.position.z - cameraOffsZ);
        Vector3 camTargetPoint = player.transform.position - camOffset;
        cam.transform.position = Vector3.Lerp(cam.transform.position, camTargetPoint, camMoveSens * Time.deltaTime);
    }

    // Reload the current scene to restart the game
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
