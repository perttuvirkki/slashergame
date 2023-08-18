using UnityEngine;

public class ExperienceCube : MonoBehaviour
{
    public int experienceValue = 1;  // This can be adjusted based on the value you want each cube to provide.
    private float lifespan = 10f;  // How long before the cube disappears.

    void Start()
    {
        Destroy(gameObject, lifespan);  // Destroy the cube after its lifespan.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add experience to player
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddExperience(experienceValue);
            }
            Destroy(gameObject);  // Destroy the cube after being collected.
        }
    }
}
