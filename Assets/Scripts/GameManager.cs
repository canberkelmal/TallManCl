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

    float directorOffsZ = 3f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            UpdateDirectorPositionX();
        }
    }
    void UpdateDirectorPositionX()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float moveAmount = mouseX * playerRotateSens;
        Vector3 newPosX = director.transform.position + director.transform.right * moveAmount;
        newPosX.x = Mathf.Clamp(newPosX.x, xMin, xMax);
        newPosX.z = player.transform.position.z + directorOffsZ;
        newPosX.y = player.transform.position.y;
        director.transform.position = newPosX;
        UpdatePlayerRotationY();
    }
    void UpdatePlayerRotationY()
    {
        player.transform.LookAt(director.transform.position);
    }
}
