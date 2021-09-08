using UnityEngine;
using ZombieFight;
using ZombieFight.Interfaces.Core;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour, IPlayerController
{
    #region Fields
    ParticleSystem shooting;
    CharacterController controller;
    Transform bulletInstatce;
    Transform playerTransform;
    PlayerController playercontroller;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject explosion;
    [SerializeField] PlayerSoundsSO playerSoundsSO;
    AudioSource playerSounds;
    Animator anim;
    Vector3 movementDirection;
    Vector3 rotation;
    [SerializeField]float speed;
    float halfSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float health;
    [SerializeField] float maxCharacteristicValue;
    float gavityModifier = 10;
    float staticGravity = -0.2f;
    float armor;
    float gravity;
    float fireTime = 0;
    float fireRate = 0.2f;
    float aimingTime = 0.5f;
    float noFIreTime = 0;
    float noFire = 1.5f;
    bool isDie = false;
    #endregion

    #region Properties
    public Transform PlayerTransform => playerTransform;
    public event VoidDelegate Death;
    public event VoidDelegate Hit;
    IZombieFightClass CoreClass;
    IBounds ScreenBounds;
    IlevelPannel LevelPannelInt;
    #endregion

    #region Core Methods
    void Start()
    {
        FindObjects();
        GetComponents();
        playerTransform = gameObject.transform;
        halfSpeed = speed / 2;
        SubscribeToEvents();
    }

    private void Update()
    {
        Fire();
    }
    private void FixedUpdate()
    {
        MovePlayer();
        UseGravity();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject coll = other.gameObject;
        int zombieHandLayer = 12;
        if(coll.layer == zombieHandLayer)
        {
            float damage = coll.GetComponentInParent<Zombie>().Damage;
            GetDamage(damage);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject coll = collision.gameObject;
        int boxLayer = 11;
        if (coll.layer == boxLayer)
        {
            playerSounds.PlayOneShot(playerSoundsSO.GetBox);
            BoxObject boxObject = coll.GetComponent<BoxObject>();
            GetBonus(boxObject);
        }
    }
    private void OnDestroy() 
    { 
        Death -= PlayerDeath;
        CoreClass.GameOver.EndGame -= PlayerDestroy;
        CoreClass.StopLevel -= MuteSound;
        LevelPannelInt.ChangeLevel -= UnMuteSound;
    }

    #endregion

    #region Support Methods
    private void FindObjects()
    {
        CoreClass = GameObject.Find("GameManager").GetComponent<ZombieFightClass>();
        LevelPannelInt = GameObject.Find("Level").GetComponent<LevelPannel>();
        bulletInstatce = GameObject.Find("startFire").transform;
    }

    private void GetComponents()
    {
        playercontroller = GetComponent<PlayerController>();
        controller = GetComponent<CharacterController>();
        playerSounds = GetComponent<AudioSource>();
        ScreenBounds = Camera.main.gameObject.GetComponentInChildren<ScreenBounds>();
        anim = GetComponent<Animator>();
        shooting = explosion.GetComponent<ParticleSystem>();
    }

    private void SubscribeToEvents()
    {
        LevelPannelInt.ChangeLevel += UnMuteSound;
        CoreClass.GameOver.EndGame += PlayerDestroy;
        CoreClass.StopLevel += MuteSound;
    }
    private void MovePlayer()
    {
        movementDirection = Vector3.zero;
        rotation = Vector3.zero;
        float motion = Input.GetAxis("Vertical");
        float rotationSide = Input.GetAxis("Horizontal");
        SelectAnimation(motion, rotationSide);
        rotation.y = rotationSide * rotationSpeed;       
        if (motion > 0) movementDirection = transform.forward*Input.GetAxis("Vertical") * speed; 
        else movementDirection = transform.forward * Input.GetAxis("Vertical") * halfSpeed;             
        transform.Rotate(rotation);
        movementDirection.y = gravity;
        CheckPlayerPosition();
        controller.Move(movementDirection * Time.deltaTime);
    }

    private void UseGravity()
    {
        float gavityModifier = 10;
        float staticGravity = -0.2f;
        if (!controller.isGrounded) gravity -= gavityModifier * Time.deltaTime;
        else gravity = staticGravity;
    }
   
    private void CheckPlayerPosition()
    {
        Vector3 playerPos = transform.position;
        if (playerPos.z > ScreenBounds.ZRange.Item2 && movementDirection.z > 0)
        {
            movementDirection.z = 0;
            movementDirection.x = 0;
        }
        else if (playerPos.z < ScreenBounds.ZRange.Item1 && movementDirection.z < 0)
        {
            movementDirection.z = 0;
            movementDirection.x = 0;
        }
        if (playerPos.x > ScreenBounds.XRange.Item2 && movementDirection.x > 0)
        {
            movementDirection.x = 0;
        }
        else if(playerPos.x < ScreenBounds.XRange.Item1 && movementDirection.x < 0)
        {
            movementDirection.x = 0;
        }
       
    }

     private void SelectAnimation(float motion, float rotation)
    {
        if (motion != 0) anim.SetBool("inMotion", true);
        else if (rotation != 0 && motion == 0)
        {
            anim.SetBool("inMotion", false);
            anim.SetBool("turn", true);
            anim.SetFloat("turnSide", rotation);
        }
        else
        {
            anim.SetBool("turn", false);
            anim.SetBool("inMotion", false);
        }
    }

    private void Shoot()
    {
        if (!anim.GetBool("inMotion"))
        {
            if (!anim.GetBool("isShoot")) anim.SetBool("isShoot", true);
            Invoke(nameof(InstatiateBullet), aimingTime);
        }
        else
        {
            InstatiateBullet();
        }
    }

    private void InstatiateBullet()
    {
        playerSounds.PlayOneShot(playerSoundsSO.Shot);
        shooting.Play();
        Instantiate(bullet, bulletInstatce.position, bulletInstatce.rotation);
    }

    private void Fire()
    {
        if (Input.GetButton("Fire1") && Time.time > fireTime)
        {
            noFIreTime = Time.time + noFire;
            fireTime = Time.time + fireRate;
            Shoot();
        }
        else if (!Input.GetButton("Fire1") && Time.time > noFIreTime)
            anim.SetBool("isShoot", false);
    }
    private void GetDamage(float damage)
    {
        float difference = damage;
        int playerDeathTime = 3;
        Hit();
        if (armor > 0)
        {
            if (armor < damage)
            {
                difference = damage - armor;
            }
            armor -= damage;
            if(armor < 0)
            {
                armor = 0;
            }
            CoreClass.DecreaseStats(armor, ZombieFightClass.UIStats.armor);
        }
        if (armor == 0)
        {
            health -= difference;
            if (health > 0)
            {
                playerSounds.PlayOneShot(playerSoundsSO.HitReaction);
            }
            if (health <= 0)
            {
                health = 0;
            }
            CoreClass.DecreaseStats(health,ZombieFightClass.UIStats.health);
            if (health == 0)
            {
                anim.SetTrigger("isDie");
                playercontroller.enabled = false;
                if (!isDie)
                {
                    playerSounds.PlayOneShot(playerSoundsSO.Death);
                    isDie = true;
                }
                Invoke(nameof(PlayerDeath), playerDeathTime);
            }
        }
    }
    private void GetBonus(BoxObject boxObject)
    {
        switch(boxObject.BoxType)
        {
            case Box.BoxType.medicine:
                health += boxObject.Additional;
                if (health > maxCharacteristicValue) health = maxCharacteristicValue;
                CoreClass.DecreaseStats(health,ZombieFightClass.UIStats.health);
                break;
            case Box.BoxType.armor:
                armor += boxObject.Additional;
                if (armor > maxCharacteristicValue) armor = maxCharacteristicValue;
                CoreClass.DecreaseStats(armor, ZombieFightClass.UIStats.armor);
                break;
        }
        boxObject.DestroyBox();
    }

    private void MuteSound()
    {
        playerSounds.mute = true;
    }

    private void UnMuteSound()
    {
        playerSounds.mute = false;
    }
    private void PlayerDeath() => Death();

    private void PlayerDestroy() => Destroy(gameObject);
    #endregion
}
