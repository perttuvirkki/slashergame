using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerInput controller;
    public float moveSpeed = 5f;
    public float dashSpeed = 6f;  // Speed of the dash
    public float dashDuration = 0.2f;  // Duration of the dash in seconds
    public float dodgeSpeed = 15f;  // Speed of the dash
    public Rigidbody rb;
    public PlayerAttackPoint PlayerAttackPoint;
    private bool allowRotation = true;  // Add this line
    public Camera camera;  // Reference to the main camera
    private float slashCooldown = 0.3f;  // Duration of the cooldown for slashing
    private float timeSinceLastSlash = 0f;
    private bool canSlash = true;

    public int maxHealth = 5; // Maximum health of the player
    private int currentHealth; // Current health of the player
    public delegate void PlayerHealthChanged(int health); // Delegate for broadcasting health changes
    public event PlayerHealthChanged onPlayerHealthChanged; // Event to notify when the player's health changes
    public int experience = 0;
    public int maxExperience = 100;  // You can adjust this value as needed

    private bool isImmune = false; // Flag to determine if the player is currently immune
    private float immunityDuration = 1.0f; // Duration of immunity in seconds
    private float immunityTimer = 0.0f; // Timer to track how long the player has been immune
    private float flashTimer = 0.0f;
    private float flashDuration = 0.1f;

    public Material regularMaterial; // Drag the regular material here in the inspector
    public Material flashingMaterial; // Drag the flashing material here in the inspector
    private MeshRenderer meshRenderer; // Reference to the MeshRenderer

    public ExperienceBar experienceBar;  // Drag the ExperienceBarContainer with the script here in the Inspector
    public HealthBlocks healthBlocks;

    [HideInInspector]
    public Vector3 moveDir;
    [HideInInspector]
    public Vector3 lastMoveDir;

    private bool isDashing = false;  // Flag to determine if the player is dashing

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = new PlayerInput();
        controller.Enable();
        currentHealth = maxHealth;
        healthBlocks.SetHealth(currentHealth);
        meshRenderer = GetComponent<MeshRenderer>();

    }

    void Update()
    {
        if (!canSlash)
        {
            timeSinceLastSlash += Time.deltaTime;
            if (timeSinceLastSlash >= slashCooldown)
            {
                canSlash = true;
                timeSinceLastSlash = 0f;
            }
        }

        InputManagement();

        if (isImmune)
        {
            immunityTimer -= Time.deltaTime;
            flashTimer -= Time.deltaTime;

            if (flashTimer <= 0)
            {
                // Toggle material by checking material name
                if (meshRenderer.material.name.StartsWith(regularMaterial.name))
                {
                    meshRenderer.material = flashingMaterial;
                }
                else
                {
                    meshRenderer.material = regularMaterial;
                }

                flashTimer = flashDuration;
            }


            if (immunityTimer <= 0)
            {
                isImmune = false;
                meshRenderer.material = regularMaterial; // Ensure player returns to regular material after immunity ends
            }
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            Move();
        }

        if (moveDir != Vector3.zero && allowRotation)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, moveSpeed * 3 * Time.deltaTime);
        }
    }

    void InputManagement()
    {
        float moveX = controller.Player.LeftStick.ReadValue<Vector2>().x;
        float moveZ = controller.Player.LeftStick.ReadValue<Vector2>().y;

        Vector3 camFwd = Vector3.Scale(camera.transform.forward, new Vector3(1, 0, 1)).normalized;
        moveDir = moveZ * camFwd + moveX * camera.transform.right;
        if (moveDir != Vector3.zero)
        {
            lastMoveDir = moveDir;
        }

        if (controller.Player.Attack.triggered && canSlash)
        {
            PlayerAttackPoint.SlashAttack();
            StartDash();  // Dash with the attack
            canSlash = false;
        }

        // New dodge logic
        if (controller.Player.Dodge.triggered && canSlash)  // Assuming you've defined "Dodge" in your PlayerInput actions
        {
            StartDodge();  // Start a dodge move, which will also grant immunity
        }
    }


    void Move()
    {
        rb.velocity = new Vector3(moveDir.x * moveSpeed, 0, moveDir.z * moveSpeed);
    }

    void StartDash()
    {
        if (!isDashing)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    IEnumerator DashCoroutine()
    {
        allowRotation = false;  // Disable rotation at the start of the dash
        isDashing = true;

        Vector3 dashDirection = transform.forward.normalized;
        rb.velocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector3.zero;
        isDashing = false;
        allowRotation = true;  // Re-enable rotation after the dash
    }

    void StartDodge()
    {
        if (!isDashing)
        {
            // Activate immunity for dodge
            isImmune = true;
            immunityTimer = immunityDuration;

            StartCoroutine(DodgeCoroutine());
        }
    }

    IEnumerator DodgeCoroutine()
    {
        allowRotation = false;  // Disable rotation at the start of the dodge
        isDashing = true;

        Vector3 dodgeDirection = transform.forward.normalized;
        rb.velocity = dodgeDirection * dodgeSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector3.zero;
        isDashing = false;
        allowRotation = true;  // Re-enable rotation after the dodge

    }


    public void TakeDamage(int damage)
    {
        if (isImmune)
        {
            // If the player is immune, we don't apply any damage.
            return;
        }

        currentHealth -= damage;

        // Activate immunity
        isImmune = true;
        immunityTimer = immunityDuration;

        // Your existing code...
        // Ensure health doesn't drop below zero
        currentHealth = Mathf.Max(currentHealth, 0);
        healthBlocks.SetHealth(currentHealth);

        // Notify any listeners about the health change (for UI or other systems)
        onPlayerHealthChanged?.Invoke(currentHealth);

        // Check if the player died
        if (currentHealth <= 0)
        {
            Die();
        }
    }


    public void AddExperience(int amount)
    {
        experience += amount;
        // Here you can also implement leveling up mechanisms, UI updates, etc.

        // Calculate percentage experience and set the bar value
        float experiencePercentage = (float)experience / maxExperience;  // Make sure maxExperience matches the one in ExperienceBar script
        experienceBar.SetExperience(experiencePercentage);

        Debug.Log("Experience added! Total experience: " + experience);
    }

    private void Die()
    {
        // Handle player death (e.g., respawn, load last checkpoint, show game over screen)
        Debug.Log("Player Died!");
        // This is just an example. Depending on your game design, you may want to 
        // restart the level, load a game over scene, or do something else.
    }

}
