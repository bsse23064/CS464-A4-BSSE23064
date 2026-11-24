using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PersistentMusic : MonoBehaviour
{
    static PersistentMusic instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        AudioSource src = GetComponent<AudioSource>();
        src.loop = true;
        if (!src.isPlaying && src.clip != null)
            src.Play();
    }
}