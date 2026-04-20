using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("[ Interaction ]")]
    [SerializeField]
    private float _interactionDistance;
    private Vector3 _interactionRayPoint;
    private Interactable _currentInteractable;
    LayerMask interactionMask = 1 << EInteractableLayers.INTERACTION;
    public void HandleInteraction(Ray inCameraRay)
    {
        CheckInteraction(inCameraRay);
        ClickInteraction(inCameraRay);
    }

    private void ClickInteraction(Ray inRay)
    {
        if (InputManager.Instance.IsInteractDown()
            && _currentInteractable != null
            && Physics.Raycast(inRay, out RaycastHit hit, _interactionDistance))
        {
            _currentInteractable.OnInteract();
        }
        else if (InputManager.Instance.IsInteractUp())
        {
            MainUI.Instance.CursorUI.CancelHoldInteraction();
        }
    }

    private void CheckInteraction(Ray inRay)
    {
#if UNITY_EDITOR
        Debug.DrawRay(inRay.origin, inRay.direction * _interactionDistance, Color.cyan);
#endif

        if (Physics.Raycast(inRay, out RaycastHit hit, _interactionDistance, interactionMask))
        {
            GameObject hitObject = hit.collider.gameObject;


            var interactable = hitObject.GetComponentInParent<Interactable>();
            if (interactable != null)
            {
                GameObject rootObj = (interactable as MonoBehaviour)?.gameObject;
                if (rootObj != null &&
                    rootObj.layer == EInteractableLayers.INTERACTION &&
                    (_currentInteractable == null || interactable != _currentInteractable))
                {
                    _currentInteractable = interactable;
                    _currentInteractable.OnFocus(MainUI.Instance.CursorUI);
                    return;
                }
            }
        }
        else if (_currentInteractable != null)
        {
            _currentInteractable.OnLoseFocus();
            _currentInteractable = null;
        }
    }

    public void ProcessLoseFocus()
    {
        _currentInteractable?.OnLoseFocus();
        _currentInteractable = null;
    }
}
