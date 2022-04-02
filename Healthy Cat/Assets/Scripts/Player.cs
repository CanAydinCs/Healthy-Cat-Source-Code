using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("MOVEMENTS")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 5;
    [SerializeField] private float gravity = -9.8f;

    private CharacterController controller;
    private PlayerActions actions;
    private InputAction movement;

    private float yValue = 0;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = .4f;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded = true;

    [Space]
    [Header("MODEL VIS")]
    [SerializeField] private Transform model;
    private Animator animator;
    private int isWalkingHash = 0;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        actions = new PlayerActions();

        animator = model.GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
    }

    private void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    private void HandleAnimation()
    {
        bool isWalking = movement.ReadValue<Vector2>() != Vector2.zero;
        animator.SetBool(isWalkingHash, isWalking);
    }

    private void HandleMovement()
    {
        Vector3 direction = new Vector3(movement.ReadValue<Vector2>().x, yValue, movement.ReadValue<Vector2>().y);

        yValue += gravity * Time.deltaTime;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);

        if (isGrounded && yValue <= 0) 
        {
            yValue = -2f;
        }

        controller.Move(direction * speed * Time.deltaTime);

        ModelRotation();
    }

    private void ModelRotation()
    {
        Vector2 dir = movement.ReadValue<Vector2>();
        if (dir == Vector2.zero) return;

        float degree = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

        model.rotation = Quaternion.Euler(0, degree, 0);
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        if (isGrounded)
        {
            yValue = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void OnEnable()
    {
        actions.Enable();
        movement = actions.Character.Movement;

        actions.Character.Jump.performed += Jump_performed;
    }
    private void OnDisable()
    {
        actions.Disable();
    }
}
