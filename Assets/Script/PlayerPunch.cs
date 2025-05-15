using UnityEngine;

public class PlayerPunch : MonoBehaviour
{
    public AudioClip punchClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) // misalnya tombol punch
        {
            Punch();
        }
    }

    void Punch()
    {
        // Panggil animasi juga kalau ada
        audioSource.PlayOneShot(punchClip);
    }
}
