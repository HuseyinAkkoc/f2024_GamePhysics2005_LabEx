using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Bounce : MonoBehaviour
{

  public float  acc = Physics.gravity.y;
   public  float vel = 0.0f;
    public float pos = 5.0f;

     void FixedUpdate()
    {
        float dt= Time.fixedDeltaTime;
       
        // acceleration is change in velocity
        vel = vel + acc * dt;
        pos = pos + vel * dt;
        // velocity is change in position
        transform.position =  new Vector3(0.0f, pos, 0.0f);

        if (transform.position.y< 0.50f)
        {
            vel = -vel;
            pos = 5.0f;
        }
    }
}
