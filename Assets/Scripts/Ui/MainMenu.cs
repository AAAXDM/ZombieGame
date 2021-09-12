using UnityEngine;
using UnityEngine.SceneManagement;
using ZombieFight.Interfaces.Core;

namespace ZombieFight.UI
{
    public class MainMenu : MonoBehaviour, IMainMenu
    {
        #region Fields
        SaveSystem saveSystem;
        SaveData saveData;
        HighScorePannel highScorePannel;
        #endregion

        #region Properties
        public SaveData SaveData => saveData;
        #endregion

        #region Core Methods
        private void Awake()
        {
            saveSystem = new SaveSystem();
            saveData = saveSystem.Load();
            highScorePannel = GetComponentInChildren<HighScorePannel>();
            highScorePannel.gameObject.SetActive(false);
        }
        #endregion

        #region Support Methods
        public void StartNewGame()
        {
            SceneManager.LoadScene(1);
        }

        public void GetStatistics()
        {
            highScorePannel.gameObject.SetActive(true);
        }
        public void QuitGame()
        {
            Application.Quit();
        }
        #endregion
    }
}