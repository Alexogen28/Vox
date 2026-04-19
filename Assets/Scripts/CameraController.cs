using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera parameters")]
    [SerializeField] public Transform playerPosition;
    [SerializeField] public Vector3 cameraOffset = new Vector3(0, 3, -6);
    [SerializeField] public float mouseSensitivity = 3f;
    [SerializeField] public float pitchClamp = 85f;

    private float pitch = 0f;

    private void Update()
    {
        // Vertical look input (mouse Y)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);
    }

    private void LateUpdate()
    {
        // Rotate the offset by pitch (X) and player yaw (Y)
        Quaternion yawRotation = Quaternion.Euler(0f, playerPosition.eulerAngles.y, 0f);
        Quaternion pitchRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Combined rotation
        Quaternion finalRotation = yawRotation * pitchRotation;

        // Apply offset in camera space
        Vector3 finalOffset = finalRotation * cameraOffset;

        // Position the camera
        transform.position = playerPosition.position + finalOffset;

        // Look at player (eye level)
        transform.LookAt(playerPosition.position + Vector3.up * 1.0f);
    }
}
