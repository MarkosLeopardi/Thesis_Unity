using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System.Collections.Generic;

// This script allows both hands to grab an object (like a steering wheel) using separate grab buttons.
// It supports snapping and maintaining multiple grab points simultaneously.

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Grabbable : MonoBehaviour
{
    [Tooltip("Attach point for the left hand grab")]
    public Transform leftHandAttachPoint;

    [Tooltip("Attach point for the right hand grab")]
    public Transform rightHandAttachPoint;

    [Tooltip("Input action for left hand grab")]
    public InputActionProperty leftGrabAction;

    [Tooltip("Input action for right hand grab")]
    public InputActionProperty rightGrabAction;

    private List<XRBaseInteractor> activeInteractors = new List<XRBaseInteractor>();
    private List<XRBaseInteractor> nearbyInteractors = new List<XRBaseInteractor>();
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Freeze position, only allow rotation (e.g. steering wheel behavior)
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        foreach (var interactor in nearbyInteractors)
        {
            bool isLeftHand = interactor.name.ToLower().Contains("left");
            bool isRightHand = interactor.name.ToLower().Contains("right");

            if (!activeInteractors.Contains(interactor))
            {
                if (isLeftHand && leftGrabAction.action.WasPerformedThisFrame())
                    Grab(interactor);
                if (isRightHand && rightGrabAction.action.WasPerformedThisFrame())
                    Grab(interactor);
            }
        }

        for (int i = activeInteractors.Count - 1; i >= 0; i--)
        {
            XRBaseInteractor interactor = activeInteractors[i];
            bool isLeftHand = interactor.name.ToLower().Contains("left");
            bool isRightHand = interactor.name.ToLower().Contains("right");

            if ((isLeftHand && leftGrabAction.action.WasReleasedThisFrame()) ||
                (isRightHand && rightGrabAction.action.WasReleasedThisFrame()))
            {
                Release(interactor);
            }
        }
    }

    public void Grab(XRBaseInteractor interactor)
    {
        if (!activeInteractors.Contains(interactor))
        {
            activeInteractors.Add(interactor);

            Transform chosenAttachPoint = GetAttachPoint(interactor);
            if (chosenAttachPoint == null) return;

            GameObject anchor = new GameObject("GrabAnchor");
            anchor.transform.position = chosenAttachPoint.position;
            anchor.transform.rotation = chosenAttachPoint.rotation;
            anchor.transform.SetParent(interactor.attachTransform);

            transform.SetParent(anchor.transform, true);
        }
    }

    public void Release(XRBaseInteractor interactor)
    {
        if (activeInteractors.Contains(interactor))
        {
            activeInteractors.Remove(interactor);
            transform.SetParent(null);
        }
    }

    private Transform GetAttachPoint(XRBaseInteractor interactor)
    {
        string name = interactor.name.ToLower();
        if (name.Contains("left")) return leftHandAttachPoint;
        if (name.Contains("right")) return rightHandAttachPoint;
        return rightHandAttachPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        var interactor = other.GetComponent<XRBaseInteractor>();
        if (interactor != null && !nearbyInteractors.Contains(interactor))
        {
            nearbyInteractors.Add(interactor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var interactor = other.GetComponent<XRBaseInteractor>();
        if (interactor != null)
        {
            nearbyInteractors.Remove(interactor);
            Release(interactor);
        }
    }
}
