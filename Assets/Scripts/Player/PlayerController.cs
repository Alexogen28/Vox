using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] Transform cameraTransform;

    [Header("Components")]
    public PlayerMovement movement;
    public PlayerInputs inputs;
    public PlayerActions actions;
    public Health health;


    public void GetAllComponents()
    {
        movement = GetComponent<PlayerMovement>();
        health = GetComponent<Health>();
        inputs = GetComponent<PlayerInputs>();
        actions = GetComponent<PlayerActions>();
    }

    void Update()
    {
        HandleMovement();
        HandleActions();
    }

    private void HandleActions()
    {
        actions.SelfInflictDamage(inputs.selfDamagePressed);
        actions.FireWeapon(inputs.attackPressed);
        actions.ReloadWeapon(inputs.reloadPressed);
    }
    

    private void HandleMovement()
    {
        movement.CheckCharacterGrounded();
        movement.CheckCharacterMovement(inputs.keyboardInput, cameraTransform);
        movement.CheckJumpAndJump(inputs.jumpPressed);
        movement.ApplyGravity();
        movement.ApplyFinalMovement();
        inputs.jumpPressed = false;
    }

    public void TeleportPlayer(Vector3 coords)
    {
        transform.position = coords;
    }
}
