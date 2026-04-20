using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : MovementBaseState
{
    public override void EnterState(PlayerMovementController inController)
    {
        inController.AnimatorController.SetBool(AnimatorParameters.Running, true);
    }

    public override void UpdateState(PlayerMovementController inController)
    {
        float speed = inController.MovementSetting.GetSpeed(PlayerMovementController.MovementSettings.MovementType.Run);
        inController.Move(InputManager.Instance.GetMoveDirection(), speed, PlayerCameraController.HeadbobSettings.MovementType.Run);

        if (!InputManager.Instance.IsRunning())
        {
            inController.SwitchState(inController.Walk);
        }

        if (InputManager.Instance.IsJumping() && inController.IsGrounded())
        {
            inController.HandleJump();
        }
    }


    public override void ExitState(PlayerMovementController inController)
    {
        inController.AnimatorController.SetBool(AnimatorParameters.Running, false);
    }
}
