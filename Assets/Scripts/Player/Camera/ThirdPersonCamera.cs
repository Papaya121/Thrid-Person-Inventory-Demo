using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float _distance = 4f;
    [SerializeField] private float _height = 2f;
    [SerializeField] private float _sensitivity = 2f;

    [Header("Rotation Limits")]
    [SerializeField] private float _minPitch = -30f;
    [SerializeField] private float _maxPitch = 60f;

    [Header("References")]
    [SerializeField] private Transform _target;
    [SerializeField] private PlayerInput _playerInput;

    private float _yaw;
    private float _pitch;

    private Vector2 _lookInput;

    private void OnEnable()
    {
        if (!_playerInput) return;
        _playerInput.LookChanged += OnLook;
    }

    private void OnDisable()
    {
        if (!_playerInput) return;
        _playerInput.LookChanged -= OnLook;
    }

    private void OnLook(Vector2 look)
    {
        _lookInput = look;
    }

    private void LateUpdate()
    {
        if (!_target || Time.timeScale == 0) return;

        RotateCamera();
        UpdatePosition();
    }

    private void RotateCamera()
    {
        _yaw += _lookInput.x * _sensitivity;
        _pitch -= _lookInput.y * _sensitivity;

        _pitch = Mathf.Clamp(_pitch, _minPitch + 0.01f, _maxPitch - 0.01f);
    }

    private void UpdatePosition()
    {
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);

        Vector3 offset = rotation * new Vector3(0, 0, -_distance);
        Vector3 finalPosition = _target.position + Vector3.up * _height + offset;

        transform.position = finalPosition;
        transform.LookAt(_target.position + Vector3.up * _height);
    }
}
