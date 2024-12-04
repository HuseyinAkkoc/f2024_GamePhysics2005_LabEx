using UnityEngine;

public enum ShapeType
{
    SPHERE,
    PLANE
}

public class PhysicsBody : MonoBehaviour
{
    public Vector3 pos = Vector3.zero;         // Position of the object
    public Vector3 vel = Vector3.zero;         // Velocity of the object
    public float drag = 1.0f;                  // Air resistance factor
    public float invMass = 1.0f;               // Inverse mass (0 for static objects)
    public ShapeType shapeType = ShapeType.SPHERE; // Shape type (SPHERE or PLANE)
    public float radius = 1.0f;                // Radius (for SPHERE shape type)
    public Vector3 normal = Vector3.up;        // Normal vector (for PLANE shape type)
    public bool collision = false;             // Whether the object has collided
    public float frictionCoefficient = 1.0f;  // Friction coefficient (static & kinetic combined)
    public float restitutionCoefficient = 1.0f; // Restitution coefficient (bounciness)

    // Check if the object is dynamic (can move)
    public bool Dynamic()
    {
        return invMass > 0.0f; // Object is dynamic if inverse mass is non-zero
    }
}
