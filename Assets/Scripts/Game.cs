using UnityEngine;

public class Game : MonoBehaviour
{
    PhysicsSystem physicsSystem = new PhysicsSystem();

    void Start()
    {
        
        // Initialize bodies and them to physics system
        PhysicsBody Testbody1 = new PhysicsBody();
        PhysicsBody Testbody2 = new PhysicsBody();
        PhysicsBody Testbody3 = new PhysicsBody();
        PhysicsBody Testbody4 = new PhysicsBody();

        // TestBody1

        Testbody1.pos = new Vector3(40.0f, 0.0f, 0.0f);
        Testbody1.vel = new Vector3(-30.0f, 50.0f, 0.0f);
        Testbody1.drag = 0.05f; // *Must be between 0.0 and 1.0*

        

       //TestBody 2
        Testbody2.pos = new Vector3(30.0f, 0.0f, 0.0f);
        Testbody2.vel = new Vector3(0.0f, 20.0f, 0.0f);
        Testbody2.drag = 0.1f;
        


        //TestBody3
        Testbody3.pos = new Vector3(0.0f, 0.0f, 0.0f);
        Testbody3.vel = new Vector3(0.0f, 30.0f, 0.0f);
        Testbody3.drag = 0.2f;


        //TestBody4
        Testbody4.pos = new Vector3(-30.0f, 0.0f, 0.0f);
        Testbody4.vel = new Vector3(10.0f, 30.0f, 0.0f);
        Testbody4.drag = 0.2f;


        physicsSystem.bodies.Add( Testbody1);
        physicsSystem.bodies.Add(Testbody2);
        physicsSystem.bodies.Add(Testbody3);
        physicsSystem.bodies.Add(Testbody4);

    }

    void FixedUpdate()
    {
       
        physicsSystem.Step(Time.fixedDeltaTime);
    }

    void OnDrawGizmos()
    {
     
        // TODO -- consider making this into a "Render()" method of PhysicsSystem
        for (int i = 0; i < physicsSystem.bodies.Count; i++)
        {
            PhysicsBody body = physicsSystem.bodies[i];
            Gizmos.DrawSphere(body.pos, 1.0f);
        }
    }
   
}