using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarPhysics : MonoBehaviour
{
    [SerializeField] private Transform car;
    [SerializeField] private Rigidbody carRigidBody;
    [SerializeField] private float carTopSpeed;
    [SerializeField] private AnimationCurve powerCurve;
    [SerializeField] private List<WheelData> wheels;
    [SerializeField] private float tireGripFactor;
    [SerializeField] private float tireMass;
    [SerializeField] private float suspensionRestDist;
    [SerializeField] private float springDamper;
    [SerializeField] private float springStrength;
    [SerializeField] private float accelInput;
    [SerializeField] private int accelMultiplier;
    [SerializeField] private float maxSteering;
    
    [System.Serializable]
    class  WheelData
    {
        public Transform transform;
        public bool turnable;
    }
    
    void Start()
    {
        
    }

    void OnTurn(InputValue input)
    {
        foreach (WheelData wheel in wheels)
        {
            if (wheel.turnable == false)
            {
                continue;
            }
            
            Vector3 rotation = wheel.transform.rotation.eulerAngles;

            rotation.y = input.Get<float>() * maxSteering;

            wheel.transform.rotation = Quaternion.Euler(rotation);
        }
    }

    void OnAccelerate(InputValue input)
    {
        accelInput = input.Get<float>();
    }
    
    void Update()
    {
        // --- the car physics stuff ---
        foreach (WheelData wheel in wheels)
        {
            Transform wheelTransform = wheel.transform;
            
            if (Physics.Raycast(new Ray(wheelTransform.position, -wheelTransform.up), out RaycastHit hit, 1))
            {
                // --- suspension spring force ---
                
                Vector3 springDir = wheelTransform.up;
                Vector3 wheelVel = carRigidBody.GetPointVelocity(wheelTransform.position);

                //offset from the raycast
                float offset = suspensionRestDist - hit.distance;

                float vel = Vector3.Dot(springDir, wheelVel);

                float force = (offset * springStrength) - (vel * springDamper);
                
                carRigidBody.AddForceAtPosition(springDir * force, wheelTransform.position);
                
                // --- acceleration & braking ---

                Vector3 accelDir = wheelTransform.forward;
                
                float carSpeed = Vector3.Dot(car.forward, carRigidBody.velocity);
        
                //normalize car speed
                float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);
        
                //available torque
                float availableTorque = powerCurve.Evaluate(normalizedSpeed) * (accelInput * accelMultiplier);
            
                carRigidBody.AddForceAtPosition(accelDir * availableTorque, wheelTransform.position);
                
                // --- steering ---

                Vector3 steeringDir = wheelTransform.right;

                float steeringVel = Vector3.Dot(steeringDir, wheelVel);

                float desiredVelChange = -steeringVel * tireGripFactor;

                float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
                
                carRigidBody.AddForceAtPosition(steeringDir * tireMass * desiredAccel, wheelTransform.position);
            }
        }

        
    }
}
