using UnityEngine;
using System.Collections;

public class ExploderEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3.5f; // Koşma hızı

    [Header("Explosion Settings")]
    public float detectionRange = 1.5f; // Ne kadar yaklaşınca patlasın?
    public float explosionDelay = 0.5f; // "Boom" demeden önceki bekleme süresi
    public int damage = 40;
    public float explosionRadius = 2.5f; 

    [Header("References")]
    public Transform player; 
    private Animator anim;
    private Rigidbody2D rb; // Fizik motoru referansı
    private bool hasExploded = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        // Eğer patladıysa veya player yoksa hiçbir şey yapma
        if (hasExploded || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            // --- 1. KOVALAMA KISMI ---
            ChasePlayer();
        }
        else
        {
            // --- 2. PATLAMA KISMI ---
            // Menzile girdi! Hareketi kes ve patla.
            StartCoroutine(ExplodeSequence());
        }
    }

    void ChasePlayer()
    {
        // Yönü bul (Player nerede?)
        Vector2 direction = (player.position - transform.position).normalized;
        
        // O yöne doğru git
        rb.linearVelocity = direction * speed;

        // YENİ: Düşmanın yüzünü döndür (Sağa mı sola mı bakıyor?)
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Sağa bak
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Sola bak
        }
        
        // Eğer koşma animasyonun varsa burada tetikleyebilirsin
        // if(anim != null) anim.SetBool("IsRunning", true);
    }

    IEnumerator ExplodeSequence()
    {
        hasExploded = true;

        // Dur! (Kaymayı önle)
        rb.linearVelocity = Vector2.zero;
        
        // Artık fiziksel olarak itilemesin (Kinematic yapıyoruz)
        rb.bodyType = RigidbodyType2D.Kinematic; 
        GetComponent<Collider2D>().enabled = false;

        // Animasyonu Başlat
        if (anim != null) anim.SetTrigger("Explode");

        // Patlama anına kadar bekle
        yield return new WaitForSeconds(explosionDelay);

        // Hasar Ver
        DamagePlayer();

        // Yok Et
        Destroy(gameObject, 0.5f); 
    }

    void DamagePlayer()
{
    Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
    foreach (Collider2D obj in objects)
    {
        if (obj.CompareTag("Player"))
        {
            
            PlayerHealth2D health = obj.GetComponent<PlayerHealth2D>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}