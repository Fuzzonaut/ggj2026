using UnityEngine;

public class SanityPill : MonoBehaviour
{
    public float sanityReductionAmount = 10f; // Ne kadar düşürecek?

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
        {

           InsanityManager manager = other.GetComponent<InsanityManager>();
           
           if (manager == null)
            {
                manager = FindObjectOfType<InsanityManager>();
            }

            // Scripti bulduysak azaltma işlemini yap
            if (manager != null)
            {
                manager.ReduceInsanity(sanityReductionAmount);
                Destroy(gameObject); 
            }
            else
            {
                Debug.LogWarning("InsanityManager scripti sahnede bulunamadı!");
            }
        }
    }
}