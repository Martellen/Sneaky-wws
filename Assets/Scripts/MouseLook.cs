using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float mouseAxisX;
    private float mouseAxisY;
    private float xCameraRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerBody = gameObject.GetComponentInParent<Transform>().parent;
    }

    // Update is called once per frame
    void Update()
    {
        mouseAxisX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseAxisY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xCameraRotation -= mouseAxisY;
        xCameraRotation = Mathf.Clamp(xCameraRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xCameraRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseAxisX);

    }
}
