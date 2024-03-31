using UnityEngine.Audio;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound Data", menuName = "ScriptableObjects/New Sound", order = 0)]
[System.Serializable]
public class Sound : ScriptableObject{

    public string soundName;

    public SoundType soundType;

    public AudioClip audioFile;

    [Range(0f, 2f)]
    public float volume = 1;

    [Range(0f, 2f)]
    public float basePitch = 1;

    [Range(0f, 2f)]
    public float pitchRange = 0.2f;

    public bool loop = false;
}

public enum SoundType
{
   Explosion,
   Thrust,
   Discharge,
   Splat

}


