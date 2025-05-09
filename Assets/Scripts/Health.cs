using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSytem : MonoBehaviour
{

    public int maxHealth = 100;
    private int currentHealth;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took damage: " + damage + ", current health: " + currentHealth);

        if (animator != null)
            animator.SetTrigger("Hurt"); // jika ada animasi terkena serangan

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log(gameObject.name + " died.");
        if (animator != null)
            animator.SetTrigger("Die"); // animasi mati

        // Nonaktifkan komponen yang tak dibutuhkan saat mati
        GetComponent<Collider>().enabled = false;
        GetComponent<MonoBehaviour>().enabled = false;

        // Bisa diubah: Destroy(gameObject) setelah beberapa detik
    }
    
}
