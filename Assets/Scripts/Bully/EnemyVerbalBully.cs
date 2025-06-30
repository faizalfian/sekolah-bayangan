using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyVerbalAI : EnemyAI
{
    [Header("Verbal Attack Settings")]
    public float wordDamage = 8f;
    public float wordCooldown = 2.5f;
    public GameObject wordProjectilePrefab;
    public Transform wordSpawnPoint;
    public AudioClip wordShoutSound;

    private AudioSource audioSource;

    //protected override void Start()
    //{
    //    base.Start();
    //    audioSource = GetComponent<AudioSource>();
    //    if (audioSource == null)
    //    {
    //        audioSource = gameObject.AddComponent<AudioSource>();
    //    }
    //}

    protected override void AttackPlayer()
    {
        if (isAttacking) return;

        if (!agent.isStopped)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
        }

        // Rotasi menghadap player
        Quaternion lastRot = transform.rotation;
        transform.LookAt(playerTransform.position);
        transform.rotation = Quaternion.Lerp(lastRot, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), 0.5f);
        attackRotation = transform.rotation;

        if (Time.time - lastAttackTime >= wordCooldown)
        {
            WordAttack();
        }
    }

    private void WordAttack()
    {
        isAttacking = true;
        //animator.SetTrigger("Shout");
        PlaySound(wordShoutSound);

        StartCoroutine(LockAttackRotation());
        StartCoroutine(SpawnWordProjectile(0.5f));

        lastAttackTime = Time.time;
    }

    private IEnumerator SpawnWordProjectile(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (wordProjectilePrefab != null && wordSpawnPoint != null)
        {
            GameObject word = Instantiate(wordProjectilePrefab, wordSpawnPoint.position, Quaternion.identity);
            WordProjectile wordScript = word.GetComponent<WordProjectile>();

            if (wordScript != null)
            {
                wordScript.damage = (int)wordDamage;
                wordScript.target = playerTransform;
            }
        }

        isAttacking = false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}