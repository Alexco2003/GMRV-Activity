using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float lookSensitivity = 2f;

    private float rotationX;
    private float rotationY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 initialRotation = transform.localEulerAngles;

        rotationX = initialRotation.y;
        rotationY = initialRotation.x;

        if (rotationY > 180f)
        {
            rotationY -= 360f;
        }
    }

    void Update()
    {
        rotationX += Input.GetAxis("Mouse X") * lookSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * lookSensitivity;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");  
        float moveY = 0f;

        if (Input.GetKey(KeyCode.E)) moveY = 1f; 
        if (Input.GetKey(KeyCode.Q)) moveY = -1f;

        Vector3 moveDir = transform.right * moveX + transform.up * moveY + transform.forward * moveZ;

        transform.position += moveDir * moveSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}