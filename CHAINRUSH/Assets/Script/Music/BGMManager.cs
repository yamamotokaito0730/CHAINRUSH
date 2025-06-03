using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public AudioData audioData;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
    }

    public void Play(string name)
    {
        AudioClip clip = audioData.GetBGM(name);
        if (clip != null && clip != audioSource.clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

    }

    public void Stop() => audioSource.Stop();


}
