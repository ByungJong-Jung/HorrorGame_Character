using UnityEngine;

public class HeadbobHandler
{
    private readonly Transform _cameraTransform;
    private readonly PlayerCameraController.HeadbobSettings _settings;
    private readonly float _defaultYPos;
    private float _bobTimer;

    public HeadbobHandler(Transform cameraTransform, Transform heightReference, PlayerCameraController.HeadbobSettings settings)
    {
        _cameraTransform = cameraTransform;
        _settings = settings;
        _defaultYPos = heightReference.localPosition.y;
    }

    public void Apply(PlayerCameraController.HeadbobSettings.MovementType type)
    {
        float speed = _settings.GetSBobSpeed(type);
        float amount = _settings.GetBobAmount(type);
        _bobTimer += Time.deltaTime * speed;
        _cameraTransform.localPosition = _cameraTransform.localPosition.SetY(_defaultYPos + Mathf.Sin(_bobTimer) * amount);
    }
}
