﻿using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound {

    public string name;
    public AudioClip clip;
    public float volume;
    public AudioMixerGroup audioGroup;

    [HideInInspector] public AudioSource source;
}
