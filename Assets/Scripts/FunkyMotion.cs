using UnityEngine;

public class FunkyMotion : MonoBehaviour
{
    public float x;
    public float y;
    public float amplitude;
    public float frequency;
    public float time;


    // Update is called once per frame
    void FixedUpdate()
    {

     

        float dt= Time.fixedDeltaTime;

        float z = 0.0f;   //Example of frecuency & amplitude // Mathf.Sin(time* 10.0f)*5.0f;   


        x= x+ (-Mathf.Sin(time*frequency)* frequency*amplitude*dt);
        y=y + (-Mathf.Cos(time * frequency) * frequency * amplitude * dt);
        transform.position = new Vector3(x,y,z);

        time += dt;

    }

   
}
