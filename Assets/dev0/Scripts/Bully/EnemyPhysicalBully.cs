using System.Collections;
using UnityEngine;

public class EnemyPhysicalBully : EnemyAI
{
    [Header("Physical Attack Settings")]
    public float punchDamage = 10f;
    public float kickDamage = 15f;
    public float punchCooldown = 1.5f;
    public float kickCooldown = 2f;
    public AudioClip punchSound;
    public AudioClip kickSound;

    private AudioSource audioSource;
    private float lastPunchTime;
    private float lastKickTime;

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

        // Pilih serangan fisik secara acak
        if (Time.time - lastPunchTime >= punchCooldown && Time.time - lastKickTime >= kickCooldown)
        {
            // Bisa pilih antara punch atau kick
            if (Random.Range(0, 2) == 0)
            {
                PunchAttack();
            }
            else
            {
                KickAttack();
            }
        }
        else if (Time.time - lastPunchTime >= punchCooldown)
        {
            PunchAttack();
        }
        else if (Time.time - lastKickTime >= kickCooldown)
        {
            KickAttack();
        }
    }

    private void PunchAttack()
    {
        isAttacking = true;
        animator.SetTrigger("Punch");
        PlaySound(punchSound);

        StartCoroutine(LockAttackRotation());
        StartCoroutine(ApplyMeleeDamageAfterDelay(punchDamage, 0.3f));

        lastPunchTime = Time.time;
        lastAttackTime = Time.time;
    }

    private void KickAttack()
    {
        isAttacking = true;
        //animator.SetTrigger("Kick");
        PlaySound(kickSound);

        StartCoroutine(LockAttackRotation());
        StartCoroutine(ApplyMeleeDamageAfterDelay(kickDamage, 0.4f));

        lastKickTime = Time.time;
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

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}