using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPos;
    public Vector3 offset;
    public bool y = true;

    private void Update()
    {
        if (y)
            transform.position = cameraPos.position + offset;
        else
            transform.position = new Vector3(cameraPos.position.x + offset.x, transform.position.y, cameraPos.position.z + offset.z);
    }
}
