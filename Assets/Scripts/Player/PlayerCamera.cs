using UnityEngine;
using UnityEngine.Windows;

public class PlayerCamera : MonoBehaviour
{

    [SerializeField] Transform playerBody;
    [SerializeField] float verticalClampMin = -90f;
    [SerializeField] float verticalClampMax = 90f;

    private PlayerInputs inputs;
    private float xRotation = 0;


    void Awake()
    {
        inputs = playerBody.GetComponent<PlayerInputs>();

        transform.position = playerBody.transform.position;
        transform.rotation = playerBody.transform.rotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
    }

    private void HandleMouseLook()
    {
        float _mouseX = inputs.mouseX * inputs.mouseSensitivity;
        float _mouseY = inputs.mouseY * inputs.mouseSensitivity;

        xRotation -= _mouseY;
        xRotation = Mathf.Clamp(xRotation, verticalClampMin, verticalClampMax);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * _mouseX);
    }
}
