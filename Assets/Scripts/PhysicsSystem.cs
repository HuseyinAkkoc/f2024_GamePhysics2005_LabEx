using UnityEngine;
using System.Collections.Generic;

public delegate void OnCollision(GameObject a, GameObject b);

public class PhysicsSystem
{
    public OnCollision collisionCallback; // Callback for collisions
    public Vector3 gravity = new Vector3(0, -9.81f, 0); // Gravity
    private PhysicsBody[] bodies;

    public void Step(float dt)
    {
        UpdateMotion(dt);
        List<HitPair> collisions = DetectCollisions();
        ResolveCollisions(collisions);
    }

    private void UpdateMotion(float dt)
    {
        foreach (PhysicsBody body in bodies)
        {
            if (!body.Dynamic()) continue;

            // Apply gravity
            body.vel += gravity * dt;

            // Apply drag
            body.vel *= Mathf.Pow(body.drag, dt);

            // Integrate position
            body.pos += body.vel * dt;

            // Update GameObject's position
            body.transform.position = body.pos;
        }
    }

    private List<HitPair> DetectCollisions()
    {
        List<HitPair> collisions = new List<HitPair>();

        for (int i = 0; i < bodies.Length; i++)
        {
            for (int j = i + 1; j < bodies.Length; j++)
            {
                PhysicsBody a = bodies[i];
                PhysicsBody b = bodies[j];

                Vector3 mtv = Vector3.zero;
                bool collision = false;

                if (a.shapeType == ShapeType.SPHERE && b.shapeType == ShapeType.SPHERE)
                    collision = SphereSphere(a, b, out mtv);
                else if (a.shapeType == ShapeType.SPHERE && b.shapeType == ShapeType.PLANE)
                    collision = SpherePlane(a, b, out mtv);
                else if (a.shapeType == ShapeType.PLANE && b.shapeType == ShapeType.SPHERE)
                    collision = SpherePlane(b, a, out mtv);
                else if (a.shapeType == ShapeType.AABB && b.shapeType == ShapeType.PLANE)
                    collision = AABBPlane(a, b, out mtv);
                else if (a.shapeType == ShapeType.PLANE && b.shapeType == ShapeType.AABB)
                    collision = AABBPlane(b, a, out mtv);
                else if (a.shapeType == ShapeType.AABB && b.shapeType == ShapeType.AABB)
                    collision = AABBAABB(a, b, out mtv);
                else if (a.shapeType == ShapeType.SPHERE && b.shapeType == ShapeType.AABB)
                    collision = SphereAABB(a, b, out mtv);
                else if (a.shapeType == ShapeType.AABB && b.shapeType == ShapeType.SPHERE)
                    collision = SphereAABB(b, a, out mtv);

                if (collision)
                    collisions.Add(new HitPair(a, b, mtv));
            }
        }
        return collisions;
    }

    private bool SphereSphere(PhysicsBody a, PhysicsBody b, out Vector3 mtv)
    {
        Vector3 delta = b.pos - a.pos;
        float distance = delta.magnitude;
        float penetration = a.radius + b.radius - distance;

        if (penetration > 0)
        {
            mtv = delta.normalized * penetration;
            return true;
        }

        mtv = Vector3.zero;
        return false;
    }

    private bool SpherePlane(PhysicsBody sphere, PhysicsBody plane, out Vector3 mtv)
    {
        float distance = Vector3.Dot(sphere.pos - plane.pos, plane.normal);
        if (distance < sphere.radius)
        {
            mtv = plane.normal * (sphere.radius - distance);
            return true;
        }

        mtv = Vector3.zero;
        return false;
    }

    private bool SphereAABB(PhysicsBody sphere, PhysicsBody aabb, out Vector3 mtv)
    {
        // Find the closest point on the AABB to the sphere's center
        Vector3 closestPoint = new Vector3(
            Mathf.Clamp(sphere.pos.x, aabb.Min.x, aabb.Max.x),
            Mathf.Clamp(sphere.pos.y, aabb.Min.y, aabb.Max.y),
            Mathf.Clamp(sphere.pos.z, aabb.Min.z, aabb.Max.z)
        );

        Vector3 delta = sphere.pos - closestPoint;
        float distanceSquared = delta.sqrMagnitude;
        float radiusSquared = sphere.radius * sphere.radius;

        if (distanceSquared < radiusSquared)
        {
            float distance = Mathf.Sqrt(distanceSquared);
            Vector3 direction = distance > 0 ? delta / distance : Vector3.up;
            mtv = direction * (sphere.radius - distance);
            return true;
        }

        mtv = Vector3.zero;
        return false;
    }

