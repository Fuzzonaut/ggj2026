using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; // YENİ: Sahne değişimi için gerekli
using System.Collections; // YENİ: Bekleme süresi (Coroutine) için gerekli

public class PlayerHealth2D : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Collision Damage (Touching Enemies)")]
    [SerializeField] private int collisionDamage = 10;
    [SerializeField] private float damageCooldown = 0.5f;
    private float nextDamageTime = 0f;

    [Header("UI (Real Time)")]               
    public TMP_Text healthText;

    // YENİ: Üst üste ölmemesi için kontrol değişkeni
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Update()                           
    {
        UpdateHealthUI();                  
    }

    private void UpdateHealthUI()
    {
        if (healthText == null) return;

        if (currentHealth <= 0)
            healthText.text = "HP: 0 (DEAD)";
        else
            healthText.text = "HP: " + currentHealth + " / " + maxHealth;
    }

    // Hem Patlayan Düşmanlar hem de normal çarpışma burayı kullanır
    public void TakeDamage(int amount)
    {
        if (isDead) return; // YENİ: Ölüye hasar verilemez

        currentHealth -= amount;

        if (currentHealth < 0) currentHealth = 0;

        // Konsola yazmaya devam etsin
        // Debug.Log("Ouch! Player HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Düşmana dokunmaya devam edince hasar yeme
    void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return; // YENİ: Ölüyken hasar alma

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (Time.time >= nextDamageTime)
            {
                TakeDamage(collisionDamage);
                nextDamageTime = Time.time + damageCooldown;
            }
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("PLAYER DIED! Starting Game Over sequence...");

        // --- YOK OLMA EFEKTİ (VANISH) ---
        
        // 1. Görüntüyü kapat
        GetComponent<SpriteRenderer>().enabled = false;
        
        // 2. Fiziği kapat (Düşmanlar içinden geçsin)
        GetComponent<Collider2D>().enabled = false;
        
        // 3. Hareketi kapat
        Controller ctrl = GetComponent<Controller>();
        if (ctrl != null) ctrl.enabled = false;

        // 4. Silahları ve efektleri kapat (Player'ın altındaki her şeyi gizle)
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        // --- SAHNE DEĞİŞİMİ ---
        StartCoroutine(LoadGameOverScene());
    }

    // YENİ: Beklemeli sahne değişimi
    IEnumerator LoadGameOverScene()
    {
        // 1.5 saniye boş ekrana (veya ölüm efektine) bakmamızı sağla
        yield return new WaitForSeconds(1.5f); 

        // Sahneyi yükle
        SceneManager.LoadScene("GameOver");
    }
}