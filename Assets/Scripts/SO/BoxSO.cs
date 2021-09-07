using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoxSO", menuName = "BoxSO", order = 52)]
public class BoxSO : ScriptableObject
{
    [SerializeField] List<Box> boxes;
    public Box GetBox()
    {
        int chance = 5;
        int randomizer = Random.Range(0, chance);
        if (randomizer > 0) return boxes[0];
        else return boxes[1];
    }
}

[System.Serializable]
public class Box
{
    #region Fields
    public enum BoxType { armor, medicine };
    [SerializeField] private BoxType type;
    [SerializeField] private GameObject box;
    [SerializeField] private int additionMin;
    [SerializeField] private int additionMax;
    #endregion

    #region Properties
    public GameObject BoxObject => box;
    public BoxType Type => type;
    public int AdditionMin => additionMin;
    public int AdditionMax => additionMax;
    #endregion
}
