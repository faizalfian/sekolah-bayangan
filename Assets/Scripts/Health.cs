using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public HealthBar healthBar;
    public bool immuneToDamage = false; // Apakah immune terhadap damage
    private int currHP;
    private bool isDead;

    [Header("Respawn Settings")]
    public bool canRespawn = false;
    public bool haveDeathscreen = false;
    public bool doNothingWHenDie = false;
    public float respawnTime = 3f; // Waktu respawn dalam detik
    public GameObject gameOverUI;
    public Animator animator;

    [Header("Death Settings")]
    public MonoBehaviour[] componentsToDisableOnDeath;
    public Collider[] collidersToDisableOnDeath;

    [Header("Events")]
    public UnityEvent<int> onDeath; // Event untuk ketika mati (dengan parameter score)

    // Start is called before the first frame update
    void Start()
    {
        currHP = maxHealth;
        if(haveDeathscreen && gameOverUI != null) gameOverUI.SetActive(false);
        updateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || immuneToDamage) return;

        Debug.Log("Take Damage");

        currHP  = Mathf.Clamp(currHP - damage, 0, maxHealth);

        updateHealthBar();

        if (animator != null) animator.SetTrigger("Hurt");

        if (currHP <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currHP = Mathf.Clamp(currHP + healAmount, 0, maxHealth);
        updateHealthBar();
    }

    public bool isDeath()
    {
        return isDead;
    }

    void resetHP()
    {
        currHP = maxHealth;
        updateHealthBar();
    }

    void updateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.maxHP = maxHealth;
            healthBar.currHP = currHP;
        }
    }

    public void Respawn() // for player
    {
        if (!isDead) return;
        Vector3 respawnPos = CheckpointManager.Instance.GetRespawnPosition();

        transform.position = respawnPos;
        Debug.Log(transform.position);

        resetHP();

        gameObject.SetActive(true);
        gameOverUI.SetActive(false);

        foreach (var component in componentsToDisableOnDeath)
        {
            if (component != null) component.enabled = true;
        }

        foreach (var collider in collidersToDisableOnDeath)
        {
            if (collider != null) collider.enabled = true;
        }

        isDead = false;
    }

    private void Die()
    {
        isDead = true;
        if (animator != null) animator.SetTrigger("Die");

        // Nonaktifkan komponen yang ditentukan
        foreach (var component in componentsToDisableOnDeath)
        {
            if (component != null) component.enabled = false;
        }

        foreach (var collider in collidersToDisableOnDeath)
        {
            if (collider != null) collider.enabled = false;
        }


        // Jika bisa respawn, jalankan respawn setelah delay
        if (canRespawn && !haveDeathscreen)
        {
            StartCoroutine(MoveToBottom());
            StartCoroutine(RespawnAfterDelay());
        }
        else if (haveDeathscreen)
        {
            // Jika ada deathscreen, tampilkan deathscreen
            showDeathsreen();
        } else if(doNothingWHenDie)
        {
            onDeath?.Invoke(CalculateScoreValue());
        }
        else
        {
            StartCoroutine(MoveToBottom());
            // Panggil event onDeath jika tidak bisa respawn
            onDeath?.Invoke(CalculateScoreValue());
            Destroy(gameObject, 1.5f); // Hapus objek setelah delay
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnTime);

        // Reset health
        currHP = maxHealth;

        transform.position += new Vector3(0f, 1.5f, 0f);

        // Aktifkan kembali komponen yang dimatikan
        foreach (var component in componentsToDisableOnDeath)
        {
            if (component != null) component.enabled = true;
        }

        foreach (var collider in collidersToDisableOnDeath)
        {
            if (collider != null) collider.enabled = true;
        }

        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.Play("Idle"); // Ganti dengan state awal animasi yang sesuai
        }
    }

    private void showDeathsreen()
    {
        gameObject.SetActive(false);
        gameOverUI.SetActive(true);
        
    }

    private int CalculateScoreValue()
    {
        // Anda bisa menyesuaikan perhitungan score berdasarkan kebutuhan
        // Contoh sederhana: return maxHealth sebagai score
        return maxHealth / 2;
    }

    protected IEnumerator MoveToBottom()
    {
        Vector3 startPos = transform.position;
        Vector3 bottom = new Vector3(startPos.x, -5f, startPos.z); // Ganti dengan posisi bawah yang sesuai
        float elapsed = 0f;
        float duration = 0.75f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, bottom, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = bottom;
    }
}