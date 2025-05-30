using UnityEngine;

public class PlayBGM : MonoBehaviour
{
    public AudioClip bgmClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = bgmClip;
        source.playOnAwake = false;
        source.loop = false;
        source.Play();
    }
}
