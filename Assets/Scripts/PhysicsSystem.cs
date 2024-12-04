using System.Collections.Generic;
using UnityEngine;



public class HitPair
{
    public int a = -1;
    public int b = -1;
    public Vector3 mtv = Vector3.zero;
}

public class PhysicsSystem
{
    public Vector3 gravity = new Vector3(0, -9.8f, 0);
    private PhysicsBody[] bodies = null;

    public void Step(float dt)
    {
        UpdateMotion(dt);
        List<HitPair> collisions = DetectCollisions();
        ResolveCollisions(collisions);
    }

    private void UpdateMotion(float dt)
    {
        foreach (var body in bodies)
        {
            if (body.Dynamic())
            {
                Vector3 acc = gravity;

                foreach (var other in bodies)
                {
                    if (other.shapeType == ShapeType.PLANE)
                    {
                        float projection = Vector3.Dot(gravity, other.normal);
                        Vector3 normalForce = -projection * other.normal;

                        Vector3 tangentialVelocity = body.vel - Vector3.Dot(body.vel, other.normal) * other.normal;
                        Vector3 frictionForce = -body.frictionCoefficient * normalForce.magnitude * tangentialVelocity.normalized;

                        acc += (normalForce + frictionForce) * body.invMass;
                    }
                }

                Integrate(ref body.vel, acc, dt);
                Integrate(ref body.pos, body.vel, dt);

                body.transform.position = body.pos;
            }
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

                if (a.shapeType == ShapeType.SPHERE && b.shapeType == ShapeType.PLANE)
                {
                    collision = SpherePlane(a.pos, a.radius, b.pos, b.normal, out mtv);
                }

                if (collision)
                {
                    HitPair hitPair = new HitPair { a = i, b = j, mtv = mtv };
                    collisions.Add(hitPair);
                }
            }
        }

        return collisions;
    }

    private void ResolveCollisions(List<HitPair> collisions)
    {
        foreach (HitPair collision in collisions)
        {
            PhysicsBody a = bodies[collision.a];
            PhysicsBody b = bodies[collision.b];

            Vector3 relativeVelocity = a.vel - b.vel;
            Vector3 mtvDir = collision.mtv.normalized;
            float velAlongMtv = Vector3.Dot(relativeVelocity, mtvDir);

            if (velAlongMtv > 0)
                continue;

            float restitution = Mathf.Min(a.restitutionCoefficient, b.restitutionCoefficient); 
            float impulseMagnitude = -(1 + restitution) * velAlongMtv / (a.invMass + b.invMass);

            Vector3 impulse = impulseMagnitude * mtvDir;

            if (a.Dynamic()) a.vel += impulse * a.invMass;
        }
    }

    private void Integrate(ref Vector3 value, Vector3 change, float dt)
    {
        value += change * dt;
    }

    private bool SpherePlane(Vector3 spherePos, float radius, Vector3 planePos, Vector3 planeNormal, out Vector3 mtv)
    {
        float distance = Vector3.Dot(spherePos - planePos, planeNormal);

        if (distance < radius)
        {
            mtv = planeNormal * (radius - distance);
            return true;
        }

        mtv = Vector3.zero;
        return false;
    }

    public void Init()
    {
        bodies = GameObject.FindObjectsByType<PhysicsBody>(FindObjectsSortMode.None);
    }

    public void PreStep()
    {
        foreach (var body in bodies)
        {
            body.pos = body.transform.position;
        }
    }

    public void PostStep()
    {
        foreach (var body in bodies)
        {
            Debug.DrawLine(body.pos, body.pos + gravity * 0.5f, Color.red);        // Gravity
            Debug.DrawLine(body.pos, body.pos + body.vel * 0.5f, Color.green);    // Velocity
        }
    }

    public void Quit()
    {
        bodies = null;
    }
}
