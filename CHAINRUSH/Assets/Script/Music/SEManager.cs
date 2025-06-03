using UnityEngine;

public class SEManager : MonoBehaviour
{
    public AudioData audioData;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Play(string name)
    {
        AudioClip clip = audioData.GetSE(name);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