    private bool AABBAABB(PhysicsBody a, PhysicsBody b, out Vector3 mtv)
    {
        bool overlapX = a.Max.x > b.Min.x && a.Min.x < b.Max.x;
        bool overlapY = a.Max.y > b.Min.y && a.Min.y < b.Max.y;
        bool overlapZ = a.Max.z > b.Min.z && a.Min.z < b.Max.z;

        if (overlapX && overlapY && overlapZ)
        {
            float overlapXVal = Mathf.Min(a.Max.x - b.Min.x, b.Max.x - a.Min.x);
            float overlapYVal = Mathf.Min(a.Max.y - b.Min.y, b.Max.y - a.Min.y);
            float overlapZVal = Mathf.Min(a.Max.z - b.Min.z, b.Max.z - a.Min.z);

            if (overlapXVal < overlapYVal && overlapXVal < overlapZVal)
                mtv = new Vector3(overlapXVal, 0, 0);
            else if (overlapYVal < overlapZVal)
                mtv = new Vector3(0, overlapYVal, 0);
            else
                mtv = new Vector3(0, 0, overlapZVal);

            return true;
        }

        mtv = Vector3.zero;
        return false;
    }

    private bool AABBPlane(PhysicsBody aabb, PhysicsBody plane, out Vector3 mtv)
    {
        float distance = Vector3.Dot(aabb.pos - plane.pos, plane.normal);
        float halfSize = Vector3.Dot(aabb.size / 2, plane.normal);

        if (distance < halfSize)
        {
            mtv = plane.normal * (halfSize - distance);
            return true;
        }

        mtv = Vector3.zero;
        return false;
    }

    private void ResolveCollisions(List<HitPair> collisions)
    {
        foreach (var hit in collisions)
        {
            PhysicsBody a = hit.bodyA;
            PhysicsBody b = hit.bodyB;

            // Skip resolution if both objects are static
            if (!a.Dynamic() && !b.Dynamic()) continue;

            Vector3 mtv = hit.mtv;

            // Apply the MTV to resolve position
            if (a.Dynamic()) a.pos += mtv * 0.5f;
            if (b.Dynamic()) b.pos -= mtv * 0.5f;

            // Apply basic velocity reflection based on MTV direction
            Vector3 relativeVelocity = a.vel - b.vel;
            Vector3 mtvDirection = mtv.normalized;

            float restitution = 0.3f; // Coefficient of restitution for bounciness
            float impulseMagnitude = -(1 + restitution) * Vector3.Dot(relativeVelocity, mtvDirection);

            if (impulseMagnitude > 0)
            {
                Vector3 impulse = mtvDirection * impulseMagnitude;

                if (a.Dynamic()) a.vel += impulse * a.invMass;
                if (b.Dynamic()) b.vel -= impulse * b.invMass;
            }

            // Invoke collision callback
            collisionCallback?.Invoke(a.gameObject, b.gameObject);
        }
    }


    public void PreStep()
    {
        // Use the new method to find all PhysicsBody components
        bodies = GameObject.FindObjectsByType<PhysicsBody>(FindObjectsSortMode.None);

        foreach (var body in bodies)
        {
            body.pos = body.transform.position;
        }
    }


    public void PostStep()
    {
        foreach (var body in bodies)
        {
            body.transform.position = body.pos;
        }
    }

    public void Clear()
    {
        if (bodies == null) return;

        foreach (PhysicsBody body in bodies)
        {
            if (body == null || body.shapeType == ShapeType.PLANE || !body.Dynamic())
                continue;

            GameObject.Destroy(body.gameObject);
        }

        bodies = GameObject.FindObjectsByType<PhysicsBody>(FindObjectsSortMode.None);

    }
}

public class HitPair
{
    public PhysicsBody bodyA;
    public PhysicsBody bodyB;
    public Vector3 mtv;

    public HitPair(PhysicsBody a, PhysicsBody b, Vector3 mtv)
    {
        bodyA = a;
        bodyB = b;
        this.mtv = mtv;
    }
}
