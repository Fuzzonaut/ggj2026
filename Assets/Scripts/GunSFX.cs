using UnityEngine;

public class GunSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip gunShotSound;

    public void PlayGunShot()
    {
        audioSource.PlayOneShot(gunShotSound);
    }
}
