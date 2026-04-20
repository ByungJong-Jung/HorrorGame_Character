# 공포 3D 게임 — 플레이어 시스템 코드

> 일본 학교 배경 1인칭 공포 게임 (개발 중)  
> 수상한 현상을 카메라로 촬영하여 기록하는 게임

---

## 📁 이 레포지토리에 대해

전체 프로젝트 중 **플레이어 시스템(Character/)** 코드만 발췌하여 올렸습니다.  
`GameManager`, `MainUI` 등 외부 의존 클래스가 없어 단독 컴파일은 되지 않으며,  
**설계 구조와 코드 품질 확인 목적**으로 작성되었습니다.

---

## 📂 파일 구조

```
Scripts/GameScene/
└── Character/
    ├── PlayerMovementController.cs   # 이동 총괄 (싱글톤)
    ├── PlayerCameraController.cs     # 카메라 Facade
    ├── PlayerInteractionController.cs# 레이캐스트 상호작용
    ├── InputManager.cs               # 입력 추상화 (싱글톤)
    ├── AnimatorController.cs         # 애니메이터 래퍼
    ├── CameraLookHandler.cs          # 마우스 시점 회전
    ├── HeadbobHandler.cs             # 이동 흔들림
    ├── CameraZoomHandler.cs          # FOV 줌 처리
    ├── PhotoCaptureHandler.cs        # 사진 촬영 처리
    └── MovementBaseState/
        ├── MovementBaseState.cs      # 이동 상태 추상 베이스
        ├── IdleState.cs
        ├── WalkState.cs
        ├── RunState.cs
        └── CrouchState.cs
```

---

## 🏗 아키텍처

```
InputManager
     ↓
PlayerMovementController ──→ State Machine (Idle/Walk/Run/Crouch)
     ↓                   ──→ PlayerInteractionController → Interactable
     ↓                   ──→ AnimatorController
     ↓
PlayerCameraController (Facade)
     ├── CameraLookHandler
     ├── HeadbobHandler
     ├── CameraZoomHandler  ──[OnZoomEnter/Exit 이벤트]──→ PhotoCaptureHandler
     └── PhotoCaptureHandler
```

의존이 **한 방향**으로만 흐릅니다. 역방향 참조 없음.

---

## ✅ 핵심 설계 포인트

### 1. State 패턴 — 이동 상태 머신

`MovementBaseState`를 추상 베이스로, 4개의 상태가 각자의 전환 조건을 담당합니다.  
새로운 이동 상태를 추가할 때 기존 코드를 건드리지 않아도 됩니다.

```csharp
// 각 State가 자신의 전환 조건을 직접 판단
public override void UpdateState(PlayerMovementController controller)
{
    if (!InputManager.Instance.IsMoving())
        controller.SwitchState(controller.Idle);
    else if (InputManager.Instance.IsRunning())
        controller.SwitchState(controller.Run);
}
```

### 2. Facade + POCO 핸들러 — 카메라 시스템

`PlayerCameraController`는 조율만 하고, 실제 로직은 4개의 순수 C# 클래스(POCO)에 위임합니다.  
`MonoBehaviour` 없이 분리되어 한 파일 = 한 책임 구조를 유지합니다.

```csharp
// PlayerCameraController.Init()
_look    = new CameraLookHandler(_cameraHolder, transform, CameraSetting);
_headbob = new HeadbobHandler(_characterCamera.transform, _cameraHolder, headbobSettings);
_zoom    = new CameraZoomHandler(this, _characterCamera, CameraSetting);
_photo   = new PhotoCaptureHandler(this);
```

### 3. 이벤트 기반 연결 — 줌 ↔ 사진 촬영

`CameraZoomHandler`와 `PhotoCaptureHandler`는 서로를 직접 참조하지 않습니다.  
이벤트만으로 연동되어, 한쪽을 교체해도 다른 쪽에 영향이 없습니다.

```csharp
_zoom.OnZoomEnter += _photo.EnterCaptureMode;
_zoom.OnZoomExit  += _photo.ExitCaptureMode;
```

---

## 🛠 기술 스택

- Unity 2022.3 LTS / URP
- C#
- FinalIK, DOTween, RetroVision Pro (VHS 후처리)
