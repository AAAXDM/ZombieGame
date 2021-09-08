using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSoundsSO", menuName = "PlayerSoundsSO", order = 51)]
public class PlayerSoundsSO : ScriptableObject
{
    [SerializeField] AudioClip shot;
    [SerializeField] AudioClip hitReaction;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip getBox;

    public AudioClip Shot => shot;
    public AudioClip HitReaction => hitReaction;
    public AudioClip Death => death;
    public AudioClip GetBox => getBox;
}
