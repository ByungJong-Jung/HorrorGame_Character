using UnityEngine;

public class CameraLookHandler
{
    private readonly Transform _cameraHolder;
    private readonly Transform _body;
    private readonly PlayerCameraController.CameraSettings _settings;
    private float _rotationX;

    public CameraLookHandler(Transform cameraHolder, Transform body, PlayerCameraController.CameraSettings settings)
    {
        _cameraHolder = cameraHolder;
        _body = body;
        _settings = settings;
    }

    public void Tick()
    {
        _rotationX -= Input.GetAxis("Mouse Y") * _settings.lookSpeedY;
        _rotationX = Mathf.Clamp(_rotationX, -_settings.upperLookLimit, _settings.lowerLookLimit);
        _cameraHolder.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        _body.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * _settings.lookSpeedX, 0);
    }
}
