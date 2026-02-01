using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public int scoreValue = 10; 

    // YENİ: Animator bağlantısı
    private Animator anim;
    
    // YENİ: Düşman zaten öldü mü? (Üst üste ölmemesi için)
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>(); // Animator'ı otomatik bul
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Zaten öldüyse hasar alma

        currentHealth -= damage;

        // Opsiyonel: Hasar alma animasyonu (Hurt) varsa buraya ekleyebilirsin
        // if(anim != null) anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // 1. Skoru ekle
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(scoreValue);
        }

        // 2. Fizikleri ve Çarpışmayı Kapat (Cesedin içinden geçebilelim)
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; 
        
        // 3. Varsa Hareket Scriptlerini kapat (Kovalamayı bıraksın)
        // Eğer ExploderEnemy scripti varsa onu kapatır:
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach(MonoBehaviour script in scripts)
        {
            // Bu script (EnemyHealth) ve Animator HARİÇ hepsini kapat
            if (script != this && script != anim)
            {
                script.enabled = false;
            }
        }

        // 4. Animasyonu Oynat
        if (anim != null)
        {
            anim.SetTrigger("Die");
            
            // 5. Animasyon bitince yok et (Örn: 1 saniye sonra)
            // İPUCU: Animasyonunun süresine göre buradaki 1f'i değiştir.
            Destroy(gameObject, 1f); 
        }
        else
        {
            // Animator yoksa direkt yok et
            Destroy(gameObject);
        }
    }
}
