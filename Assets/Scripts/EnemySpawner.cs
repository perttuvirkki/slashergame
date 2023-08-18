using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;       // Drag your enemy prefab here
    public float spawnInterval = 5f;     // Time between spawns
    public float upwardSpeed = 1f;       // Speed at which the enemy moves upwards
    public float inwardSpeed = 1f;       // Speed at which the enemy moves towards the center

    private Vector3 center;

    private void Start()
    {
        center = new Vector3(0, -23.8f, 0);   // Assuming the center of your cube is at (0,0,0)
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // Decide the axis and direction for spawning
            bool useXAxis = Random.Range(0, 2) == 0;
            float direction = Random.Range(0, 2) == 0 ? -1f : 1f;

            Vector3 spawnPosition = center;
            if (useXAxis)
            {
                spawnPosition.x += 25.5f * direction;
                spawnPosition.z += Random.Range(-25.5f, 25.5f);  // Random position on Z axis
            }
            else
            {
                spawnPosition.z += 25.5f * direction;
                spawnPosition.x += Random.Range(-25.5f, 25.5f);  // Random position on X axis
            }

            Quaternion enemyRotation;
            if (useXAxis)
            {
                enemyRotation = Quaternion.LookRotation(Vector3.left * direction);
            }
            else
            {
                enemyRotation = Quaternion.LookRotation(Vector3.back * direction);
            }
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, enemyRotation);

            // Disable NavMeshAgent after spawning
            UnityEngine.AI.NavMeshAgent agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
                agent.enabled = false;

            StartCoroutine(MoveEnemy(enemy, useXAxis, direction, agent));

            yield return new WaitForSeconds(spawnInterval);
        }
    }


    IEnumerator MoveEnemy(GameObject enemy, bool useXAxis, float direction, UnityEngine.AI.NavMeshAgent agent)
    {
        Vector3 endPosition = enemy.transform.position + Vector3.up * 25f;
        while (Vector3.Distance(enemy.transform.position, endPosition) > 0.1f)
        {
            enemy.transform.position += Vector3.up * upwardSpeed * Time.deltaTime;
            yield return null;
        }

        endPosition = enemy.transform.position + (useXAxis ? Vector3.right : Vector3.forward) * -1.5f * direction;
        while (Vector3.Distance(enemy.transform.position, endPosition) > 0.1f)
        {
            Vector3 moveDirection = (useXAxis ? Vector3.right : Vector3.forward) * inwardSpeed * Time.deltaTime * -direction;
            enemy.transform.position += moveDirection;
            yield return null;
        }

        // Re-enable NavMeshAgent once the movement is complete
        if (agent != null)
            agent.enabled = true;
    }
}
