using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Image;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Adjustments")]
    [SerializeField] float baseMoveSpeed = 5.0f;
    [SerializeField] float baseJumpForce = 1.0f;
    [SerializeField] float gravity = -9.81f;

    [Header("Ground Checking Params")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float raycastDistance = 0.1f;
    [SerializeField] LayerMask groundMask;

    public bool isGrounded;

    public float currentMoveSpeed;
    public float currentJumpForce;


    private CharacterController characterController;
    private Vector3 velocity;

    CapsuleCollider cc;

    void Awake()
    {
        cc = GetComponent<CapsuleCollider>();
        characterController = GetComponent<CharacterController>();

        currentMoveSpeed = baseMoveSpeed;
        currentJumpForce = baseJumpForce;
        velocity = Vector3.zero;
    }

    public void CheckCharacterMovement(Vector3 inputDir, Transform cameraTransform)
    {
        var cameraForward   = cameraTransform.forward;
        var cameraRight     = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDir = (cameraForward * inputDir.z + cameraRight * inputDir.x).normalized;

        velocity.x = moveDir.x * currentMoveSpeed;
        velocity.z = moveDir.z * currentMoveSpeed;
    }

    public void CheckCharacterGrounded()
    {
        isGrounded = false;

        Vector3 ccCenter = cc.bounds.center;
        float ccHeight = cc.bounds.extents.y;
        float ccRadius = cc.bounds.extents.x;

        Vector3 ccBottom = ccCenter - new Vector3(0, ccHeight, 0);

        Vector3[] raycastPoints =
        {
            Vector3.zero,                                       //center point
            Vector3.forward * ccRadius,                         //forward
            -Vector3.forward * ccRadius,                        //backward
            Vector3.right * ccRadius,                           //right
            -Vector3.right * ccRadius,                          //left
            (Vector3.forward + Vector3.right) * ccRadius,       //forward right
            (Vector3.forward - Vector3.right) * ccRadius,       //forward left
            (-Vector3.forward + Vector3.right) * ccRadius,      //back right
            (-Vector3.forward - Vector3.right) * ccRadius       //back left
        };

        foreach (var raycastPoint in raycastPoints)
        {
            if (Physics.Raycast(ccBottom + raycastPoint, Vector3.down, raycastDistance, groundMask))
                isGrounded = true;
            Debug.DrawRay(ccBottom + raycastPoint, Vector3.down * raycastDistance, Color.red);
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;
        }
    }

    public void CheckJumpAndJump(bool jumpInputState)
    {
        if(isGrounded && jumpInputState)
        {
            velocity.y = Mathf.Sqrt(currentJumpForce * -gravity);
        }
    }

    public void ApplyGravity()
    {
            velocity.y += gravity * Time.deltaTime;
            //characterController.Move(new Vector3(0, velocity.y * Time.deltaTime, 0));
    }

    public void ApplyFinalMovement()
    {
        characterController.Move(velocity * Time.deltaTime);
    }
}
