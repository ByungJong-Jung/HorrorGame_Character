using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HeadbobSettings = PlayerCameraController.HeadbobSettings;
public class PlayerMovementController : MonoBehaviour
{
    #region Singleton
    public static PlayerMovementController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Serializable]
    public class MovementSettings
    {
        public float walkSpeed = 3f;
        public float runSpeed = 6f;
        public float crouchSpeed = 1.5f;

        public float GetSpeed(MovementType type)
        {
            return type switch
            {
                MovementType.Walk => walkSpeed,
                MovementType.Run => runSpeed,
                MovementType.Crouch => crouchSpeed,
                _ => walkSpeed
            };
        }

        public enum MovementType
        {
            Walk,
            Run,
            Crouch
        }
    }

    [Serializable]
    public class GravitySettings
    {
        public float groundYOffset;
        public LayerMask groundMask;
        public float gravity = -9.81f;
        public float jumpForce = 3f;
    }
    private Vector3 _velocity;
    public bool IsJumped { get; set; }

    [Serializable]
    public class CrouchSettings
    {
        public float crouchingHeight = 1.25f;
        public float standingHeight = 2f;
        public Vector3 crouchingCenter = new Vector3(0f, -0.37f, 0f);
        public Vector3 standingCenter = new Vector3(0f, -0.1f, 0f);
        public float crouchCameraPosY = 0.2f;
        public float standCameraPosY = 0.7f;

        public float timeToCrouch = 0.15f;
    }

    [SerializeField] private PlayerInteractionController _playerInteractionController;
    [SerializeField] private PlayerCameraController _playerCameraController;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private AnimatorController _animatorController;
    [SerializeField] private Camera _characterCamera;
    [SerializeField] private Transform _cameraRoot;
    [SerializeField] private Transform _cameraHolder;
    public MovementSettings MovementSetting;
    public CrouchSettings CrouchSetting;
    public GravitySettings GravitySetting;
    private Coroutine crouchAndStandRoutine;

    public bool IsCrouching { get; set; }

    public PlayerInteractionController PlayerInteractionController => _playerInteractionController;
    public AnimatorController AnimatorController => _animatorController;

    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public RunState Run = new RunState();
    public CrouchState Crouch = new CrouchState();

    private MovementBaseState _currentState;

    private bool _isInitialized = false;
    private bool _isStarted = false;

    public IEnumerator Co_Initialize()
    {
        _characterController.enabled = false;
        _playerInteractionController.enabled = false;
        _playerCameraController.enabled = false;

        SwitchState(Idle);
        _playerCameraController.Init();

        yield return null;

        _isInitialized = true;
    }

    public void StartGameplay(Vector3 inSpawnPos, bool isTestScene = false)
    {
        if (!_isInitialized)
        {
            Debug.LogWarning("Player not initialized yet.");
            return;
        }

        if (_isStarted)
            return;

        if (!isTestScene)
            transform.position = inSpawnPos;

        _isStarted = true;
        _characterController.enabled = true;
        _playerInteractionController.enabled = true;
        _playerCameraController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Player Movement Started");
    }


    private void Update()
    {
        if (!_isInitialized || !_isStarted)
            return;

        if (GameManager.Instance.IsPaused)
            return;

        _currentState.UpdateState(this);

        ApplyGravity();
        ApplyCrouching();

        Ray ray = _characterCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        _playerInteractionController.HandleInteraction(ray);
        //HandleInteraction();
    }

    public void SwitchState(MovementBaseState inState)
    {
        _currentState?.ExitState(this);
        _currentState = inState;
        _currentState?.EnterState(this);
    }

    public void Move(Vector3 direction, float speed, HeadbobSettings.MovementType inType)
    {
        Vector3 dir = transform.forward * direction.y + transform.right * direction.x;
        _characterController.Move(dir * speed * Time.deltaTime);

        _animatorController.PlayMove(dir);

        _playerCameraController.HandleHeadbob(inType);
    }

    public bool IsGrounded()
    {
        Vector3 origin = transform.position + _characterController.center;
        float bottom = origin.y - (_characterController.height / 2f) + _characterController.skinWidth + 0.01f;

        Vector3 checkPosition = new Vector3(origin.x, bottom, origin.z);
        float checkRadius = _characterController.radius - 0.05f;

#if UNITY_EDITOR
        Debug.DrawRay(checkPosition, Vector3.up * 0.2f, Color.green, 1f);
        Debug.DrawRay(checkPosition, Vector3.down * 0.2f, Color.red, 1f);
#endif

        bool isGround = Physics.CheckSphere(checkPosition, checkRadius, GravitySetting.groundMask);
        return isGround;
    }

    public void ApplyGravity()
    {
        if (!IsGrounded())
        {
            _velocity.y += GravitySetting.gravity * Time.deltaTime;
        }
        else if (_velocity.y < 0f)
        {
            _velocity.y = -2f;
        }

        _characterController.Move(_velocity * Time.deltaTime);
    }

    public void HandleJump()
    {
        _velocity.y = 0f;
        _velocity.y += GravitySetting.jumpForce;
    }

    public void ApplyCrouching()
    {
        if (InputManager.Instance.IsCrouching() && !IsCrouching)
        {
            IsCrouching = true;

            if (crouchAndStandRoutine != null)
            {
                StopCoroutine(crouchAndStandRoutine);
                crouchAndStandRoutine = null;
            }

            crouchAndStandRoutine = StartCoroutine(Co_SmoothCrouchAndStandTransition(true));
        }
        else if (!InputManager.Instance.IsCrouching() && CanStand() && IsCrouching)
        {
            IsCrouching = false;
            if (crouchAndStandRoutine != null)
            {
                StopCoroutine(crouchAndStandRoutine);
                crouchAndStandRoutine = null;
            }

            crouchAndStandRoutine = StartCoroutine(Co_SmoothCrouchAndStandTransition(false));
        }
    }

    private IEnumerator Co_SmoothCrouchAndStandTransition(bool inIsCrouching)
    {
        float targetHeight = inIsCrouching ? CrouchSetting.crouchingHeight : CrouchSetting.standingHeight;
        Vector3 targetCenter = inIsCrouching ? CrouchSetting.crouchingCenter : CrouchSetting.standingCenter;
        float targetCameraPosY = inIsCrouching ? CrouchSetting.crouchCameraPosY : CrouchSetting.standCameraPosY;

        float timeElapsed = 0f;
        float timeToCrouch = CrouchSetting.timeToCrouch;
        float cameraPosY = _cameraRoot.localPosition.y;
        while (timeElapsed < CrouchSetting.timeToCrouch)
        {
            yield return null;

            _characterController.height = Mathf.Lerp(_characterController.height, targetHeight, timeElapsed / timeToCrouch);
            _characterController.center = Vector3.Lerp(_characterController.center, targetCenter, timeElapsed / timeToCrouch);

            cameraPosY = Mathf.Lerp(cameraPosY, targetCameraPosY, timeElapsed / timeToCrouch);
            _cameraRoot.localPosition = _cameraRoot.localPosition.SetY(cameraPosY);

            timeElapsed += Time.deltaTime;
        }

        _characterController.height = targetHeight;
        _characterController.center = targetCenter;
        _cameraRoot.localPosition = _cameraRoot.localPosition.SetY(targetCameraPosY);
        crouchAndStandRoutine = null;
    }

    private bool CanStand()
    {
        return !Physics.Raycast(transform.position, Vector3.up, 1f);
    }
}
