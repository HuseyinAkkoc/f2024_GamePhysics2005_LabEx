using UnityEngine;

public class Pig : MonoBehaviour
{
    public float toughness = 5.0f; // Resistance to breaking

    void OnCollision(GameObject other, float impactForce)
    {
        if (impactForce > toughness)
        {
            Destroy(gameObject);
            Debug.Log("Pig Destroyed!");
        }
    }
}
