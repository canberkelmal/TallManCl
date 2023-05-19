using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
    [Title("Movement")]
    [TabGroup("GamePlay")]
    public float playerMoveSens = 1f;
    [TabGroup("GamePlay")]
    public float playerRotateSens = 1f;
    [TabGroup("GamePlay")]
    public float xMax = 1f;
    [TabGroup("GamePlay")]
    public float xMin = 1f;



    [Title("Scene Objects")]
    [TabGroup("GameObjects")]
    [SceneObjectsOnly]
    public GameObject player;
    [TabGroup("GameObjects")]
    [SceneObjectsOnly]
    public GameObject director;

    float directorOffsZ = 1f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            UpdateDirectorPositionX();
        }
        else
        {
            director.transform.position = new Vector3(director.transform.position.x, director.transform.position.y,  player.transform.position.z + directorOffsZ);
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
        }
        UpdatePlayerRotationY();
    }
    void UpdatePlayerRotationY()
    {
        player.transform.LookAt(director.transform.position);
    }
}
