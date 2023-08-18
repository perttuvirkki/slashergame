using UnityEngine;

public class SpinAttack : MonoBehaviour
{
    private int spinDamage;
    private EnemyController parentEnemy;

    private void Start()
    {
        parentEnemy = transform.parent.GetComponent<EnemyController>();
        if (parentEnemy != null)
        {
            spinDamage = parentEnemy.spinDamage;
        }
        else
        {
            Debug.LogError("SpinAttack must be a child of an object with an EnemyController component.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the spinning cube collided with the player
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(spinDamage);
            }
        }
    }
}
