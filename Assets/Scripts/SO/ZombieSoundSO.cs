using UnityEngine;

[CreateAssetMenu(fileName = "ZombieSoundSO", menuName = "ZombieSoundSO", order = 51)]
public class ZombieSoundSO : ScriptableObject
{
    [SerializeField] AudioClip hitZombie;
    [SerializeField] AudioClip dieZombie;

    public AudioClip HitZombie => hitZombie;
    public AudioClip DieZombie => dieZombie;
}
