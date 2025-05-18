using System;
using UnityEngine;

[RequireComponent((typeof(CharacterController)))]
public class Move : MonoBehaviour
{
    public float speed;
    public float runMultiplier;
    public float gravity = 9.81f;
    public float jumpHeight;
    public float rotationSpeed;

    private CharacterController _characterController;
    private Vector3 _velocity;
    private bool _isRunning = false;
    private bool _isGrounded;
    private InputSystemActions _inputActions;
    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _inputActions = new InputSystemActions();
        _inputActions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += _ => _moveInput = Vector2.zero;

        _inputActions.Player.Sprint.performed += _ => _isRunning = true;
        _inputActions.Player.Sprint.canceled += _ => _isRunning = false;

        _inputActions.Player.Jump.performed += _ => Jump();

        _inputActions.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.Look.canceled += _ => _lookInput = new Vector2(0, 0);
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    void Update()
    {
        _isGrounded = _characterController.isGrounded;
        if (_isGrounded && _velocity.y <= 0)
        {
            _velocity.y = -2.0f;
        }

        var move = new Vector3(_moveInput.x, 0, _moveInput.y);
        move = transform.TransformDirection(move);
        var currentSpeed = _isRunning ? speed * runMultiplier : speed;

        _characterController.Move(move * (currentSpeed * Time.deltaTime));
        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);

        Rotate();
    }

    private void Rotate()
    {
        if (_lookInput is { x: 0, y: 0 }) return;
        var rotation = new Vector3(0, _lookInput.x * rotationSpeed * Time.deltaTime, 0);
        transform.Rotate(rotation);
    }

    private void Jump()
    {
        if (_isGrounded)
            _velocity.y = (float)Math.Sqrt(jumpHeight * -2.0f * gravity);
    }
}