using TMPro;
using UnityEngine;

public class WordProjectile : MonoBehaviour
{
    public int damage = 10;
    public float speed = 5f;
    public Vector3 target;
    public TextMeshPro tm;
    public float lifetime = 3f;

    private void Start()
    {
        const string randomLetter = "abcdef#@&%*!?";
        string randomWord = "";
        int randomLen = Random.Range(3, 7);
        for(int i = 0; i < randomLen; i++)
        {
            randomWord += randomLetter[Random.Range(0, randomLetter.Length - 1)];
        }
        tm.text = randomWord;
        Destroy(gameObject, lifetime);
        transform.LookAt(target);
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}