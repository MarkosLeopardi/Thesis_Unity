using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSteering : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider FLWheel;
    public WheelCollider FRWheel;

    [Header("Hinge Joint")]
    public HingeJoint steeringHinge;

    [Header("Steering Settings")]
    public float maxSteering = 35f;
    public float wheelMaxAngle = 90f;
    
    [Header("Grabbing")]
    public Transform leftHand;
    public Transform rightHand;
    private bool leftGrabbing = false;
    private bool rightGrabbing = false;


    public void GrabLeftHand(Transform hand)
    {
        leftHand = hand;
        leftGrabbing = true;
    }

    public void GrabRightHand(Transform hand)
    {
        rightHand = hand;
        rightGrabbing = true;
    }

    public void ReleaseLeftHand()
    {
        leftHand = null;
        leftGrabbing = false;
    }

    public void ReleaseRightHand()
    {
        rightHand = null;
        rightGrabbing = false;
    }

    // Update is called once per frame
    void Update()
    {
        float wheelAngle = steeringHinge.angle;

        //Bind the rotation to the max allowed
        wheelAngle = Mathf.Clamp(wheelAngle, -wheelMaxAngle, wheelMaxAngle);

        //Map that rotation to actual steer angle
        float steerAngle = (wheelAngle / wheelMaxAngle) * maxSteering;

        //Apply to Wheels
        FLWheel.steerAngle = steerAngle;
        FRWheel.steerAngle = steerAngle;
    }
}
