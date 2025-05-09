using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public HealthBar healthBar;
    public GameObject gameOverUI;

    private int currentHealth;
    private bool isDead;

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

        currentHealth -= damage;
        healthBar.currHP = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        //gameOverUI.SetActive(true);
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerCombat>().enabled = false;
    }

    // Untuk healing item atau checkpoint
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healthBar.currHP = currentHealth;
    }
}