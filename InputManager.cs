using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public const KeyCode ZoomKey = KeyCode.Mouse1;
    public const KeyCode InteractionKey = KeyCode.E;
    public const KeyCode PhotoViewKey = KeyCode.Tab;
    public const KeyCode ShutterKey = KeyCode.Mouse0;

    public Vector2 GetMoveDirection()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
    
    public bool IsMoving()
    {
        Vector2 dir = GetMoveDirection();
        return dir.sqrMagnitude > 0.01f;
    }

    public bool IsRunning()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }

    public bool IsCrouching()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            return true;

        else if (Input.GetKeyUp(KeyCode.LeftControl))
            return false;

        else
            return false;
    }

    public Vector2 GetMouseDirection()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    public bool IsJumping()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public bool IsInteractDown()
    {
        return Input.GetKeyDown(InteractionKey);
    }

    public bool IsInteractUp()
    {
        return Input.GetKeyUp(InteractionKey);
    }

    public bool IsZoomDown()
    {
        return Input.GetKeyDown(ZoomKey);
    }

    public bool IsZoomUp()
    {
        return Input.GetKeyUp(ZoomKey);
    }

    public bool IsShutterDown()
    {
        return Input.GetKeyDown(ShutterKey);
    }
}
