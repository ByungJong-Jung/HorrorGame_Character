using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : MovementBaseState
{
    public override void EnterState(PlayerMovementController inController)
    {
        inController.AnimatorController.SetBool(AnimatorParameters.Walking, true);
    }

    public override void UpdateState(PlayerMovementController inController)
    {
        float speed = inController.MovementSetting.GetSpeed(PlayerMovementController.MovementSettings.MovementType.Walk);
        inController.Move(InputManager.Instance.GetMoveDirection(), speed, PlayerCameraController.HeadbobSettings.MovementType.Walk);

        if(InputManager.Instance.IsMoving() == false)
        {
            inController.SwitchState(inController.Idle);
        }
        else if(InputManager.Instance.IsRunning())
        {
            inController.SwitchState(inController.Run);
        }
        else if (InputManager.Instance.IsCrouching() && inController.IsGrounded())
        {
            inController.SwitchState(inController.Crouch);
        }

        if (InputManager.Instance.IsJumping() && inController.IsGrounded())
        {
            inController.HandleJump();
        }
    }


    public override void ExitState(PlayerMovementController inController)
    {
        inController.AnimatorController.SetBool(AnimatorParameters.Walking, false);
    }
}
