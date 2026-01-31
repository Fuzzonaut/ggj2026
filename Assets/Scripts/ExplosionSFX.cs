using UnityEngine;

public class ExplosionSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip explosionSound;

    public void PlayExplosion()
    {
        audioSource.PlayOneShot(explosionSound);
    }
}
