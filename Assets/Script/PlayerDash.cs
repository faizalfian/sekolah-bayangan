using UnityEngine;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    public AudioClip dashClip;
    private AudioSource audioSource;

    public float dashForce = 10f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        canDash = false;

        // 🔊 Mainkan suara
        if (dashClip != null)
        {
            audioSource.PlayOneShot(dashClip);
        }

        // 💨 Tambahkan dash force
        rb.velocity = transform.forward * dashForce;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
