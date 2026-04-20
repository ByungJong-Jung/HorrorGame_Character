using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : MovementBaseState
{
    private bool _isTryingToStand;
    public override void EnterState(PlayerMovementController inController)
    {
        inController.AnimatorController.SetBool(AnimatorParameters.Crouching, true);
    }

    public override void UpdateState(PlayerMovementController inController)
    {
        if(InputManager.Instance.IsMoving())
        {
            float speed = inController.MovementSetting.GetSpeed(PlayerMovementController.MovementSettings.MovementType.Crouch);
            inController.Move(InputManager.Instance.GetMoveDirection(), speed, PlayerCameraController.HeadbobSettings.MovementType.Crouch);
        }

        if(inController.IsCrouching == false)
        {
            if (InputManager.Instance.IsMoving())
                inController.SwitchState(inController.Walk);
            else
                inController.SwitchState(inController.Idle);
        }
    }


    public override void ExitState(PlayerMovementController inController)
    {
        inController.AnimatorController.SetBool(AnimatorParameters.Crouching, false);
    }
}
