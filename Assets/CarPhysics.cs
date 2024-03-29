using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPhysics : MonoBehaviour
{
    [SerializeField] private Transform car;
    [SerializeField] private Rigidbody carRigidBody;
    [SerializeField] private float carTopSpeed;
    [SerializeField] private AnimationCurve powerCurve;
    [SerializeField] private List<Transform> wheels;
    [SerializeField] private float suspensionRestDist;
    [SerializeField] private float springDamper;
    [SerializeField] private float springStrength;

    [Range(0, 100)] 
    [SerializeField] private int accelInput; //dummy
    
    void Start()
    {
        
    }
    
    void Update()
    {
        foreach (Transform wheel in wheels)
        {
            
            if (Physics.Raycast(new Ray(wheel.position, -wheel.up), out RaycastHit hit, 1))
            {
                // --- suspension spring force ---
                
                Vector3 springDir = wheel.up;
                Vector3 wheelVel = carRigidBody.GetPointVelocity(wheel.position);

                //offset from the raycast
                float offset = suspensionRestDist - hit.distance;

                float vel = Vector3.Dot(springDir, wheelVel);

                float force = (offset * springStrength) - (vel * springDamper);
                
                carRigidBody.AddForceAtPosition(springDir * force, wheel.position);
                
                // --- acceleration & braking ---

                Vector3 accelDir = wheel.forward;

                if (accelInput > 0.0f)
                {
                    float carSpeed = Vector3.Dot(car.forward, carRigidBody.velocity);
                
                    //normalize car speed
                    float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);
                
                    //available torque
                    float availableTorque = powerCurve.Evaluate(normalizedSpeed) * accelInput;
                    
                    carRigidBody.AddForceAtPosition(accelDir * availableTorque, wheel.position);
                }
                
            }
        }
        
        //
    }
}
