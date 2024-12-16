using UnityEngine;


public class Block : MonoBehaviour
{
    public float toughness = 10.0f; // Resistance to breaking

    void OnCollision(GameObject other, float impactForce)
    {
        if (impactForce > toughness)
        {
            Destroy(gameObject);
        }
    }
}
