using UnityEngine;

// *Unity Physics are not allowed. We must make EVERYTHING from scratch in this course!*
public class PhysicsBody
{

    public Vector3 vel = Vector3.zero;
    public Vector3 pos = Vector3.zero;
    // Drag that's near-zero makes velocity smaller (ie 2 * 0.05 = 0.1
    // Drag that's near-one makes velocity larger (ie 2 * 0.95 = 1.9)
    // Drag must be between 0 and 1. Drag of 1 means no air resistance!
    public float drag = 1.0f;
}