using UnityEngine;

public class Game : MonoBehaviour
{
    private PhysicsSystem physicsSystem = new PhysicsSystem();

    void Start()
    {
        // Create and rotate the ground plane
        GameObject planeObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        planeObject.transform.rotation = Quaternion.Euler(-20, 0, 0); // Rotate -20 degrees on the X-axis
        planeObject.name = "Ground";
        PhysicsBody planeBody = planeObject.AddComponent<PhysicsBody>();
        planeBody.shapeType = ShapeType.PLANE;
        planeBody.normal = planeObject.transform.up; // Plane's normal direction
        planeBody.invMass = 0.0f; // Static object
        planeBody.restitutionCoefficient = 0.0f; // No bouncing for the plane

        // Create 4 spheres with different properties
        CreateSphere(new Vector3(-2, 5, 0), Vector3.zero, 2.0f, 0.1f, Color.red);   
        CreateSphere(new Vector3(0, 5, 0), Vector3.zero, 2.0f, 0.8f, Color.green);  
        CreateSphere(new Vector3(2, 5, 0), Vector3.zero, 8.0f, 0.1f, Color.blue);  
        CreateSphere(new Vector3(4, 5, 0), Vector3.zero, 8.0f, 0.8f, Color.yellow); 

        // Initialize the physics system
        physicsSystem.Init();
    }

    void FixedUpdate()
    {
        physicsSystem.PreStep();
        physicsSystem.Step(Time.fixedDeltaTime);
        physicsSystem.PostStep();
    }

    void OnDestroy()
    {
        physicsSystem.Quit();
    }

    private void CreateSphere(Vector3 position, Vector3 velocity, float mass, float friction, Color color)
    {
        // Create a new sphere GameObject
        GameObject sphereObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereObject.name = "Sphere";
        sphereObject.transform.position = position;

        // Add PhysicsBody component
        PhysicsBody sphereBody = sphereObject.AddComponent<PhysicsBody>();
        sphereBody.pos = position;
        sphereBody.vel = velocity;
        sphereBody.invMass = 1.0f / mass; // Inverse mass
        sphereBody.frictionCoefficient = friction;
        sphereBody.shapeType = ShapeType.SPHERE;
        sphereBody.restitutionCoefficient = 0.0f; // No bouncing for spheres

        // Set sphere color
        Renderer renderer = sphereObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }
}
