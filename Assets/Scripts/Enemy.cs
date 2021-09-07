using UnityEngine;

[System.Serializable]
public class Test : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] int startHealth;
    [SerializeField] int instantiateLevel;
    [SerializeField] int amount;
    [SerializeField] float zombieSpeed;
}
