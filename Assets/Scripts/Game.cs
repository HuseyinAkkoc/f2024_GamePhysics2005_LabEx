using UnityEditor.SceneManagement;
using UnityEngine;

public class Game : MonoBehaviour
{
    // Public references to prefabs
    public GameObject spherePrefab; // Prefab for spheres
    public GameObject planePrefab;  // Prefab for the ground plane
    public GameObject aabbPrefab;   // Prefab for AABB cubes
    public GameObject launch;       // Launch point for spheres
     public float speed = 16f;
    public GameObject LaunchCylinder;
   Vector3 spawnPosition = new Vector3(-20, 1, 0);
    // Physics system instance
    private PhysicsSystem physicsSystem = new PhysicsSystem();

    void Start()
    {
        // Register collision callback
        physicsSystem.collisionCallback = CollisionTest;

        // Setup the ground plane
        SetupGroundPlane();

        // Instantiate a grid of AABB cubes
        InstantiateAABBCubes();
        Instantiate(LaunchCylinder, spawnPosition, Quaternion.identity);
    }

    void Update()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0.0f;

        // Draw a debug line for the slingshot
        Vector3 slingshot = launch.transform.position - mouse;
        Debug.DrawLine(mouse, launch.transform.position, Color.cyan);

        // Left mouse click to launch a sphere
        if (Input.GetMouseButtonDown(0))
        {
            LaunchSphere();

        }

        // Right mouse click to clear all dynamic objects
        if (Input.GetMouseButtonDown(1))
        {
            physicsSystem.Clear();
        }
    }

    void FixedUpdate()
    {
        // Step the physics simulation
        physicsSystem.PreStep();
        physicsSystem.Step(Time.fixedDeltaTime);
        physicsSystem.PostStep();
    }

    // Setup the ground plane
    private void SetupGroundPlane()
    {
        if (planePrefab == null)
        {
            Debug.LogError("Plane Prefab is not assigned!");
            return;
        }

        GameObject ground = Instantiate(planePrefab, Vector3.zero, Quaternion.identity);
        ground.name = "Ground";

        PhysicsBody body = ground.GetComponent<PhysicsBody>();
        if (body != null)
        {
            body.shapeType = ShapeType.PLANE;
            body.invMass = 0.0f; // Static plane
            body.normal = Vector3.up;
        }
    }

    // Instantiate AABB cubes in a grid layout
    private void InstantiateAABBCubes()
    {
        if (aabbPrefab == null)
        {
            Debug.LogError("AABB Prefab is not assigned!");
            return;
        }

        int rows = 3; // Number of rows
        int cols = 3; // Number of columns
        float spacing = 1.5f; // Distance between cubes

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                Vector3 position = new Vector3(x * spacing, y * spacing + 1.0f, 0);
                GameObject cube = Instantiate(aabbPrefab, position, Quaternion.identity);
                cube.name = $"AABB_Cube_{x}_{y}";

                PhysicsBody body = cube.GetComponent<PhysicsBody>();
                if (body != null)
                {
                    body.shapeType = ShapeType.AABB;
                    body.size = Vector3.one;
                    body.invMass = 1.0f; // Dynamic AABB cube
                }
            }
        }
    }

    // Launch a sphere from the launch point
    private void LaunchSphere()
    {
        if (spherePrefab == null)
        {
            Debug.LogError("Sphere Prefab is not assigned!");
            return;
        }

        // Define the spawn position: x = -20, y = 0 (ground level), z = 0
        Vector3 spawnPosition = new Vector3(-20,1, 0);

        // Instantiate the sphere
        GameObject sphere = Instantiate(spherePrefab, spawnPosition, Quaternion.identity);
        sphere.name = "Sphere";

        // Set up the sphere's physics properties
        PhysicsBody body = sphere.GetComponent<PhysicsBody>();
        if (body != null)
        {
            body.shapeType = ShapeType.SPHERE;
            body.radius = 0.5f;
            body.invMass = 150.0f; // Make it dynamic

            // Increase the speed for a faster launch
           // added at the beggining // Adjust this value to make the launch faster
            Vector3 launchVelocity = new Vector3(Mathf.Cos(25 * Mathf.Deg2Rad), Mathf.Sin(25 * Mathf.Deg2Rad), 0) * speed;

            body.vel = launchVelocity;
        }
    }



    // Collision callback: log collisions and destroy spheres
    private void CollisionTest(GameObject a, GameObject b)
    {
        PhysicsBody bodyA = a.GetComponent<PhysicsBody>();
        PhysicsBody bodyB = b.GetComponent<PhysicsBody>();

        Debug.Log($"Collision detected between {a.name} and {b.name}");

        // Example behavior: destroy spheres on collision
        if (bodyA.shapeType == ShapeType.SPHERE)
        {
            Destroy(a);
        }
        if (bodyB.shapeType == ShapeType.SPHERE)
        {
            Destroy(b);
        }
    }
}
