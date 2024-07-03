using UnityEngine.Audio;
using UnityEngine;
using System;

public class Sounds
{
    public enum SoundID
    {
        None,
        ForestMusic,
        LevelCompleted,
        GameOver,
        PlayerHurt,
        EnemyDying,
        BombShoot,
        BombExplosion,
        FreezeShoot,
        FreezeExplosion,
    }
}

[Serializable]
public class Sound
{
    public AudioMixerGroup audioMixerGroup;

    private AudioSource source;

    public Sounds.SoundID soundID;

    public AudioClip clip;

    [Range(0, 2f)]
    public float volume = 1f;
    [Range(0, 3f)]
    public float pitch = 1f;

    public bool loop = false;
    public bool playOnAwake = false;



    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
        source.playOnAwake = playOnAwake;
        source.outputAudioMixerGroup = audioMixerGroup;
    }

    public void Play()
    {
        source.Play();
    }

    public void Pause()
    {
        source.Pause();
    }

}

public class Audio_Manager : MonoBehaviour
{
    public static Audio_Manager Instance;

    [SerializeField]
    Sound[] sound;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < sound.Length; i++)
        {
            GameObject _go = new GameObject("sound_" + i + "_" + sound[i].soundID);
            _go.transform.SetParent(this.transform);
            sound[i].SetSource(_go.AddComponent<AudioSource>());
        }
    }


    public void PlaySound(Sounds.SoundID _soundID)
    {
        for (int i = 0; i < sound.Length; i++)
        {
            if (sound[i].soundID == _soundID)
            {
                sound[i].Play();
                return;
            }
        }
    }


    public void PauseSound(Sounds.SoundID _soundID)
    {
        for (int i = 0; i < sound.Length; i++)
        {
            if (sound[i].soundID == _soundID)
            {
                sound[i].Pause();
                return;
            }
        }
    }

}
