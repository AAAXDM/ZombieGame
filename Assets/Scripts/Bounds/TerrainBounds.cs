using UnityEngine;
using ZombieFight.Interfaces.Core;

namespace ZombieFight.ScreenBounds
{
    [RequireComponent(typeof(Terrain))]
    public class TerrainBounds : MonoBehaviour, IBounds
    {
        #region Fields
        Terrain terrain;
        (float, float) xTerrainBounds;
        (float, float) zTerrainBounds;
        float frame = 2;
        float width;
        float length;
        #endregion

        #region Properties
        public (float, float) XRange => xTerrainBounds;
        public (float, float) ZRange => zTerrainBounds;
        #endregion

        #region Core Methods
        void Awake()
        {
            terrain = GetComponent<Terrain>();
            width = terrain.terrainData.size.x;
            length = terrain.terrainData.size.z;
            CalculateBounds();
        }
        #endregion

        #region Support Methods
        void CalculateBounds()
        {
            xTerrainBounds.Item1 = transform.position.x + frame;
            xTerrainBounds.Item2 = transform.position.x + width - frame;
            zTerrainBounds.Item1 = transform.position.z + frame;
            zTerrainBounds.Item2 = transform.position.z + length - frame;
        }
        #endregion
    }
}
