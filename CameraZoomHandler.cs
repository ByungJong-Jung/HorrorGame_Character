using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraZoomHandler
{
    private readonly MonoBehaviour _coroutineRunner;
    private readonly Camera _camera;
    private readonly PlayerCameraController.CameraSettings _settings;
    private readonly float _defaultFieldOfView;
    private Coroutine _zoomRoutine;

    public bool IsZooming { get; private set; }
    public event Action OnZoomEnter;
    public event Action OnZoomExit;

    public CameraZoomHandler(MonoBehaviour coroutineRunner, Camera camera, PlayerCameraController.CameraSettings settings)
    {
        _coroutineRunner = coroutineRunner;
        _camera = camera;
        _settings = settings;
        _defaultFieldOfView = camera.fieldOfView;
    }

    public void Tick()
    {
        if (InputManager.Instance.IsZoomDown())
        {
            IsZooming = true;
            OnZoomEnter?.Invoke();
            RestartRoutine(true);
        }

        if (InputManager.Instance.IsZoomUp())
        {
            IsZooming = false;
            OnZoomExit?.Invoke();
            RestartRoutine(false);
        }
    }

    private void RestartRoutine(bool zoomIn)
    {
        if (_zoomRoutine != null)
        {
            _coroutineRunner.StopCoroutine(_zoomRoutine);
            _zoomRoutine = null;
        }
        _zoomRoutine = _coroutineRunner.StartCoroutine(Co_HandleZoom(zoomIn));
    }

    private IEnumerator Co_HandleZoom(bool inIsZoom)
    {
        float targetFOV = inIsZoom ? _settings.zoomFieldOfView : _defaultFieldOfView;
        float startingFOV = _camera.fieldOfView;
        float timeElapsed = 0f;
        float duration = _settings.timeToZoom;

        Slider zoomSlider = MainUI.Instance.CameraUI.CameraCapture.zoomSlider;
        float sliderStart = zoomSlider != null ? zoomSlider.value : 0f;
        float sliderTarget = inIsZoom ? 1f : 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            _camera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, t);
            if (zoomSlider != null)
                zoomSlider.value = Mathf.Lerp(sliderStart, sliderTarget, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _camera.fieldOfView = targetFOV;
        _zoomRoutine = null;
    }
}
