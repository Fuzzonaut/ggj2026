using UnityEngine;

public class ExplosionSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip explosionClip;

    public void PlayExplosion()
    {
        if (audioSource != null && explosionClip != null)
        {
            audioSource.PlayOneShot(explosionClip);
        }
    }
}