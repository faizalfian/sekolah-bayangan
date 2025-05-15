using UnityEngine;

public class PlayerFootstep : MonoBehaviour
{
    public AudioClip footstepClip;
    public float footstepDelay = 0.5f;
    private AudioSource audioSource;
    private float timer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        timer = 0f;
    }

    void Update()
    {
        bool isMoving = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;

        if (isMoving)
        {
            timer += Time.deltaTime;
            if (timer >= footstepDelay)
            {
                audioSource.PlayOneShot(footstepClip);
                timer = 0f;
            }
        }
        else
        {
            timer = footstepDelay; // Reset timer biar gak langsung bunyi pas mulai gerak lagi
        }
    }
}
