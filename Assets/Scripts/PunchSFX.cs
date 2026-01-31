using UnityEngine;

public class PunchSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip punchSound;

    [ContextMenu("Play Punch Sound")]
    public void PlayPunch()
    {
        audioSource.PlayOneShot(punchSound);
    }
}
