using UnityEngine;

public class PlaySE : MonoBehaviour
{
    public AudioClip Clip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.PlayOneShot(Clip);
    }

}
