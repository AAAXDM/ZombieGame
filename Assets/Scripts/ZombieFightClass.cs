using System.Collections;
using System.Collections.Generic;
using ZombieFight.Interfaces.Core;
using ZombieFight;
using UnityEngine;
using UnityEngine.UI;
using Download;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

[RequireComponent(typeof(AudioSource))]
public class ZombieFightClass : MonoBehaviour, IZombieFightClass
{
    #region Fields
    [SerializeField] GameObject playerPrefab;
    [SerializeField] ZombieSO zombieSO;
    [SerializeField] BoxSO boxS0;
    [SerializeField] BackgroundSoundsSO backgroundSoundsSO;
    List<GameObject> enemies = new List<GameObject>();
    Bounds[,] boxSpawnBounds;
    GameObject gameOverPannel;
    GameObject pausePannel;
    GameObject player;
    GameObject boxObject;
    AudioSource backgroundSounds;
    Text[] playerStats = new Text[(int)UIStats.count];
    FrostEffect cameraEffect;
    SaveSystem saveSystem;
    SaveData saveData;
    int[] enemyAmounts;
    int[] enemyTypeInLevel;
    int[] instantiateLevels;
    Vector3 direction;
    Vector3 playerPos;
    int highScore;
    int enemiesCount;
    int levelNumber = 0;
    int numberOfInstantiate = 0;
    int division = 8;
    bool canChangeLevel = true;
    bool canInstantiateBox = true;
    bool isPaused = false;
    #endregion

    #region Properties
    IScreenBounds ScreenBounds;
    IGameOverPannel GameOverPannel;
    IPlayerController PlayerControllerInt;
    IlevelPannel LevelPannel;
    public int LevelNumber => levelNumber;
    public IGameOverPannel GameOver => GameOverPannel;
    public event VoidDelegate StopLevel;
    #endregion

