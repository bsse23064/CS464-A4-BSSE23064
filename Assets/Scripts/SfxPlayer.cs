using UnityEngine;
using UnityEngine.Audio;

public class SfxPlayer : MonoBehaviour
{
    public static SfxPlayer Instance { get; private set; }

    [SerializeField] AudioMixerGroup sfxGroup;
    [SerializeField] int poolSize = 4;
    [SerializeField] float semitoneRange = 2f;

    AudioSource[] sources;
    int index;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sources = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.outputAudioMixerGroup = sfxGroup;
            sources[i] = src;
        }
    }

    public void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        AudioSource src = sources[index];
        index = (index + 1) % sources.Length;

        float semitones = Random.Range(-semitoneRange, semitoneRange);
        src.pitch = Mathf.Pow(2f, semitones / 12f);
        src.PlayOneShot(clip, volume);
    }
}