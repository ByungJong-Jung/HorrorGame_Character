using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementBaseState
{
    public abstract void EnterState(PlayerMovementController inController);

    public abstract void UpdateState(PlayerMovementController inController);

    public abstract void ExitState(PlayerMovementController inController);
}
