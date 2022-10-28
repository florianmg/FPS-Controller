using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// https://www.youtube.com/watch?v=-XNm7dPVVOQ&list=PLfhbBaEcybmgidDH3RX_qzFM0mIxWJa21&index=4

public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldCrounch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.C;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 100)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 100)] private float lowerLookLimit = 80.0f;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0.0f, 0.5f, 0.0f);
    [SerializeField] private Vector3 standingCenter = new Vector3(0.0f, 0f, 0.0f);
    private bool isCrouching;
    private bool duringCrouchAnimation;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0.0f;

    // Start is called before the first frame update
    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();    
        characterController = GetComponent<CharacterController>();  
        Cursor.lockState = CursorLockMode.Locked;   
        Cursor.visible = false; 
    }

    // Update is called once per frame
    void Update()
    {
        if(CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if (canJump) HandleJump();
            if (canCrouch) HandleCrouch();
                
            ApplyFinalMovements();
        }
    }

    private void HandleMovementInput()
    {
        float speed = isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed;
        currentInput = new Vector2(speed * Input.GetAxis("Vertical"), speed * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
        transform.rotation *= Quaternion.Euler(0.0f, Input.GetAxis("Mouse X") * lookSpeedX, 0.0f);
    }

    private void HandleJump()
    {
        if (!ShouldJump) return;
        moveDirection.y = jumpForce;
    }

    private void HandleCrouch()
    {
        if (!ShouldCrounch) return;
        StartCoroutine(CrouchStand());
    }

    private void ApplyFinalMovements()
    {
        if(!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {

        if(isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1)) yield break;
        
        duringCrouchAnimation = true;

        float timePassed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while(timePassed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timePassed/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timePassed / timeToCrouch);

            timePassed += Time.deltaTime;

            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }
}
