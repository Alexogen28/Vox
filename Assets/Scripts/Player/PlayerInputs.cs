using System;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    [Header("Input Tuning")]
    [SerializeField] public float mouseSensitivity = 1.0f;

    private bool playerCanAct = true;

    public float mouseX;
    public float mouseY;
    public Vector3 keyboardInput;
    public bool jumpPressed;
    public bool selfDamagePressed;
    public bool attackPressed;
    public bool reloadPressed;

    public static event Action<PlayerInputs> OnInventoryOpen;



    void Update()
    {
        HandlePlayerGameplayInputs();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Tab pressed");
            OnInventoryOpen?.Invoke(this);
            playerCanAct = !playerCanAct;

            if (playerCanAct)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    private void HandlePlayerGameplayInputs()
    {
        keyboardInput = new Vector3(UnityEngine.Input.GetAxis("Horizontal"), 0, UnityEngine.Input.GetAxis("Vertical")).normalized;
        if (playerCanAct)
        {
            mouseX = UnityEngine.Input.GetAxis("Mouse X");
            mouseY = UnityEngine.Input.GetAxis("Mouse Y");
            jumpPressed = UnityEngine.Input.GetKeyDown(KeyCode.Space);

            attackPressed = UnityEngine.Input.GetKeyDown(KeyCode.Mouse0);

            selfDamagePressed = UnityEngine.Input.GetKeyDown(KeyCode.P);
            reloadPressed = UnityEngine.Input.GetKeyDown(KeyCode.R);
        }
    }
}
