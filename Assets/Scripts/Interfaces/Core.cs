using UnityEngine;

namespace ZombieFight.Interfaces.Core 
{
    public delegate void VoidDelegate();

    public interface IBounds
    {
        #region Properties
        (float,float) XRange { get; }
        (float, float) ZRange { get; }
        #endregion
    }

    public interface IScreenBounds
    {
        public (float, float) PlayerCoordinates { get; }
        public Bounds Bounds { get; }
    }

    public interface IPlayerController 
    {
        GameObject Player { get; }
        Transform PlayerTransform { get; }

        event VoidDelegate Death;
        event VoidDelegate Hit;
    }

    public interface IColliderOpener
    {
        bool IsOpen { get; }
        void OpenCollider();

        void CloseCollider();
    }

    public interface IZombieFightClass 
    {
        int LevelNumber { get; }
        IGameOverPannel GameOver { get; }
        void DecreaseStats(float statChange, ZombieFightClass.UIStats type);
        void IncreaseScore(int value);
        void DeleteEnemyFromList(GameObject enemy);
    }

    public interface IGameOverPannel
    {
        event VoidDelegate EndGame;
    }

    public interface IlevelPannel
    {
        event VoidDelegate ChangeLevel;
        bool IsActive { get; }
        void  ShowLevelPanel();
    }

    public interface IMainMenu
    {
        public SaveData SaveData { get; }
    }
}
