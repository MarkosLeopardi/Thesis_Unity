using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class VRCarInput : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider FLWheel;
    public WheelCollider FRWheel;
    public WheelCollider RLWheel;
    public WheelCollider RRWheel;

    [Header("Physics Settings")]
    public float torque = 3000f;
    public float brakeforce = 5000f;
    public float handbrakeforce = 10000f;

    public Rigidbody carBody;

    private bool accelerating = false;
    private bool braking = false;
    private bool handbraking = false;

    private void Update()
    {
        float currentSpeed = carBody.velocity.magnitude;

        // ACCELERATION
        if (accelerating && !handbraking && !braking)
        {
            ApplyMotorTorque(torque);
        }
        else
        {
            ApplyMotorTorque(0f);
        }

        // BRAKING & REVERSE
        if (braking)
        {
            if (currentSpeed < 0.1f)
            {
                ApplyBrakes(0f); // No brake torque when reversing
                ApplyMotorTorque(-brakeforce); // Reverse force
            }
            else
            {
                ApplyMotorTorque(0f);
                ApplyBrakes(brakeforce); // Braking force
            }
        }
        else
        {
            ApplyBrakes(0f);
        }

        // HANDBRAKE
        ApplyHandbrake(handbraking ? handbrakeforce : 0f);
    }

    // ???????????????????????????????????????????????
    // UNITY EVENTS - Hook these in PlayerInput Events
    // ???????????????????????????????????????????????

    public void OnAccelerate(InputAction.CallbackContext context)
    {
        accelerating = context.performed;
        if (context.canceled)
            accelerating = false;
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        braking = context.performed;
        if (context.canceled)
            braking = false;
    }

    public void OnHandbrake(InputAction.CallbackContext context)
    {
        handbraking = context.performed;
        if (context.canceled)
            handbraking = false;
    }

    public void OnRestart(InputAction.CallbackContext context)
    {
        if (context.performed)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ???????????????????????????????????????????????
    // DRIVE METHODS
    // ???????????????????????????????????????????????

    private void ApplyMotorTorque(float force)
    {
        RLWheel.motorTorque = force;
        RRWheel.motorTorque = force;
    }

    private void ApplyBrakes(float force)
    {
        RLWheel.brakeTorque = force;
        RRWheel.brakeTorque = force;
    }

    private void ApplyHandbrake(float force)
    {
        FLWheel.brakeTorque = force;
        FRWheel.brakeTorque = force;
        RLWheel.brakeTorque = force;
        RRWheel.brakeTorque = force;
    }
}
