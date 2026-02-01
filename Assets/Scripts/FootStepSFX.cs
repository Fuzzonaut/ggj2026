using UnityEngine;

public class FootstepSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip footstepSound;

    public void PlayFootstep()
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(footstepSound);
    }
}
