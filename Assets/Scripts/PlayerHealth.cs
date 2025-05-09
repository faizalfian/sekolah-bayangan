using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 200;
    public HealthBar healthBar;
    public GameObject gameOverUI;

    private int currentHealth;
    public bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxHP = maxHealth;
        healthBar.currHP = currentHealth;
        gameOverUI.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        healthBar.currHP = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        gameOverUI.SetActive(true);
        PlayerMovement playerMov = GetComponent<PlayerMovement>();
        playerMov.animator.enabled = false;
        playerMov.enabled = false;
        playerMov.resetMovement();
        GetComponent<PlayerCombat>().enabled = false;
        Debug.Log("die");
    }

    public void Respawn()
    {
        var playerMov = GetComponent<PlayerMovement>();
        var playerCombat = GetComponent<PlayerCombat>();
        var playerController = GetComponent<CharacterController>();
        playerMov.enabled = false;
        playerCombat.enabled = false;
        playerController.enabled = false;
        Vector3 respawnPos = CheckpointManager.Instance.GetRespawnPosition();

        transform.localPosition = respawnPos;
        Debug.Log($"Respawning at: {respawnPos} | Current position: {transform.localPosition}");
        Debug.Log($"Respawning at: {respawnPos} | Current position: {transform.position}");

        // Debug posisi
        isDead = false;
        currentHealth = maxHealth;
        healthBar.currHP = maxHealth;
        gameOverUI.SetActive(false);
        playerController.enabled = true;
        playerMov.animator.enabled = true;
        playerMov.enabled = true;
        playerCombat.enabled = true;
    }

    // Untuk healing item atau checkpoint
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healthBar.currHP = currentHealth;
    }

}