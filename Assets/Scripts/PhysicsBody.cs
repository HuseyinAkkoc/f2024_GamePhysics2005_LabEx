using UnityEngine;

public enum ShapeType
{
    SPHERE,
    PLANE,
    AABB
}

public class PhysicsBody : MonoBehaviour
{
    public Vector3 pos = Vector3.zero; // Position
    public Vector3 vel = Vector3.zero; // Velocity

    public float drag = 0.99f;         // Drag for motion
    public float invMass = 1.0f;       // 1.0 for dynamic, 0.0 for static

    public ShapeType shapeType = ShapeType.SPHERE;

    // Sphere properties
    [Header("Sphere Settings")]
    public float radius = 0.5f;

    // Plane properties
    [Header("Plane Settings")]
    public Vector3 normal = Vector3.up;

    // AABB properties
    [Header("AABB Settings")]
    public Vector3 size = Vector3.one;

    // Calculated bounds for AABB
    public Vector3 Min => pos - size / 2;
    public Vector3 Max => pos + size / 2;

    void Start()
    {
        // Initialize position based on GameObject transform
        pos = transform.position;

        // Auto-adjust settings based on ShapeType
        ValidateShapeType();
    }

    void Update()
    {
        // Keep the GameObject position synchronized with pos
        transform.position = pos;
    }

    // Ensures only relevant properties are applied based on shape type
    private void ValidateShapeType()
    {
        switch (shapeType)
        {
            case ShapeType.SPHERE:
                if (radius <= 0) radius = 0.5f; // Default radius for spheres
                break;

            case ShapeType.AABB:
                if (size == Vector3.zero) size = Vector3.one; // Default size for AABBs
                break;

            case ShapeType.PLANE:
                normal = normal.normalized; // Ensure the normal is normalized
                break;
        }
    }

    // Check if this body is dynamic
    public bool Dynamic()
    {
        return invMass > 0.0f;
    }

    // Method to update AABB size dynamically
    public void SetBounds(Vector3 newSize)
    {
        if (shapeType != ShapeType.AABB)
        {
            Debug.LogWarning("Cannot set bounds for a non-AABB shape.");
            return;
        }

        size = newSize;
    }

    // Debug drawing for different shapes
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (shapeType == ShapeType.SPHERE)
        {
            Gizmos.DrawWireSphere(pos, radius);
        }
        else if (shapeType == ShapeType.PLANE)
        {
            Gizmos.DrawLine(pos - normal * 10, pos + normal * 10);
        }
        else if (shapeType == ShapeType.AABB)
        {
            Gizmos.DrawWireCube(pos, size);
        }
    }
}
