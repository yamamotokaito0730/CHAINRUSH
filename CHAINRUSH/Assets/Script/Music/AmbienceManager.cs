using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class AmbienceManager : MonoBehaviour
{
    public static AmbienceManager Instance { get; private set; }

    private Dictionary<string, AudioSource> activeSources = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
    }

    public void PlayAmbience(AmbienceData data, Vector3? worldPosition = null)
    {
        if (data == null || data.clip == null || activeSources.ContainsKey(data.ambienceName))
            return;

        GameObject go = new GameObject("Ambience_" + data.ambienceName);
        if (worldPosition != null)
            go.transform.position = worldPosition.Value;
        else
            go.transform.SetParent(transform); // 2D音なら管理オブジェクト内

        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = data.clip;
        src.volume = data.volume;
        src.loop = data.loop;
        src.spatialBlend = data.spatialized ? 1f : 0f;
        src.minDistance = data.minDistance;
        src.maxDistance = data.maxDistance;
        src.Play();

        activeSources[data.ambienceName] = src;
    }

    public void StopAmbience(string name)
    {
        if (activeSources.TryGetValue(name, out var src))
        {
            src.Stop();
            Destroy(src.gameObject);
            activeSources.Remove(name);
        }
    }

    public void StopAllAmbience()
    {
        foreach (var src in activeSources.Values)
        {
            if (src != null) Destroy(src.gameObject);
        }
        activeSources.Clear();
    }

    public bool IsPlaying(string name)
    {
        return activeSources.ContainsKey(name) && activeSources[name].isPlaying;
    }

    public void SetVolume(string name, float volume)
    {
        if (activeSources.TryGetValue(name, out var src))
        {
            src.volume = volume;
        }
    }

}
