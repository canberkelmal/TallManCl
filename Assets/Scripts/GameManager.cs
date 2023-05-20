using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using static Sirenix.OdinInspector.Editor.Internal.FastDeepCopier;
using Unity.VisualScripting;

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



    [Title("Scene Objects")]
    [TabGroup("GameObjects")]
    [SceneObjectsOnly]
    public GameObject camera;
    [TabGroup("GameObjects")]
    [SceneObjectsOnly]
    public GameObject player;
    [TabGroup("GameObjects")]
    [SceneObjectsOnly]
    public GameObject director;

    float directorOffsZ = 1f;
    float playerCurrentSpeed = 0f;
    float cameraOffsZ;

    void Start()
    {
        cameraOffsZ = player.transform.position.z - camera.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            UpdateDirectorPositionX();
            MovePlayer(true);
        }
        else if(playerCurrentSpeed > 0)
        {
            director.transform.position = new Vector3(director.transform.position.x, director.transform.position.y, player.transform.position.z + directorOffsZ);
            UpdatePlayerRotationY();
            MovePlayer(false);
        }
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
        MoveCamera();
    }

    void MoveCamera()
    {
        camera.transform.position = new Vector3(player.transform.position.x, camera.transform.position.y, player.transform.position.z - cameraOffsZ);
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
        }
        else if(player.transform.GetChild(1).localScale.x - value * 0.01f >= 0.2f)
        {
            player.transform.GetChild(1).localScale -= new Vector3(value * 0.01f, 0, value * 0.01f);
            player.transform.GetChild(2).localScale -= new Vector3(value * 0.01f, 0, value * 0.01f);
        }
        else
        {
            player.transform.GetChild(1).localScale = new Vector3(0.2f, player.transform.GetChild(1).localScale.y, 0.2f);
            player.transform.GetChild(2).localScale = new Vector3(0.2f, player.transform.GetChild(2).localScale.y, 0.2f);
        }
    }
    public void ChangePlayerHeight(bool increase, float value)
    {
        if (increase)
        {
            player.transform.GetChild(1).localScale += new Vector3(0, value * 0.01f, 0);
        }
        else if (player.transform.GetChild(1).localScale.y - value * 0.01f >= 1f)
        {
            player.transform.GetChild(1).localScale -= new Vector3(0, value * 0.01f, 0);
        }
        else
        {
            player.transform.GetChild(1).localScale = new Vector3(player.transform.GetChild(1).localScale.x, 1, player.transform.GetChild(1).localScale.z);
        }

    }
}
