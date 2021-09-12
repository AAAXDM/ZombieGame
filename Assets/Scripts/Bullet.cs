using UnityEngine;

namespace ZombieFight
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        #region Fields
        Rigidbody rb;
        [SerializeField] float bulletSpeed;
        int destroyTime = 2;
        int damage = 5;
        #endregion

        #region Properties
        public int Damage => damage;
        static private Transform _Bullets;
        static Transform Bullets
        {
            get
            {
                if (_Bullets == null)
                {
                    GameObject go = new GameObject("Bullets");
                    _Bullets = go.transform;
                }
                return _Bullets;
            }
        }
        #endregion

        #region Core Methods
        void Start()
        {
            rb = GetComponent<Rigidbody>();

            transform.SetParent(Bullets, true);
            rb.velocity = transform.up * bulletSpeed;
            Invoke(nameof(DestroyBullet), destroyTime);
        }
        #endregion

        #region Support Methods
        private void DestroyBullet() => Destroy(gameObject);
        #endregion
    }
}
