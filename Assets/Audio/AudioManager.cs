using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField]
    private Sound[] sounds;

    private List<List<Sound>> sortedSounds;

    public static AudioManager Instance;
    
    //unity can play 32 sounds at once so total pool size should be less than 33
    
    [SerializeField]
    private int PoolSizeLowPriority = 5;
    [SerializeField]
    private int PoolSizeMediumPriority = 10;
    [SerializeField]
    private int PoolSizeHighPriority = 15;

    private Queue<AudioSource> sourcePoolLow;
    private Queue<AudioSource> sourcePoolMedium;
    private Queue<AudioSource> sourcePoolHigh;

    [SerializeField]
    private AudioSource musicSource;

    [Header("Master Audio Settings")]
    [SerializeField]
    [Range(0f,1f)]
    private float volume;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
        if(TryGetComponent<AudioSource>(out AudioSource foundSource))
        {
            musicSource = foundSource;
            musicSource.volume = volume;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnValidate()
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        if(musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    public static AudioManager GetInstance()
    {
        if(Instance == null)
        {
            Instance = new AudioManager();
        }

        return Instance;
    }

    internal float GetVolume()
    {
        return volume;
    }

    private void Start()
    {
        InitilizeAudioSources();
        SortSounds();

        
    }

    private void InitilizeAudioSources()
    {
        sourcePoolLow = CreatePoolQueue(PoolSizeLowPriority);
        sourcePoolMedium = CreatePoolQueue(PoolSizeMediumPriority);
        sourcePoolHigh = CreatePoolQueue(PoolSizeHighPriority);


    }

    private Queue<AudioSource> CreatePoolQueue(int size)
    {
        Queue<AudioSource> newPool = new Queue<AudioSource>();
        for (int i = 0; i < size; i++)
        {
            AudioSource source = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

            gameObject.AddComponent(typeof(AudioSource));

            newPool.Enqueue(source);
        }
        return newPool;
    }

    public void PlaySound(SoundType soundType)
    {
        Queue<AudioSource> targetPool = sourcePoolMedium;

        if (targetPool.Count != 0)
        {
            Sound sound = GetRandomSound(soundType);

            if (sound != null)
            {
                AudioSource source = PrepareAudioSource(targetPool.Dequeue(), sound);

                source.Play();

                StartCoroutine(EnqueueSource(source, sound.audioFile.length, targetPool));


            }
        }
    }


    public void PlaySound(SoundType soundType, SoundPriority priority)
    {
        Queue<AudioSource> targetPool;
        switch (priority)
        {
            case SoundPriority.Low:
                targetPool = sourcePoolLow;
                break;
            case SoundPriority.Medium:
                targetPool = sourcePoolMedium;
                break;
            case SoundPriority.High:
                targetPool = sourcePoolHigh;
                break;
            default:
                targetPool = sourcePoolMedium;
                break;
        }


        if (targetPool.Count != 0)
        {
            Sound sound = GetRandomSound(soundType);

            if (sound != null)
            {
                AudioSource source = PrepareAudioSource(targetPool.Dequeue(), sound);

                source.Play();

                StartCoroutine(EnqueueSource(source, sound.audioFile.length, targetPool));
            }
        }
    }

    private Sound GetRandomSound(SoundType soundType)
    {
        Sound returnValue;

        int soundTypeLength = sortedSounds[(int)soundType].Count;

        if (soundTypeLength > 0)
        {
            int random = Random.Range(0, soundTypeLength);

            returnValue = sortedSounds[(int)soundType][random];            
        }
        else
        {
            returnValue = null;
        }

        return returnValue;
    }

    private AudioSource PrepareAudioSource(AudioSource source,Sound sound)
    {

        source.volume = sound.volume * volume;
        source.pitch = sound.basePitch + Random.Range(-sound.pitchRange, sound.pitchRange);
        source.clip = sound.audioFile;

        return source;
    }

    private IEnumerator EnqueueSource(AudioSource source, float delayTime, Queue<AudioSource> targetQueue)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        targetQueue.Enqueue(source);        
    }

    private void SortSounds()
    {
        sortedSounds = new List<List<Sound>>();
        for (int i = 0; i < System.Enum.GetNames(typeof(SoundType)).Length; i++)
        {
            sortedSounds.Add(new List<Sound>());
        }

        foreach (Sound sound in sounds)
        {
            sortedSounds[(int)sound.soundType].Add(sound);
        }

        /*
        foreach (List<Sound> l in sortedSounds)
        {
            print(sortedSounds);
        }
        */
    }

}

public enum SoundPriority
{
    Low,
    Medium,
    High
}
