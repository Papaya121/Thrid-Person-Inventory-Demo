using System;
using UnityEngine;
using ThirdPersonInventoryDemo.Player.Input;

namespace ThirdPersonInventoryDemo.Player.Movement
{
    public class PlayerRotation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInput _playerInput;

    public float Yaw { get; private set; }

        private Vector2 _lookInput;

        private void OnEnable()
        {
            if (!_playerInput)
                throw new NullReferenceException("Player Input is not assigned!");

            _playerInput.LookChanged += OnLook;
        }


        private void OnDisable()
        {
            _playerInput.LookChanged -= OnLook;
        }

        private void OnLook(Vector2 look) => _lookInput = look;

        private void Update()
        {
            if (Time.timeScale == 0) return;

        Yaw += _lookInput.x;
        transform.rotation = Quaternion.Euler(0f, Yaw, 0f);
    }
}
}
