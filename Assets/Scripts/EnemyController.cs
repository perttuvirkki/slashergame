using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    private Vector3 movement;
    private Transform player;
    public UnityEngine.AI.NavMeshAgent agent;  // Add this

    public int health = 3;
    public Material[] materials;
    public Material damageMaterial;
    public GameObject enemyDeathPrefab;  // Assign your EnemyDeath prefab here in the inspector
    public float attackRange = 1.5f;  // The range within which the enemy initiates the attack
    public float attackDuration = 0.2f;  // How long the enemy prepares before executing the spin attack
    public float spinAttackDuration = 0.1f;  // Duration of the spinning attack
    public float spinDamageRange = 1.0f; // Range within which the spin attack damages the player
    public int spinDamage = 1; // Damage dealt by spin attack
    public GameObject spinAttackPrefab; // Assign your spin attack prefab here in the inspector
    private GameObject currentSpinAttack; // The current spinning attack instance (if one exists)


    private bool isAttacking = false;


    private Renderer renderer;
    private bool isFlashing = false;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        renderer = GetComponent<Renderer>();
        agent.speed = speed;

        // Safety check for the correct number of materials and health initialization
        if (materials != null && materials.Length > 0 && health > 0 && health <= materials.Length)
        {
            renderer.material = materials[health - 1];
        }
    }
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // If the enemy is not already attacking and the player is within attack range
        if (!isAttacking && distanceToPlayer <= attackRange)
        {
            StartCoroutine(AttackSequence());
            if (agent.enabled) // Add this check
                agent.SetDestination(transform.position);  // Stop the agent from moving when attacking
        }
        else if (!isAttacking)
        {
            if (agent.enabled) // Add this check
                agent.SetDestination(player.position);  // Set the NavMeshAgent's destination to the player
        }
    }


    public void TakeDamage(int damage)
    {
        health -= damage;

        Material originalMaterial = null;

        if (health > 0 && materials != null && health <= materials.Length)
        {
            originalMaterial = materials[health - 1];
        }

        if (!isFlashing)
            StartCoroutine(DamageFlash(originalMaterial));

        if (health <= 0)
        {
            Die();
        }
    }


    IEnumerator DamageFlash(Material originalMaterial)
    {
        isFlashing = true;

        renderer.material = damageMaterial;
        yield return new WaitForSeconds(0.2f);

        if (originalMaterial != null)
        {
            renderer.material = originalMaterial;
        }

        isFlashing = false;
    }
    IEnumerator AttackSequence()
    {
        isAttacking = true;

        // 1. Stop the NavMeshAgent
        agent.isStopped = true;

        // 1.5. Scale down the enemy on the y-axis and enlarge x and z axis over attackDuration time
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = new Vector3(1.5f, 0.5f, 1.5f);  // x and z are 1.5 and y is 0.5
        for (float t = 0; t < attackDuration; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, t / attackDuration);
            yield return null;
        }


        // 3. Spin attack and scale back up to 1:1 simultaneously over 0.2 seconds
        float rotationSpeed = 360f / spinAttackDuration; // Calculate speed required for one revolution in given time
        float popBackDuration = 0.1f;
        for (float t = 0; t < popBackDuration; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(targetScale, Vector3.one, t / popBackDuration);  // Vector3.one is (1,1,1)
            yield return null;
        }
        transform.localScale = Vector3.one;  // Ensure the scale is exactly (1,1,1)

        // 2. Instantiate the spinning cube
        currentSpinAttack = Instantiate(spinAttackPrefab, transform.position, Quaternion.identity, transform);

        // 4. Continue spin attack without scaling
        float remainingSpinTime = spinAttackDuration - popBackDuration;
        for (float t = 0; t < remainingSpinTime; t += Time.deltaTime)
        {
            currentSpinAttack.transform.RotateAround(transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        // 5. Destroy the spinning cube
        Destroy(currentSpinAttack);

        // 6. Re-enable the NavMeshAgent
        agent.isStopped = false;

        isAttacking = false;
    }


    public void Die()
    {
        if (enemyDeathPrefab != null)
        {
            Instantiate(enemyDeathPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
