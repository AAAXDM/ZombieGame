using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieSO", menuName = "ZombieSO", order = 51)]
public class ZombieSO : ScriptableObject
{

    [SerializeField] List<Enemy> enemies;

    public List<Enemy> Enemies => enemies;

    public int[] GetEnemiesType(int[] enemiesAmount, int enemiesCount)
    {
        int[] enemiesType = new int[enemiesCount];
        int[] enemiesAmountCopy = new int[enemiesAmount.Length];
        Array.Copy(enemiesAmount, enemiesAmountCopy, enemiesAmount.Length);
        for (int i = 0; i < enemiesType.Length; i++)
        {
            while (enemiesAmountCopy[enemiesType[i]] == 0)
            {
                enemiesType[i] = UnityEngine.Random.Range(0, enemiesAmountCopy.Length);
            }

            enemiesAmountCopy[enemiesType[i]]--;
        }
        return enemiesType;
    }

    public int[] GetInstantiateLevels()
    {
        int[] instatiateLevels = new int[enemies.Count];
        for(int i = 0;i< enemies.Count;i++)
        {
            instatiateLevels[i] = enemies[i].InstantiateLevel;
        }
        return instatiateLevels;
    }
    public Enemy GetEnemy(int enemyType)
    {
        int scoreCoefficient = 1;
        enemies[enemyType].SetScorePrice(enemyType + scoreCoefficient);
        return enemies[enemyType];
    }

    public int[] GetAmountsCount()
    {
        int[] amounts = new int[enemies.Count];
        for(int i = 0; i < enemies.Count; i++)
        {
            amounts[i] = enemies[i].Amount;
        }
        return amounts;
    }

    public int GetStartEnemiesCount()
    {
        return Enemies[0].Amount;
    }
}

[System.Serializable]
public class Enemy
{

    #region Fields
    [SerializeField] GameObject enemy;
    [SerializeField] int health;
    [SerializeField] int instantiateLevel;
    [SerializeField] int amount;
    [SerializeField] float speed;
    [SerializeField] float damage;
    int scorePrice;
    #endregion

    #region Properties
    public GameObject Zombie => enemy;
    public int Health => health;
    public int InstantiateLevel => instantiateLevel;
    public int Amount => amount;
    public float Speed => speed;
    public float Damage => damage;
    public int ScorePrice => scorePrice;
    #endregion

    #region Support Methods
    public void SetScorePrice(int price)
    {
        scorePrice = price;
    }
    #endregion
}
