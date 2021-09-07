using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ZombieFight.Interfaces.Core;
using UnityEngine.SceneManagement;
public class GameOverPannel : MonoBehaviour,IGameOverPannel
{
    #region Fields
    Image image;
    Text imageText;
    [SerializeField] Color imageColor;
    [SerializeField] Color textColor;
    float transparencyStep = 0.05f;
    float imageTransparency;
    int maxTransperency = 1;
    #endregion

    #region Events
    public event VoidDelegate EndGame;
    #endregion

    #region Core Methods
    private void Awake()
    {
        image = GetComponent<Image>();
        imageTransparency = 0;
        image.color = imageColor;
        imageText = GetComponentInChildren<Text>();
        imageText.color = textColor;
    }

    private void Start() => FadeIn();
    #endregion

    #region Support Methods
   private  void FadeIn() => StartCoroutine(FadeINRoutine());

    IEnumerator FadeINRoutine()
    {
        float stepTime = 0.05f;
        while (imageTransparency < maxTransperency)
        {
            imageTransparency += transparencyStep;
            imageColor.a = imageTransparency;
            image.color = imageColor;
            yield return new WaitForSecondsRealtime(stepTime);
        }
        imageTransparency = 0;
        EndGame();
        StartCoroutine(FadeInImageRoutine());
        yield return null;
    }

    IEnumerator FadeInImageRoutine()
    {
        float stepTime = 0.1f;
        float staticTime = 2;
        while (imageTransparency < maxTransperency)
        {
            imageTransparency += transparencyStep;
            textColor.a = imageTransparency;
            imageText.color = textColor;
            yield return new WaitForSecondsRealtime(stepTime);
        }
        yield return new WaitForSecondsRealtime(staticTime);
        SceneManager.LoadScene(0);
    }
    #endregion
}
