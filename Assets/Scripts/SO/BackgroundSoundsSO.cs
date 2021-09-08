using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BackgroundSoundsSO", menuName = "BackgroundSoundsSO", order = 51)]
public class BackgroundSoundsSO : ScriptableObject
{
    [SerializeField] AudioClip rain;
    [SerializeField] AudioClip backgroundMusic;
    [SerializeField] AudioClip changeLevelMusic;
    [SerializeField] AudioClip endGame;

    public AudioClip Rain => rain;
    public AudioClip BackgroundMusic => backgroundMusic;
    public AudioClip ChangeLevelMusic => changeLevelMusic;
    public AudioClip EndGame => endGame;
}
