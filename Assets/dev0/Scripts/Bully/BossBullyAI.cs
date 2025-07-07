using System.Collections;
using UnityEngine;

public class EnemyBullyBossAI : EnemyBossAI
{
    [Header("Bully Boss Settings")]
    public float punchDamage = 15f;
    public float kickDamage = 20f;
    public float wordDamage = 10f;
    public float shockwaveDamage = 25f;
    public float shockwaveRadius = 5f;

    [Header("Attack References")]
    public GameObject wordProjectilePrefab;
    public Transform wordSpawnPoint;
    public GameObject shockwaveEffect;

    [Header("Audio")]
    public AudioClip punchSound;
    public AudioClip kickSound;
    public AudioClip wordShoutSound;
    public AudioClip rageShoutSound;
    public AudioClip shockwaveSound;

    private AudioSource audioSource;
    private bool isInRageMode = false;

    //protected override void Start()
    //{
    //    base.Start();
    //    audioSource = GetComponent<AudioSource>();
    //    if (audioSource == null)
    //    {
    //        audioSource = gameObject.AddComponent<AudioSource>();
    //    }
    //}

    protected override void variationAttack()
    {
        if (isInRageMode)
        {
            // During rage, more likely to use powerful attacks
            int attackType = Random.Range(0, 3);
            switch (attackType)
            {
                case 0:
                    PunchAttack();
                    break;
                case 1:
                    KickAttack();
                    break;
                case 2:
                    WordAttack();
                    break;
            }
        }
        else
        {
            // Normal attack pattern
            int attackType = Random.Range(0, 4);
            switch (attackType)
            {
                case 0:
                case 1:
                    PunchAttack();
                    break;
                case 2:
                    KickAttack();
                    break;
                case 3:
                    WordAttack();
                    break;
            }
        }
    }

    private void PunchAttack()
    {
        isAttacking = true;
        animator.SetTrigger("Punch");
        //PlaySound(punchSound);

        StartCoroutine(LockAttackRotation());
        StartCoroutine(ApplyMeleeDamageAfterDelay(punchDamage, 0.3f));

        lastAttackTime = Time.time;
    }

    private void KickAttack()
    {
        isAttacking = true;
        //animator.SetTrigger("Kick");
        //PlaySound(kickSound);

        StartCoroutine(LockAttackRotation());
        StartCoroutine(ApplyMeleeDamageAfterDelay(kickDamage, 0.4f));

        lastAttackTime = Time.time;
    }

    private void WordAttack()
    {
        isAttacking = true;
        //animator.SetTrigger("Shout");
        //PlaySound(wordShoutSound);

        StartCoroutine(LockAttackRotation());
        StartCoroutine(SpawnWordProjectile(0.5f));

        lastAttackTime = Time.time;
    }

    private IEnumerator ApplyMeleeDamageAfterDelay(float damage, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= attackRadius * 1.2f)
        {
            Health playerH = player.GetComponent<Health>();
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (playerH != null && angleToPlayer < 60f)
            {
                playerH.TakeDamage((int)damage);
            }
        }

        isAttacking = false;
    }

    private IEnumerator SpawnWordProjectile(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (wordProjectilePrefab != null && wordSpawnPoint != null)
        {
            GameObject word = Instantiate(wordProjectilePrefab, transform.position + transform.forward, Quaternion.identity);
            WordProjectile wordScript = word.GetComponent<WordProjectile>();

            if (wordScript != null)
            {
                wordScript.damage = (int)wordDamage;
                wordScript.target = playerTransform.position;
            }
        }

        isAttacking = false;
    }

    public override void onRage(int rageCount)
    {
        if (!isInRageMode)
        {
            isInRageMode = true;
            StartCoroutine(RageMode());
        }
    }

    private IEnumerator RageMode()
    {
        // Play rage animation and sound
        //animator.SetTrigger("Rage");
        //PlaySound(rageShoutSound);

        // Stop current actions
        if (!agent.isStopped)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
        }

        // Create shockwave after delay
        yield return new WaitForSeconds(1f);
        CreateShockwave();

        // Stay in rage mode for a while
        yield return new WaitForSeconds(5f);
        isInRageMode = false;
    }

    private void CreateShockwave()
    {
        // Visual effect
        if (shockwaveEffect != null)
        {
            Instantiate(shockwaveEffect, transform.position, Quaternion.identity);
        }

        PlaySound(shockwaveSound);

        // Damage all players in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, shockwaveRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Health playerHealth = hitCollider.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)shockwaveDamage);
                }
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    protected override void UpdateAnimations()
    {
        base.UpdateAnimations();
        animator.SetBool("RageMode", isInRageMode);
    }
}