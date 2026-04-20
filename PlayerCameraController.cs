using System;
using System.Collections;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public Camera CharacterCamera => _characterCamera;
    [SerializeField] private Camera _characterCamera;
    [SerializeField] private Transform _cameraHolder;

    [Serializable]
    public class CameraSettings
    {
        public float lookSpeedX = 2f;
        public float lookSpeedY = 2f;
        public float upperLookLimit = 80f;
        public float lowerLookLimit = 80f;
        public float timeToZoom = 0.5f;
        public float zoomFieldOfView = 30f;
    }

    [Serializable]
    public class HeadbobSettings
    {
        public float walkBobSpeed = 12f;
        public float walkBobAmount = 0.02f;
        public float runBobSpeed = 16f;
        public float runBobAmount = 0.035f;
        public float crouchBobSpeed = 6f;
        public float crouchBobAmount = 0.005f;

        public float GetSBobSpeed(MovementType type)
        {
            return type switch
            {
                MovementType.Walk => walkBobSpeed,
                MovementType.Run => runBobSpeed,
                MovementType.Crouch => crouchBobSpeed,
                _ => walkBobSpeed
            };
        }

        public float GetBobAmount(MovementType type)
        {
            return type switch
            {
                MovementType.Walk => walkBobAmount,
                MovementType.Run => runBobAmount,
                MovementType.Crouch => crouchBobAmount,
                _ => walkBobAmount
            };
        }

        public enum MovementType
        {
            Walk,
            Run,
            Crouch
        }
    }

    public CameraSettings CameraSetting;
    public HeadbobSettings headbobSettings;

    public bool ReadyTakePhoto = true;
    public bool nowTakePhoto = false;

    public bool NowCameraCaptureMode => _photoCapture != null && _photoCapture.InCaptureMode;

    private CameraLookHandler _look;
    private HeadbobHandler _headbob;
    private CameraZoomHandler _zoom;
    private PhotoCaptureHandler _photoCapture;

    public void Init()
    {
        _look         = new CameraLookHandler(_cameraHolder, transform, CameraSetting);
        _headbob      = new HeadbobHandler(_characterCamera.transform, _cameraHolder, headbobSettings);
        _zoom         = new CameraZoomHandler(this, _characterCamera, CameraSetting);
        _photoCapture = new PhotoCaptureHandler(this);

        _zoom.OnZoomEnter += _photoCapture.EnterCaptureMode;
        _zoom.OnZoomExit  += _photoCapture.ExitCaptureMode;

        StartCoroutine(Co_UpdateRoutine());
    }

    private IEnumerator Co_UpdateRoutine()
    {
        while (true)
        {
            yield return null;

            if (GameManager.Instance.IsPaused)
                continue;

            _zoom?.Tick();
            _photoCapture?.Tick();
        }
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.IsPaused)
            return;

        _look?.Tick();
    }

    public void HandleHeadbob(HeadbobSettings.MovementType inType) => _headbob?.Apply(inType);
}
