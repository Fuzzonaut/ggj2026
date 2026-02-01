using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public int scoreValue = 10; 

    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // --- YENİ EKLENEN KISIM BAŞLANGIÇ ---
        // Eğer bu düşmanın üzerinde patlama scripti varsa, onu sustur!
        ExploderEnemy exploder = GetComponent<ExploderEnemy>();
        if (exploder != null)
        {
            exploder.enabled = false; // Artık Update fonksiyonu çalışmayacak
            exploder.StopAllCoroutines(); // Eğer patlama sayacı başladıysa onu da durdur
        }
        // --- YENİ EKLENEN KISIM BİTİŞ ---

        // Skor ekle
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(scoreValue);
        }

        // Fiziği kapat
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; 

        // Normal ölüm animasyonunu oynat
        if (anim != null)
        {
            anim.SetTrigger("Die");
            Destroy(gameObject, 1f); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
}