using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    public float panSpeed = 2f;

    void Update()
    {
        HandlePanInput();
    }

    void HandlePanInput()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            Vector3 moveVector = new Vector3(-touchDeltaPosition.x, -touchDeltaPosition.y, 0) * panSpeed * Time.deltaTime;
            transform.Translate(moveVector, Space.World);
        }
    }
}
