using UnityEngine;

public class MouseLook : MonoBehaviour
{
    #region Class variables
    [SerializeField]
    private float mouseSensitivity = 100f;
    [SerializeField]
    private Transform playerBody;

    private float xAxisRotation;
    private float yAxisRotation;
    private float mouseAxisX;
    private float mouseAxisY;
    private float deltaTime;
    private float xCameraRotation = 0f;
    #endregion

    #region Unity methods
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (!playerBody)
        {
            playerBody = gameObject.GetComponentInParent<Transform>().parent;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
        RotatCamera();
        RotateBody();
    }
    #endregion

    #region Custom methods
    void Initialize()
    {
        deltaTime = Time.deltaTime;
        mouseAxisX = Input.GetAxis("Mouse X");
        mouseAxisY = Input.GetAxis("Mouse Y");
        xAxisRotation = mouseAxisX * mouseSensitivity * deltaTime;
        yAxisRotation = mouseAxisY * mouseSensitivity * deltaTime;

        xCameraRotation -= yAxisRotation;
        xCameraRotation = Mathf.Clamp(xCameraRotation, -90f, 90f);
    }

    void RotatCamera()
    {
        transform.localRotation = Quaternion.Euler(xCameraRotation, 0f, 0f);
    }

    void RotateBody()
    {
        playerBody.Rotate(Vector3.up * xAxisRotation);
    }
    #endregion
}
