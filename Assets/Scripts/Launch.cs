using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdLauncher : MonoBehaviour
{
    public GameObject circularBirdPrefab; // Prefab for the circular bird
    public GameObject squareBirdPrefab;   // Prefab for the square bird

    private GameObject currentBird;       // Currently selected bird
    private bool isDragging = false;
    private Vector3 initialPosition;
    private Vector3 launchDirection;
    private float maxLaunchDistance = 5f; // Maximum slingshot stretch distance

    private PhysicsSystem physicsSystem;

    void Start()
    {
        // Initialize the physics system
        physicsSystem = new PhysicsSystem();
        physicsSystem.gravity = new Vector3(0, -9.8f, 0);

        // Spawn the first bird (default to circular)
        SpawnBird(circularBirdPrefab);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Start dragging
            initialPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            initialPosition.z = 0f;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            // Update drag position
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePosition.z = 0f;

            Vector3 dragVector = initialPosition - currentMousePosition;

            // Clamp drag distance
            if (dragVector.magnitude > maxLaunchDistance)
            {
                dragVector = dragVector.normalized * maxLaunchDistance;
            }

            currentBird.transform.position = initialPosition - dragVector;
            launchDirection = dragVector;
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            // Launch bird
            isDragging = false;
            LaunchBird(launchDirection);
        }

        // Switch bird type with keyboard input (C for circular, S for square)
        if (Input.GetKeyDown(KeyCode.C))
        {
            SpawnBird(circularBirdPrefab);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            SpawnBird(squareBirdPrefab);
        }

        // Update physics system
        physicsSystem.PreStep();
        physicsSystem.Step(Time.deltaTime);
        physicsSystem.PostStep();
    }

    private void SpawnBird(GameObject birdPrefab)
    {
        if (currentBird != null)
        {
            Destroy(currentBird);
        }

        currentBird = Instantiate(birdPrefab, transform.position, Quaternion.identity);

        // Attach a PhysicsBody to the bird
        PhysicsBody body = currentBird.AddComponent<PhysicsBody>();
        body.shapeType = birdPrefab == circularBirdPrefab ? ShapeType.SPHERE : ShapeType.PLANE;
      //  body.restitutionCoefficient = birdPrefab == circularBirdPrefab ? 0.8f : 0.5f; // Example values
        ///body.frictionCoefficient = birdPrefab == circularBirdPrefab ? 0.3f : 0.6f;   // Example values
        body.invMass = 1.0f / (birdPrefab == circularBirdPrefab ? 1.0f : 2.0f);       // Example values
        body.drag = 0.98f; // Adjust drag for air resistance
    }

    private void LaunchBird(Vector3 direction)
    {
        PhysicsBody body = currentBird.GetComponent<PhysicsBody>();

        if (body != null)
        {
            // Apply launch force
            float launchForce = direction.magnitude * 10f; // Adjust multiplier as needed
            body.vel = direction.normalized * launchForce;
        }
    }
}
