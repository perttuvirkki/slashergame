using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackPoint : MonoBehaviour
{
    public GameObject slashPrefab;
    private int consecutiveSlashes = 0;
    private float timeBetweenSlashes = 0.7f;  // Maximum time allowed between slashes to consider them consecutive.
    private float timeSinceLastSlash = 0;

    private void Update()
    {
        if (timeSinceLastSlash <= timeBetweenSlashes)
        {
            timeSinceLastSlash += Time.deltaTime;
        }
        else
        {
            consecutiveSlashes = 0;  // Reset the counter if the time since the last slash exceeded the limit.
        }
    }

    public void SlashAttack()
    {
        consecutiveSlashes++;

        switch (consecutiveSlashes)
        {
            case 1: // First slash
                CreateSlash(transform.position + transform.forward * 0.25f, transform.rotation, new Vector3(3f, 0.2f, 3f), 1); // Original scale
                break;
            case 2: // Second slash (two slashes on front)
                CreateSlash(transform.position + transform.forward * 0.5f + transform.right * 0.5f, transform.rotation, new Vector3(3f, 0.2f, 3f), 1); // Example scales, adjust as needed
                CreateSlash(transform.position + transform.forward * 0.5f - transform.right * 0.5f, transform.rotation, new Vector3(3f, 0.2f, 3f), 1);
                break;
            case 3: // Third slash (one big slash in the middle)
                CreateSlash(transform.position, transform.rotation, new Vector3(5, 0.2f, 5), 3); // Example scales, adjust as needed
                consecutiveSlashes = 0; // Reset the counter after the third slash
                break;
        }


        timeSinceLastSlash = 0f; // Reset the timer after a slash
    }

    private void CreateSlash(Vector3 position, Quaternion rotation, Vector3 scale, int damageAmount)
    {
        GameObject slash = Instantiate(slashPrefab, position, rotation);
        Slash slashInstance = slash.GetComponent<Slash>();
        slashInstance.damage = damageAmount;

        slash.transform.localScale = scale;
        slash.GetComponent<Slash>().SetSlashType(consecutiveSlashes - 1);  // Inform the Slash script about the type of slash
        slash.transform.SetParent(transform, true);
        Destroy(slash, 0.2f);
    }

}
