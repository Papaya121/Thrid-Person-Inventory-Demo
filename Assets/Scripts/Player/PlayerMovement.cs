using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;


    [Header("References")]
    [SerializeField] private PlayerInput _playerInput;


    private CharacterController _controller;

    private Vector2 _moveInput;

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
        Move();
    }

    private void OnMoveInput(Vector2 move)
    {
        _moveInput = move;

        print(move);
    }

    private void Move()
    {
        Vector3 direction =
            transform.right * _moveInput.x +
            transform.forward * _moveInput.y;

        _controller.Move(direction * _moveSpeed * Time.deltaTime);
    }
}
