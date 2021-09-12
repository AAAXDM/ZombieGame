using UnityEngine;
using ZombieFight.Interfaces.Core;
using ZombieFight.UI;

namespace ZombieFight
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerSteps : MonoBehaviour
    {
        AudioSource step;
        IlevelPannel LevelPannelInt;
        IZombieFightClass CoreClass;

        private void Start()
        {
            step = GetComponent<AudioSource>();
            CoreClass = GameObject.Find("GameManager").GetComponent<ZombieFightClass>();
            LevelPannelInt = GameObject.Find("Level").GetComponent<LevelPannel>();
            LevelPannelInt.ChangeLevel += UnMuteSound;
            CoreClass.StopLevel += MuteSound;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!step.isPlaying) step.Play();
        }

        private void OnDestroy()
        {
            CoreClass.StopLevel -= MuteSound;
            LevelPannelInt.ChangeLevel -= UnMuteSound;
        }

        private void MuteSound()
        {
            step.mute = true;
        }

        private void UnMuteSound()
        {
            step.mute = false;
        }
    }
}