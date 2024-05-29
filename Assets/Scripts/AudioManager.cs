using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource BGMSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip")]
    public AudioClip BGM;
    public AudioClip hit;
    public AudioClip block;
    public AudioClip contact;
    public AudioClip swing;

    public void Start()
    {
        BGMSource.clip = BGM;
        BGMSource.Play();
    }
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
