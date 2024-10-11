using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhysicsSystem
{
    
    public Vector3 gravity = Physics.gravity;
    public List<PhysicsBody> bodies = new List<PhysicsBody>();

    public void Step(float dt)
    {
        
        Vector3 acc = gravity;
        for (int i = 0; i < bodies.Count; i++)
        {
            // Current physics object ("body")
            PhysicsBody body = bodies[i];
            // Apply drag to velocity
            body.vel *= Mathf.Pow(body.drag, dt);

          
            
            // Apply motion
            Integrate(ref body.vel, acc, dt);
            Integrate(ref body.pos, body.vel, dt);
        }
    }

    // "Integration" is updating a value based on the previous value + its change
    private void Integrate(ref Vector3 value, Vector3 change, float dt)
    {
        value = value + change * dt;
    }
}