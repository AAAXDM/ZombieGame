using ZombieFight.Interfaces.Core;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using ZombieFight.UI;

namespace ZombieFight
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class Zombie : MonoBehaviour
    {
        #region Fields
        [SerializeField] GameObject blood;
        [SerializeField] ZombieSoundSO zombieSoundSO;
        AudioSource zombieSound;
        NavMeshAgent motion;
        Animator zombieAnimation;
        Transform playerPos;
        RaycastHit[] hits = new RaycastHit[6];
        Vector3 slowDying = new Vector3(0, 0.005f, 0);
        string animationName = "Attack";
        float speed;
        float zombieSpeed;
        float attackDistance = 11f;
        float hitAnimationTime = 1.5f;
        float destroyPosition = -3.2f;
        float damage;
        int health;
        int startHealth;
        int instantiateLevel;
        int scorePrice;
        bool isShot = false;
        bool isDie = false;
        #endregion

        #region Properties
        IPlayerController PlayerController;
        IColliderOpener ColliderOpener;
        IZombieFightClass Core;
        IlevelPannel LevelPanel;
        public int StartHealth { get => startHealth; set => startHealth = value; }
        public float Damage => damage;
        #endregion

        #region Core Methods
        private void Start()
        {
            Core = FindObjectOfType<ZombieFightClass>();
            PlayerController = FindObjectOfType<PlayerController>();
            LevelPanel = FindObjectOfType<LevelPannel>();
            GetComponents();
            speed = zombieSpeed;
            motion.speed = speed;
            motion.updateRotation = true;
            playerPos = PlayerController.PlayerTransform;
            Core.GameOver.EndGame += DestroyZombie;
            LevelPanel.ChangeLevel += DestroyZombie;
            health = startHealth + Core.LevelNumber - instantiateLevel;
        }

        private void FixedUpdate()
        {
            if (!motion.enabled) return;
            Vector3 delta = playerPos.position - transform.position;
            if (delta.sqrMagnitude > attackDistance && !isShot)
            {
                if (zombieAnimation.GetBool("isAttak")) zombieAnimation.SetBool("isAttak", false);
                if (IsAttackAnimationEnd())
                {
                    GoToPlayer(playerPos.position);
                }
            }
            if (delta.sqrMagnitude <= attackDistance && !isShot)
            {
                if (!IsLookOnPlayer()) LookOnPlayer();
                StopZombie();
            }
            if (isDie) ColliderOpener.CloseCollider();
        }

        private void OnCollisionEnter(Collision collision)
        {
            int bulletLayer = 13;
            GameObject coll = collision.gameObject;
            if (coll.layer == bulletLayer && !isDie)
            {
                health -= coll.gameObject.GetComponent<Bullet>().Damage;
                ContactPoint[] contacts = collision.contacts;
                Vector3 hitPoint = contacts[0].point;
                Destroy(coll);
                if (health > 0) ReactToShot(hitPoint);
                if (health < 0) health = 0;
                if (health == 0)
                {
                    Die(hitPoint);
                }
            }
        }

        private void OnDestroy()
        {
            Core.GameOver.EndGame -= DestroyZombie;
            LevelPanel.ChangeLevel -= DestroyZombie;
        }
        #endregion

        #region Support Methods
        private void GetComponents()
        {
            zombieAnimation = GetComponent<Animator>();
            motion = GetComponent<NavMeshAgent>();
            ColliderOpener = GetComponentInChildren<ColliderOpener>();
            zombieSound = GetComponent<AudioSource>();
        }
        private void StopZombie()
        {
            StartCoroutine(StopZombieRoutine());
        }

        private void GoToPlayer(Vector3 playerPos)
        {
            speed = zombieSpeed;
            motion.speed = speed;
            motion.SetDestination(playerPos);
            if (zombieAnimation.GetBool("isAttak")) zombieAnimation.SetBool("isAttak", false);
            if (ColliderOpener.IsOpen) ColliderOpener.CloseCollider();
        }

        private void ReactToShot(Vector3 hitPoint)
        {
            isShot = true;
            zombieAnimation.SetTrigger("isDamage");
            speed = 0;
            motion.speed = speed;
            zombieSound.PlayOneShot(zombieSoundSO.HitZombie);
            Invoke(nameof(Go), hitAnimationTime);
            Instantiate(blood, hitPoint, transform.rotation);
        }

        private void Go()
        {
            speed = zombieSpeed;
            motion.speed = speed;
            isShot = false;
        }

        private void Die(Vector3 hitPoint)
        {
            float dieAnimationTime = 1.4f;
            isDie = true;
            zombieAnimation.SetTrigger("isDie");
            speed = 0;
            Core.IncreaseScore(scorePrice);
            Instantiate(blood, hitPoint, transform.rotation);
            zombieSound.PlayOneShot(zombieSoundSO.DieZombie);
            Invoke(nameof(FallInGround), dieAnimationTime);
        }

        private void FallInGround()
        {
            motion.enabled = false;
            Core.DeleteEnemyFromList(gameObject);
            StartCoroutine(DieRoutine());
        }

        private void DestroyZombie() => Destroy(gameObject);

        private void LookOnPlayer()
        {
            float rotationSpeed = 0.1f;
            Vector3 lookPos = playerPos.position - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed);
        }

        private bool IsLookOnPlayer()
        {
            float coefficient = 0.5f;
            float length = 6;
            int minRaycastpos = -1;
            int maxRaycastpos = 2;
            int iterationModefier = 2;
            Vector3 position = transform.position + new Vector3(0, 2, 0);
            for (int i = minRaycastpos; i < maxRaycastpos; i++)
            {
                Vector3 startPosition = position + coefficient * i * transform.right;
                Vector3 endPosition = transform.forward * length + coefficient * i * transform.right;
                Debug.DrawRay(startPosition, endPosition);
                Physics.Raycast(startPosition, endPosition, out hits[i + 2]);
                if (hits[i + iterationModefier].collider != null)
                {
                    if (hits[i + iterationModefier].collider.gameObject.CompareTag("Player"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsAttackAnimationEnd()
        {
            AnimatorStateInfo animatorStateInfo = zombieAnimation.GetCurrentAnimatorStateInfo(0);
            if (animatorStateInfo.IsName(animationName))
            {
                return false;
            }
            return true;
        }

        IEnumerator DieRoutine()
        {
            float positionChangingTime = 0.04f;
            while (transform.position.y > destroyPosition)
            {
                transform.position -= slowDying;
                yield return new WaitForSeconds(positionChangingTime);
            }
            Destroy(gameObject);
            yield return null;
        }

        IEnumerator StopZombieRoutine()
        {
            float attackAnimationTime = 0.7f;
            speed = 0;
            motion.speed = speed;
            if (!zombieAnimation.GetBool("isAttak"))
            {
                zombieAnimation.SetBool("isAttak", true);
            }
            yield return new WaitForSeconds(attackAnimationTime);
            if (!ColliderOpener.IsOpen && !isDie && !IsAttackAnimationEnd() && !ColliderOpener.IsHit)
            {
                ColliderOpener.OpenCollider();
            }
        }

        public void SetEnemyStats(Enemy enemy)
        {
            int scoreModifier = 10;
            startHealth = enemy.Health;
            instantiateLevel = enemy.InstantiateLevel;
            zombieSpeed = enemy.Speed;
            damage = enemy.Damage;
            scorePrice = enemy.ScorePrice * scoreModifier;
        }
        #endregion
    }
}
