using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClip clipPlace;
    [SerializeField] private AudioClip clipBreak;
    [SerializeField] private AudioClip clipEarthquake;
    [SerializeField] private AudioClip clipWin;
    [SerializeField] private AudioClip clipLose;

    private AudioSource _source;

    void Awake()
    {
        Instance = this;
        _source  = GetComponent<AudioSource>();
        if (_source == null) _source = gameObject.AddComponent<AudioSource>();
    }

    public void PlayPlace()      => Play(clipPlace,      0.6f);
    public void PlayBreak()      => Play(clipBreak,      0.5f);
    public void PlayEarthquake() => Play(clipEarthquake, 0.8f);
    public void PlayWin()        => Play(clipWin,        1.0f);
    public void PlayLose()       => Play(clipLose,       1.0f);

    void Play(AudioClip clip, float volume)
    {
        if (clip != null) _source.PlayOneShot(clip, volume);
    }
}
