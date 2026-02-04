using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _gravity = -9.81f;

    [Header("References")]
    [SerializeField] private PlayerInput _playerInput;

    private CharacterController _controller;
    private Vector2 _moveInput;

    private Vector3 _velocity; 
    private bool _isGrounded;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        if (!_playerInput) throw new NullReferenceException("Player Input is not assigned!");
        _playerInput.MoveChanged += OnMoveInput;
    }

    private void OnDisable()
    {
        _playerInput.MoveChanged -= OnMoveInput;
    }

    private void Update()
    {
        _isGrounded = _controller.isGrounded;

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        Move();
        ApplyGravity();
    }

    private void OnMoveInput(Vector2 move)
    {
        _moveInput = move;
    }

    private void Move()
    {
        Vector3 direction = transform.right * _moveInput.x + transform.forward * _moveInput.y;

        _controller.Move(direction * _moveSpeed * Time.deltaTime);

        _controller.Move(_velocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (_isGrounded)
        {
            if (_velocity.y < 0)
            {
                _velocity.y = -2f;
            }
        }
        else
        {
            _velocity.y += _gravity * Time.deltaTime;
        }
    }
}
