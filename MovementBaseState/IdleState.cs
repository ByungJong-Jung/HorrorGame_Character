using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : MovementBaseState
{
    public override void EnterState(PlayerMovementController inController)
    {
    }

    public override void UpdateState(PlayerMovementController inController)
    {
        if(InputManager.Instance.IsMoving())
        {
            if(InputManager.Instance.IsRunning())
                inController.SwitchState(inController.Run);
            else
                inController.SwitchState(inController.Walk);

        }
        else if (InputManager.Instance.IsCrouching() && inController.IsGrounded())
        {
            inController.SwitchState(inController.Crouch);
        }

        if(InputManager.Instance.IsJumping() && inController.IsGrounded())
        {
            inController.HandleJump();
        } 
    }

    public override void ExitState(PlayerMovementController inController)
    {
    }

}
