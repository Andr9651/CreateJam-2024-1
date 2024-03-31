using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarPhysics : MonoBehaviour
{
    [SerializeField] private Transform car;
    [SerializeField] private Rigidbody carRigidBody;
    [SerializeField] private List<WheelData> wheels;

    [Header("Speed")]
    [SerializeField] private float carTopSpeed;
    [SerializeField] private AnimationCurve powerCurve;
    [SerializeField] private int accelMultiplier;
    [SerializeField] private float accelInput;
    [SerializeField] private float maxRollingFriction;
    [Header("Steering")]
    [SerializeField] private float maxSteering;
    [SerializeField] private float tireGripFactor;
    [SerializeField] private float tireMass;

    [Header("Suspension")]
    [SerializeField] private float suspensionRestDist;
    [SerializeField] private float springDamper;
    [SerializeField] private float springStrength;

    [Header("Other")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float smokeThreshhold;


    [System.Serializable]
    class WheelData
    {
        public Transform transform;
        public bool turnable;
        public bool isMoterized = true;
        public ParticleSystem smoke;
    }

    void OnTurn(InputValue input)
    {
        foreach (WheelData wheel in wheels)
        {
            if (wheel.turnable == false)
            {
                continue;
            }

            float rotation = input.Get<float>() * maxSteering;

            wheel.transform.localRotation = Quaternion.Euler(0, rotation, 0);
        }
    }

    void OnAccelerate(InputValue input)
    {
        accelInput = input.Get<float>();
    }

    void OnJump(InputValue input)
    {
        if (Physics.Raycast(new Ray(transform.position, -transform.up), suspensionRestDist))
        {
            carRigidBody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
        }
    }

    void OnFlip()
    {
        if (Physics.Raycast(new Ray(transform.position, transform.up), 2))
        {
            Vector3 rotation = carRigidBody.rotation.eulerAngles;
            rotation.z = 0;
            rotation.y = 0;

            carRigidBody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);

            carRigidBody.rotation = Quaternion.Euler(rotation);
        }

    }

    void OnReset()
    {
        FindFirstObjectByType<GameManager>().ResetScore();
    }

    void Start()
    {
        carRigidBody.centerOfMass = new Vector3(0, 0, 0);
    }

    void FixedUpdate()
    {
        // --- the car physics stuff ---
        foreach (WheelData wheel in wheels)
        {
            if (Physics.Raycast(new Ray(wheel.transform.position, -wheel.transform.up), out RaycastHit hit, suspensionRestDist))
            {
                ApplySuspension(wheel, hit);

                if (wheel.isMoterized)
                {
                    ApplyAcceleration(wheel);
                }

                ApplyRollingFriction(wheel);

                ApplySteering(wheel, hit);
                DisplayWheels(wheel, hit);
            }
            DriftSmoke(wheel, hit);
            DisplayWheels(wheel, hit);


        }
    }

    private void ApplySuspension(WheelData wheel, RaycastHit hit)
    {
        Vector3 springDir = wheel.transform.up;
        Vector3 wheelVel = carRigidBody.GetPointVelocity(wheel.transform.position);

        //offset from the raycast
        float offset = suspensionRestDist - hit.distance;

        float vel = Vector3.Dot(springDir, wheelVel);

        float force = (offset * springStrength) - (vel * springDamper);

        carRigidBody.AddForceAtPosition(springDir * force, wheel.transform.position);
    }

    private void ApplyRollingFriction(WheelData wheel)
    {
        if (Mathf.Abs(accelInput) < 0.05)
        {

            Vector3 wheelVel = carRigidBody.GetPointVelocity(wheel.transform.position);
            Vector3 steeringDir = wheel.transform.forward;
            Vector3 frictionDir = steeringDir * Mathf.Sign(Vector3.Dot(steeringDir, wheelVel));

            carRigidBody.AddForceAtPosition(frictionDir * -maxRollingFriction, wheel.transform.position);
        }
    }

    private void ApplySteering(WheelData wheel, RaycastHit hit)
    {
        Vector3 wheelVel = carRigidBody.GetPointVelocity(wheel.transform.position);
        Vector3 steeringDir = wheel.transform.right;

        float steeringVel = Vector3.Dot(steeringDir, wheelVel);

        float desiredVelChange = -steeringVel * tireGripFactor;

        float desiredAccel = desiredVelChange / Time.fixedDeltaTime;

        carRigidBody.AddForceAtPosition(steeringDir * tireMass * desiredAccel, wheel.transform.position);
    }

    private void DisplayWheels(WheelData wheel, RaycastHit hit)
    {

        float distance = hit.distance != 0 ? hit.distance : suspensionRestDist;
        distance -= 0.5f;

        wheel.transform.GetChild(0).transform.localPosition = new Vector3(0, -distance, 0);
    }

    private void DriftSmoke(WheelData wheel, RaycastHit hit)
    {
        if (wheel.smoke == null)
        {
            return;
        }
        if (hit.distance == 0)
        {
            if (wheel.smoke.isPlaying == true)
            {
                wheel.smoke.Stop();
            }
        }

        Vector3 wheelVel = carRigidBody.GetPointVelocity(wheel.transform.position);
        Vector3 steeringDir = wheel.transform.right;

        float steeringVel = Vector3.Dot(steeringDir, wheelVel);

        if (Mathf.Abs(steeringVel) > smokeThreshhold)
        {
            if (wheel.smoke.isPlaying == false)
            {
                wheel.smoke.Play();
            }
        }
        else
        {
            if (wheel.smoke.isPlaying == true)
            {
                wheel.smoke.Stop();
            }
        }

    }

    private void ApplyAcceleration(WheelData wheel)
    {
        Vector3 accelDir = wheel.transform.forward;

        float carSpeed = Vector3.Dot(car.forward, carRigidBody.velocity);

        //normalize car speed
        float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);

        //available torque
        float availableTorque = powerCurve.Evaluate(normalizedSpeed) * (accelInput * accelMultiplier);

        carRigidBody.AddForceAtPosition(accelDir * availableTorque, wheel.transform.position);
    }
}
