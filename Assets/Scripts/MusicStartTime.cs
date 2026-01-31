using UnityEngine;

public class MusicStartTime : MonoBehaviour
{
    public AudioSource audioSource;
    public float startTimeInSeconds = 110f; // buradan baþlasýn

    void Start()
    {
        audioSource.time = startTimeInSeconds;
        audioSource.Play();
    }
}
