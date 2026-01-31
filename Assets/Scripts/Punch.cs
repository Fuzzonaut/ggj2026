using UnityEngine;

public class Punch : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip punchSound;

    [ContextMenu("Play Punch Sound")]
    public void PlayPunch()
    {
        audioSource.PlayOneShot(punchSound);
    }
    public int punchDamage = 25;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(punchDamage);
            }

        }

    }
}
