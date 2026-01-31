using UnityEngine;

public class Punch : MonoBehaviour
{
    [Header("Damage")]
    public int punchDamage = 25;

    [Header("Sound")]
    public AudioClip punchSound;

    private AudioSource playerAudio;
    private bool hasHit = false;

    void Awake()
    {
        // find player's AudioSource
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerAudio = player.GetComponent<AudioSource>();

        if (playerAudio == null)
            Debug.LogError("Punch: Player AudioSource NOT found!", this);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy == null) return;

        hasHit = true;

        // 🔊 play sound from PLAYER
        if (playerAudio != null && punchSound != null)
            playerAudio.PlayOneShot(punchSound);

        // 💥 damage
        enemy.TakeDamage(punchDamage);

        // 🧹 cleanup punch hitbox
        Destroy(gameObject, 0.1f);
    }
}
