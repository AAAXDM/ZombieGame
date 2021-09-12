using UnityEngine;
using ZombieFight.Interfaces.Core;

namespace ZombieFight.ScreenBounds
{
    [RequireComponent(typeof(BoxCollider))]
    public class GameScreenBounds : MonoBehaviour, IBounds, IScreenBounds
    {
        #region Fields
        Camera cam;
        BoxCollider boxCollider;
        float zScale = 26;
        (float, float) xRange;
        (float, float) zRange;
        (float, float) playerCoordinates;
        #endregion

        #region Properties
        public (float, float) XRange => xRange;
        public (float, float) ZRange => zRange;
        public (float, float) PlayerCoordinates => playerCoordinates;
        public Bounds Bounds => boxCollider.bounds;
        #endregion


        #region Standart Methods
        private void Awake()
        {
            cam = Camera.main;
            if (!cam.orthographic)
            {
                Debug.LogError("ScaleToCamera:Start() - Camera.main needs to be orthographic ");
            }
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.size = Vector3.one;
            transform.position = cam.transform.position;
            boxCollider.size = CalculateScale();
            CalculateBounds();
            CalculatePlaterPos();
        }

        #endregion

        #region Support Methods
        private Vector3 CalculateScale()
        {
            Vector3 colliderScale;
            colliderScale.z = zScale;
            colliderScale.y = cam.transform.position.y * 2;
            colliderScale.x = zScale * cam.aspect;
            return colliderScale;
        }

        private void CalculateBounds()
        {
            xRange.Item1 = transform.position.x - boxCollider.size.x / 2;
            xRange.Item2 = transform.position.x + boxCollider.size.x / 2;
            zRange.Item1 = transform.position.z - boxCollider.size.z / 2;
            zRange.Item2 = transform.position.z + boxCollider.size.z / 2;
        }

        private void CalculatePlaterPos()
        {
            playerCoordinates.Item1 =xRange.Item1 + (xRange.Item2 - xRange.Item1)/2;
            playerCoordinates.Item2 =zRange.Item1 + (zRange.Item2 - zRange.Item1)/2;
        }
        #endregion
    }
}
