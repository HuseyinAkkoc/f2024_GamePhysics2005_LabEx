using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Bounce : MonoBehaviour
{


    float vel = 0.0f;

     void FixedUpdate()
    {
        float dt= Time.fixedDeltaTime;
       float  acc = Physics.gravity.y;

        vel = vel + acc * dt;
        transform.position = transform.position + new Vector3(0.0f, vel * dt, 0.0f);
    }
}