    public enum UIStats {health, armor, score, count}
    #region Core Methods
    private void Awake()
    {
        float startPlayerPosY = 0.3f;
        GetComponents();
        gameOverPannel.SetActive(false);
        pausePannel.SetActive(false);
        playerPos = new Vector3(ScreenBounds.PlayerCoordinates.Item1, startPlayerPosY, ScreenBounds.PlayerCoordinates.Item2);
        instantiateLevels = zombieSO.GetInstantiateLevels();
        enemyAmounts = zombieSO.GetAmountsCount();
        SetUpBoxSpawnPoints();
        SubscribeToEvents();
        saveSystem = new SaveSystem();
        saveData = new SaveData();
        saveData = saveSystem.Load();
        enemiesCount = zombieSO.GetStartEnemiesCount();
    }
    private void Start() => ChangeLevel(); 

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !LevelPannel.IsActive)
        {
            if (!isPaused)
            {
                Pause();
            }
            else
            {
                UnPause();
            }
        }
    }
    private void OnDisable()
    {
        LevelPannel.ChangeLevel -= StartLevel;
        LevelPannel.ChangeLevel -= GoToStartPos;
        LevelPannel.ChangeLevel -= SetBoxFieldsToDefault;
        PlayerControllerInt.Hit -= PlayHitEffect;
        PlayerControllerInt.Death -= EndGame;
        LevelPannel.ChangeLevel -= DestroyBox;
    }
    #endregion

    #region Support Methods
    private void StartLevel() => StartCoroutine(StartLevelRoutine());
   
    private void InstatniateZombie() => StartCoroutine(InstantiateZombieRoutine());

    private void UnPause()
    {
        Time.timeScale = 1;
        isPaused = false;
        pausePannel.SetActive(false);   
    }
    private void Pause()
    {
        Time.timeScale = 0;
        isPaused = true;
        pausePannel.SetActive(true);
    }

    public void ExitGame()
    {
        UnPause();
        SceneManager.LoadScene(0);
        gameOverPannel.SetActive(true);
    }
    private void GetComponents()
    {
        gameOverPannel = GameObject.FindWithTag("GameOver");
        GameOverPannel = gameOverPannel.GetComponent<GameOverPannel>();
        ScreenBounds = Camera.main.gameObject.GetComponentInChildren<ScreenBounds>();
        cameraEffect = Camera.main.gameObject.GetComponent<FrostEffect>();
        LevelPannel = GameObject.Find("Level").GetComponent<LevelPannel>();
        backgroundSounds = GetComponent<AudioSource>();
        pausePannel = GameObject.Find("Pause");
    }

    private void SubscribeToEvents()
    {
        LevelPannel.ChangeLevel += StartLevel;
        LevelPannel.ChangeLevel += GoToStartPos;
        LevelPannel.ChangeLevel += SetBoxFieldsToDefault;
        LevelPannel.ChangeLevel += DestroyBox;
    }
    private  void EndGame()
    {
        numberOfInstantiate = 0;
        enemies.Clear();
        saveSystem.Save(saveData);
        gameOverPannel.SetActive(true);
        StopInstantiateBox();
    }

    private void ChangeLevel()
    {
        if(levelNumber > 1) StopLevel();
        StartCoroutine(ChangeLevelRoutine());
    }

    void ChangeAmounts()
    {
        for (int i = 1; i < enemyAmounts.Length; i++)
        {
            if(enemyAmounts[i - 1] > 0 && instantiateLevels[i] <= levelNumber)
            {
                enemyAmounts[i - 1]--;
                enemyAmounts[i]++;
            }
        }
    }

    private void PlayHitEffect()
    {
        if (!cameraEffect.IsActive)
        {
            float startValue = 0;
            float endValue = 0.2f;
            cameraEffect.ChangeFrostAmount(startValue, endValue);
        }
    }
    private void PlusAmount() => enemyAmounts[0]++;
   
    private void GoToStartPos() => player.transform.position = playerPos;
    private void InstantiateHealthBox() => StartCoroutine(InstantiateBoxRoutine());

    private void StopInstantiateBox() => StopCoroutine(InstantiateBoxRoutine());
    private void SetUpBoxSpawnPoints()
    {
        Bounds bounds = ScreenBounds.Bounds;
        boxSpawnBounds = new Bounds[division, division];
        Vector3[,] boxSpawnPoints = new Vector3[division+1, division+1];
        float dX = bounds.size.x / division;
        float dZ = bounds.size.z / division;
        for (int i = 0; i <= division; i++)
        {
            for (int j = 0; j <= division; j++)
            {
                boxSpawnPoints[i, j] = new Vector3(
                    bounds.min.x + i * dX, 0,
                    bounds.min.z + j * dZ);  
            }
        }
        float boxSizeX = (boxSpawnPoints[1, 0].x - boxSpawnPoints[0, 0].x)/2;
        float boxSizeZ = (boxSpawnPoints[1, 0].x - boxSpawnPoints[0, 0].x)/2;
        Vector3 boxSize = new Vector3(boxSizeX, 2, boxSizeZ);
        for (int i = 0; i < division; i++)
        {
            for(int j = 0; j < division; j++)
            {
                float boxCenterX = boxSpawnPoints[i, j].x + (boxSpawnPoints[i + 1, j].x - boxSpawnPoints[i, j].x) / 2;
                float boxCenterZ = boxSpawnPoints[i, j].z +(boxSpawnPoints[i, j + 1].z - boxSpawnPoints[i, j].z) / 2;
                Vector3 center = new Vector3(boxCenterX, 2, boxCenterZ);
                boxSpawnBounds[i, j] = new Bounds(center, boxSize);
            }
        }
    }
    private Vector3 CalculateBoxPositon()
    {
        Vector3 position = Vector3.zero;
        bool maySpawn = false;
        while (!maySpawn)
        {
            maySpawn = true;
            int xNumber = Random.Range(0, division);
            int zNumber = Random.Range(0, division);
            if(boxSpawnBounds[xNumber, zNumber].Contains(player.transform.position))
            {
                maySpawn = false;
                continue;
            }
            for (int i = 0; i < enemies.Count;i++)
            {
                    if (boxSpawnBounds[xNumber, zNumber].Contains(enemies[i].transform.position))
                    {
                        maySpawn = false;
                        break;
                    } 
            }
            position = boxSpawnBounds[xNumber, zNumber].center;
        }
        return position;
    }

    private void InstantiateBox()
    {
        Vector3 position = CalculateBoxPositon();
        Box box = boxS0.GetBox();
        boxObject =  Instantiate(box.BoxObject, position, Quaternion.identity);
        boxObject.GetComponent<BoxObject>().SetBoxParameters(box);
    }

    private void DestroyBox() => Destroy(boxObject);
    private void SetBoxFieldsToDefault() => canInstantiateBox = true;
    private IEnumerator InstantiateBoxRoutine()
    {
        float healthBoxInstantiateTime = 5;
        int randomInstatntiateBoxChance = 5;
        while (canInstantiateBox) 
        {
            yield return new WaitForSeconds(healthBoxInstantiateTime);
            int random = Random.Range(0, randomInstatntiateBoxChance);
            if (random == 0)
            {
                InstantiateBox();
                canInstantiateBox = false;
            }
            else
            {
                randomInstatntiateBoxChance--;
                healthBoxInstantiateTime = 3;
            } 
        }
    }

    private IEnumerator StartLevelRoutine()
    {
        float startRespawnTime = 2;
        enemyTypeInLevel = new int[enemiesCount];
        if (levelNumber == 1)
        {
            player = Instantiate(playerPrefab, playerPos, Quaternion.identity);
            PlayerControllerInt = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }
        backgroundSounds.PlayOneShot(backgroundSoundsSO.Rain);
        backgroundSounds.PlayOneShot(backgroundSoundsSO.BackgroundMusic);
        enemyTypeInLevel = zombieSO.GetEnemiesType(enemyAmounts, enemiesCount);
        yield return new WaitForSecondsRealtime(startRespawnTime);
        InstantiateHealthBox();
        InstatniateZombie();
        PlayerControllerInt.Hit += PlayHitEffect;
        PlayerControllerInt.Death += EndGame;
        canChangeLevel = true;
    }

    private IEnumerator InstantiateZombieRoutine()
    {
        float respawnTime = 3;
        while (enemies.Count < enemiesCount)
        {
            yield return new WaitWhile(() => isPaused);

            direction = SpawnBounds.S.ChoseSpawnLocation();
            Enemy enemy = zombieSO.GetEnemy(enemyTypeInLevel[numberOfInstantiate]);
            numberOfInstantiate++;
            GameObject enemyObj = Instantiate(enemy.Zombie, direction, Quaternion.identity);
            enemyObj.GetComponent<Zombie>().SetEnemyStats(enemy);
            enemies.Add(enemyObj);

            yield return new WaitWhile(() => isPaused);
            yield return new WaitForSecondsRealtime(respawnTime);
        }
    }

    private IEnumerator ChangeLevelRoutine()
    {
        float await = 2;
        if (highScore > saveData.hiScore)
        {
            saveData.hiScore = highScore;
        }
        if (levelNumber > saveData.maxLevel)
        {
            saveData.maxLevel = levelNumber;
        }
        yield return new  WaitForSecondsRealtime(await);
        StopInstantiateBox();
        int plusEnemies = 2;
        numberOfInstantiate = 0;
        levelNumber++;
        if (LevelNumber > 1)
        {
            ChangeAmounts();
        }
        if (levelNumber % plusEnemies == 0)
        {
            enemiesCount++;
            PlusAmount();
        }
        LevelPannel.ShowLevelPanel();
        backgroundSounds.Stop();
    }
    public void IncreaseScore(int value)
    {
        highScore += value;
        DecreaseStats(highScore, UIStats.score);
    }
    public void DecreaseStats(float statChange, UIStats type)
    {
        Text stat = playerStats[(int)type];
        if(stat == null)
        {
            GameObject go = GameObject.Find(type.ToString());
            if(go != null)
            {
                stat = go.GetComponent<Text>();
            }
            else
            {
                Debug.LogError(("ZombieFightClass:DecreaseStats() - Could not find a GameObject named {0}.", type));
                return;
            }
        }
        stat.text = statChange.ToString();
    }
   
    public void DeleteEnemyFromList(GameObject enemy)
    {
        enemies.Remove(enemy);
        if (enemies.Count == 0 && canChangeLevel)
        {
            ChangeLevel();
            canChangeLevel = false;
        }
    }
    #endregion
}
