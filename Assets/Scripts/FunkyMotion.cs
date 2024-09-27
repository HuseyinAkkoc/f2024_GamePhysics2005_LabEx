using UnityEngine;

public class FunkyMotion : MonoBehaviour
{   float z;   
    public float x;
    public float y;
    public float amplitude;
    public float frequency;
    public float time;


    // Update is called once per frame
    void FixedUpdate()
    {

     

        float dt= Time.fixedDeltaTime;

      


        x= x+ (-Mathf.Sin(time*frequency)* frequency*amplitude*dt);
        y=y + (-Mathf.Cos(time * frequency) * frequency * amplitude * dt);
        transform.position = new Vector3(x,y,z);

        time += dt;

    }

   
}
