using UnityEngine;
using ZombieFight.Interfaces.Core;
using TMPro;

public class HighScorePannel : MonoBehaviour
{
    TextMeshProUGUI hiScore;
    TextMeshProUGUI recordLevel;
    IMainMenu mainMenu;
    
    private void Start()
    {
        GameObject hiScoreTransform = GameObject.Find("HighScore");
        GameObject recordLevelTransform = GameObject.Find("RecordLevel");
        hiScore = hiScoreTransform.GetComponent<TextMeshProUGUI>();
        recordLevel = recordLevelTransform.GetComponent<TextMeshProUGUI>();
        mainMenu = GetComponentInParent<MainMenu>();
        if (mainMenu.SaveData != null)
        {
            hiScore.text = "HIGH SCORE" + "\n" + "\n" + mainMenu.SaveData.hiScore;
            recordLevel.text = "RECORD LEVEL" + "\n" + "\n" + mainMenu.SaveData.maxLevel;
        }
    }
    public void ExitPannel()
    {
        gameObject.SetActive(false);
    }
}
