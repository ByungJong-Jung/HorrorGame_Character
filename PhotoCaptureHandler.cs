public class PhotoCaptureHandler
{
    private readonly PlayerCameraController _owner;
    public bool InCaptureMode { get; private set; }

    public PhotoCaptureHandler(PlayerCameraController owner)
    {
        _owner = owner;
    }

    public void EnterCaptureMode()
    {
        InCaptureMode = true;
        MainUI.Instance.CursorUI.SetCursorActive(true);
    }

    public void ExitCaptureMode()
    {
        InCaptureMode = false;
        MainUI.Instance.CursorUI.SetCursorActive(false);
    }

    public void Tick()
    {
        if (InputManager.Instance.IsShutterDown() == false)
            return;

        if (InCaptureMode)
            HandleCaptureClick();
        else
            HandleNonCaptureClick();
    }

    private void HandleCaptureClick()
    {
        if (!_owner.ReadyTakePhoto && !_owner.nowTakePhoto)
        {
            MainUI.Instance.CameraUI.CameraCapture.ClearPhotoCamera();
            MainUI.Instance.CursorUI.SetCursorActive(true);
            _owner.ReadyTakePhoto = true;
        }
        else if (_owner.ReadyTakePhoto && !_owner.nowTakePhoto)
        {
            _owner.ReadyTakePhoto = false;
            _owner.nowTakePhoto = true;
            MainUI.Instance.CameraUI.CameraCapture.TakePhoto(
                () => MainUI.Instance.CursorUI.SetCursorActive(false),
                (photoItemID, photoSprite) =>
                {
                    PhotoViewHelper.Instance.AddPhotos(photoItemID, photoSprite);
                    _owner.nowTakePhoto = false;
                });
        }
    }

    private void HandleNonCaptureClick()
    {
        if (!_owner.ReadyTakePhoto && !_owner.nowTakePhoto)
        {
            MainUI.Instance.CameraUI.CameraCapture.ClearPhotoCamera();
            _owner.ReadyTakePhoto = true;
        }
    }
}
