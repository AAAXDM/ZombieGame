using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ZombieFight.Interfaces.Core;

[RequireComponent(typeof(AudioSource))]
public class LevelPannel : MonoBehaviour, IlevelPannel
{
    #region Fields
    [SerializeField] Color textColor;
    [SerializeField] BackgroundSoundsSO backgroundSoundsSO;
    AudioSource backgroundSounds;
    Text levelText;
    Image image;
    Color imageColor;
    float textTransparency = 0;
    float imageTransparency = 1;
    float waitTime = 0.05f;
    bool isActive;
    #endregion

    #region Properties
    public event VoidDelegate ChangeLevel;
    IZombieFightClass Core;
    public bool IsActive => isActive;
    #endregion

    #region Core Methods
    void Start()
    {
        isActive = true;
        Core = GameObject.Find("GameManager").GetComponent<ZombieFightClass>();
        levelText = GetComponentInChildren<Text>();
        backgroundSounds = GetComponent<AudioSource>();
        levelText.color = textColor;
        image = GetComponent<Image>();
        imageColor = image.color;
    }

    void OnDisable()
    {
        imageTransparency = 1;
        imageColor.a = imageTransparency;
    }
    #endregion

    #region Support Methods
    public void ShowLevelPanel()
    {
        levelText.text = "level " + Core.LevelNumber.ToString();
        image.color = imageColor;
        backgroundSounds.PlayOneShot(backgroundSoundsSO.ChangeLevelMusic);
        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        float transparencyStep = 0.15f;
        while (imageTransparency < 1)
        {
            imageTransparency += transparencyStep;
            imageColor.a = imageTransparency;
            image.color = imageColor;
            yield return new WaitForSeconds(waitTime);
        }
        StartCoroutine(FadeInTextRoutine());
    }
    IEnumerator FadeInTextRoutine()
    {
        float transparencyStep = 0.01f;
        float intensityChanger = 0.3f;
        int maxTransperency = 1;
        float staticTime = 1;
        while (textTransparency < maxTransperency)
        {
            if (textTransparency < intensityChanger)
            {
                textTransparency += transparencyStep;
            }
            else
            {
                transparencyStep = 0.08f;
                textTransparency += transparencyStep; 
            }
            textColor.a = textTransparency;
            levelText.color = textColor;
            yield return new  WaitForSecondsRealtime(waitTime);
        }
        yield return new WaitForSecondsRealtime(staticTime);
        StartCoroutine(FadeOutTextRoutine());
    }

    IEnumerator FadeOutTextRoutine()
    {
        float transparencyStep = 0.04f;
        while (textTransparency > 0)
        {
            textTransparency -= transparencyStep;
            textColor.a = textTransparency;
            levelText.color = textColor;
            yield return new WaitForSecondsRealtime(waitTime);
        }
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        float transparencyStep = 0.15f;
        ChangeLevel();
        backgroundSounds.Stop();
        while (imageTransparency > 0)
        {
            imageTransparency -= transparencyStep;
            imageColor.a = imageTransparency;
            image.color = imageColor;
            yield return new WaitForSecondsRealtime(waitTime);
        }
       isActive = false;
    }
    #endregion
}
