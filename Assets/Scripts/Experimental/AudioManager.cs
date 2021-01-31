using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class sound
{
    public string name;
    public AudioClip clip;
    public float volume;
    public float pitch;
    private AudioSource source;
    public void setsource (AudioSource _source)
    {
        source = _source;
        source.clip = clip;
    }
    public void play ()
    {
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
    }
}
public class AudioManager
{
    [SerializeField]
    sound[] sound;
}
