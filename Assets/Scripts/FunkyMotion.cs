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

        float z= Mathf.Sin(time);   

        float dt= Time.fixedDeltaTime;

        transform.position = new Vector3(x,y,z);

        time += dt;

    }

   
}
