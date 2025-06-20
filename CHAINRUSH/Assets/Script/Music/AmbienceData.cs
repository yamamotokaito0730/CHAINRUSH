using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmbienceData", menuName = "Scriptable Objects/AmbienceData")]
public class AmbienceData : ScriptableObject
{
        public string ambienceName;
        public AudioClip clip;
        public float volume = 1f;
        public bool loop = true;
        public bool spatialized = false; // 3D‰¹‚©‚Ç‚¤‚©
        public float minDistance = 1f;
        public float maxDistance = 20f;
}
