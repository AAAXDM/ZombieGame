using UnityEngine;
using UnityEngine.UI;
using ZombieFight.Interfaces.Core;
using TMPro;

public class HighScorePannel : MonoBehaviour
{
    TextMeshPro hiScore;
    Text recordLevel;
    IMainMenu mainMenu;
    
    private void Awake()
    {
        Transform statPannel = transform.Find("HighScore");
        hiScore = statPannel.GetComponent<TextMeshPro>();
        mainMenu = GetComponentInParent<MainMenu>();
        hiScore.text = "HIGH SCORE" + "\n" + mainMenu.SaveData.hiScore;
        recordLevel.text = "RECORD LEVEL" + "\n" + mainMenu.SaveData.maxLevel;
    }
    public void ExitPannel()
    {
        gameObject.SetActive(false);
    }
}
