using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private float moveSpeed = 0.5f;
    private float lookSpeedX = 3f;
    private float lookSpeedY = -3f;

    // Update is called once per frame
    void Update () {
        
        if (Input.GetKey("up"))
        {
            transform.Translate(Vector3.forward * moveSpeed);
        }
        if (Input.GetKey("down"))
        {
            transform.Translate(Vector3.back * moveSpeed);
        }
        if (Input.GetKey("right"))
        {
            transform.Translate(Vector3.right * moveSpeed);
        }
        if (Input.GetKey("left"))
        {
            transform.Translate(Vector3.left * moveSpeed);
        }

        float y = Input.GetAxis("Mouse X");
        float x = Input.GetAxis("Mouse Y");
        transform.eulerAngles = transform.eulerAngles - new Vector3(0, y * lookSpeedY, 0);
    }
}
