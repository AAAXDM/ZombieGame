using UnityEngine;
using ZombieFight.Interfaces.Core;

namespace ZombieFight.ScreenBounds
{
    public class SpawnBounds : MonoBehaviour
    {
        #region Fields
        (float, float) randomXLoc;
        (float, float) randomZLoc;
        float randomX;
        float randomZ;
        float spawnBound = 3;
        Vector3[] spawnLocations = new Vector3[4];
        #endregion

        #region Properties
        IBounds ScreenBounds;
        IBounds TerrainBounds;
        #endregion

        #region Singltone
        private static SpawnBounds _S;
        public static SpawnBounds S
        {
            get
            {
                return _S;
            }
            set
            {
                if (S != null)
                    Debug.Log("You have set S before");
                _S = value;
            }
        }

        #endregion

        #region Core Methods
        void Awake()
        {
            S = this;
        }
        private void Start()
        {
            ScreenBounds = Camera.main.gameObject.GetComponentInChildren<GameScreenBounds>();
            TerrainBounds = GameObject.Find("Terrain").GetComponent<TerrainBounds>();
        }
        #endregion

        #region Support Methods
        void RandomizeSpawnLocation()
        {
            randomXLoc.Item1 = Random.Range(TerrainBounds.XRange.Item1, ScreenBounds.XRange.Item1 - spawnBound);
            randomXLoc.Item2 = Random.Range(ScreenBounds.XRange.Item2 + spawnBound, TerrainBounds.XRange.Item2);
            randomZLoc.Item1 = Random.Range(TerrainBounds.ZRange.Item1, ScreenBounds.ZRange.Item1 - spawnBound);
            randomZLoc.Item2 = Random.Range(ScreenBounds.ZRange.Item2 + spawnBound, TerrainBounds.ZRange.Item2);
            randomX = Random.Range(TerrainBounds.XRange.Item1, TerrainBounds.XRange.Item2);
            randomZ = Random.Range(ScreenBounds.ZRange.Item1, ScreenBounds.ZRange.Item2);
        }

        public Vector3 ChoseSpawnLocation()
        {
            RandomizeSpawnLocation();
            spawnLocations[0] = new Vector3(randomXLoc.Item1, 1, randomZ);
            spawnLocations[1] = new Vector3(randomXLoc.Item2, 1, randomZ);
            spawnLocations[2] = new Vector3(randomX, 1, randomZLoc.Item1);
            spawnLocations[3] = new Vector3(randomX, 1, randomZLoc.Item2);
            return spawnLocations[Random.Range(0, spawnLocations.Length)];
        }

        #endregion
    }
}
