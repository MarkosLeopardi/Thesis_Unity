using System;
using Unity.VRTemplate;
using UnityEngine;
using UnityEngine.InputSystem; // Import the Input System namespace
using UnityEngine.XR.Interaction.Toolkit; // Import the XR Interaction Toolkit namespace
using UnityEngine.XR.Interaction;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Range(20, 190)]
    public int maxSpeed = 90; //The maximum speed that the car can reach in km/h.
    [Range(10, 120)]
    public int maxReverseSpeed = 45; //The maximum speed that the car can reach while going on reverse in km/h.
    [Range(1, 10)]
    public int accelerationMultiplier = 2;
    [Range(100, 600)]
    public int brakeForce = 350;
    [HideInInspector]
    public float carSpeed;
    public  GameObject FLWheel;
    public GameObject FRWheel;
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;
    public InputActionReference accelerate;
    public InputActionReference brake;
    public InputActionReference handbrake;
    public float handbrakeTorque;
    float localVelocityZ;
    Rigidbody carRigidbody;
    public Vector3 bodyMassCenter;
    float throttleAxis;
    bool decelerateCar;
    public XRKnob knob;
    void Start()
    {
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;
    }

    // Update is called once per frame
    void Update()
    {
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.linearVelocity).z;

        if (accelerate.action.IsPressed())
        {
            CancelInvoke("DecelerateCar");
            decelerateCar = false;
            Accelerate();
        }
        if (brake.action.IsPressed())
        {
            CancelInvoke("DecelerateCar");
            decelerateCar = false;
            Reverse();
        }
        if (handbrake.action.IsPressed())
        {
            CancelInvoke("DecelerateCar");
            decelerateCar = false;
            Handbrake();
        }
        if ((!accelerate.action.IsPressed()) && (!brake.action.IsPressed()))
        {
            ThrottleOff();
        }
        if ((!brake.action.IsPressed()) && (!accelerate.action.IsPressed()) && (!handbrake.action.IsPressed()) && !decelerateCar)
        {
            InvokeRepeating("DecelerateCar", 0f, 0.1f);
            decelerateCar = true;
        }


        frontLeftCollider.steerAngle = (knob.value - 0.5f) * 120f; // Adjust the steering angle based on the knob value
        frontRightCollider.steerAngle = (knob.value - 0.5f) * 120f; // Adjust the steering angle based on the knob value
    }
    public void Accelerate()
    {
        throttleAxis = throttleAxis + (Time.deltaTime * 3f);
        if (throttleAxis > 1f)
        {
            throttleAxis = 1f;
        }
        if (localVelocityZ < -1f)
        {
            Brakes();
        }
        else
        {
            if (Mathf.RoundToInt(carSpeed) < maxSpeed)
            {
                //Apply positive torque in all wheels to go forward if maxSpeed has not been reached.
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                // If the maxSpeed has been reached, then stop applying torque to the wheels.
                // IMPORTANT: The maxSpeed variable should be considered as an approximation; the speed of the car
                // could be a bit higher than expected.
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }
    public void Reverse()
    {
        throttleAxis = throttleAxis - (Time.deltaTime * 3f);
        if (throttleAxis < -1f)
        {
            throttleAxis = -1f;
        }
        if (localVelocityZ > 1f)
        {
            Brakes();
        }
        else
        {
            if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
            {
                //Apply negative torque in all wheels to go in reverse if maxReverseSpeed has not been reached.
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                //If the maxReverseSpeed has been reached, then stop applying torque to the wheels.
                // IMPORTANT: The maxReverseSpeed variable should be considered as an approximation; the speed of the car
                // could be a bit higher than expected.
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }
    public void Handbrake()
    {
        frontLeftCollider.brakeTorque = handbrakeTorque;
        frontRightCollider.brakeTorque = handbrakeTorque;
        rearLeftCollider.brakeTorque = handbrakeTorque;
        rearRightCollider.brakeTorque = handbrakeTorque;
    }
    public void ThrottleOff()
    {
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
    }
    public void Brakes()
    {
        frontLeftCollider.brakeTorque = brakeForce;
        frontRightCollider.brakeTorque = brakeForce;
        rearLeftCollider.brakeTorque = brakeForce;
        rearRightCollider.brakeTorque = brakeForce;
    }
    public void DecelerateCar()
    {
        // The following part resets the throttle power to 0 smoothly.
        if (throttleAxis != 0f)
        {
            if (throttleAxis > 0f)
            {
                throttleAxis = throttleAxis - (Time.deltaTime * 10f);
            }
            else if (throttleAxis < 0f)
            {
                throttleAxis = throttleAxis + (Time.deltaTime * 10f);
            }
            if (Mathf.Abs(throttleAxis) < 0.15f)
            {
                throttleAxis = 0f;
            }
        }
    }
}