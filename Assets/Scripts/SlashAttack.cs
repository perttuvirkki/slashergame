using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    public float pushBackForce = 5f;
    private float spinDuration = 0.25f;
    public int damage = 1;  // Default damage value


    public int slashLevel = 0; // The current slash level, starts from 0 (first slash)
    public float[] slashRotationAmounts = { 90f, -180f, 360f }; // 360 degree clockwise, 180 degree counter-clockwise, 270 degree clockwise

    void Start()
    {
        StartCoroutine(SpinSlash());
    }

    public void SetSlashType(int newSlashLevel)
    {
        this.slashLevel = Mathf.Clamp(newSlashLevel, 0, slashRotationAmounts.Length - 1);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(damage);
            }
        }
    }



    IEnumerator SpinSlash()
    {
        float elapsedTime = 0;
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + slashRotationAmounts[slashLevel]; // Use rotation amount based on slash level

        while (elapsedTime < spinDuration)
        {
            float currentRotation = Mathf.Lerp(startRotation, endRotation, elapsedTime / spinDuration);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentRotation, transform.eulerAngles.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
