using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : Effector
{
    public void SetBool(string inParameterName, bool value)
    {
        _animator.SetBool(inParameterName, value);
    }

    public void SetFloat(string inParameterName, float value)
    {
    }

    public void PlayMove(Vector3 inDir)
    {
        _animator.SetFloat(AnimatorParameters.vInput, inDir.z);
        _animator.SetFloat(AnimatorParameters.hzInput, inDir.x);
    }
}

public class AnimatorParameters
{
    public const string hzInput = "hzInput";
    public const string vInput = "vInput";
    public const string Walking = "Walking";
    public const string Running = "Running";
    public const string Crouching = "Crouching";
}

public class CameraAnimations
{
    public const string CameraOpen = "OnlyCamOpen";
    public const string CameraClose = "OnlyCamClose";
}


