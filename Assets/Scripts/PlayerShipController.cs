using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class PlayerShipController : MonoBehaviour, IDamageable
{
    public static PlayerShipController Instance { get; private set; }
    public GameObject bulletTemplate;
    public ShipGameMode gameMode;
    public AudioClip shootClip;
    public float RamDamage => ramDamage;

    private PlayerHealth playerHealth;
    private Camera mainCamera;
    private Rigidbody rb;
    
    [SerializeField] private float screenPadding = 0.5f;

    [SerializeField] private float ramDamage = 5f;

    [SerializeField] private AudioStressController audioStress;

    [SerializeField] private WeaponController weaponController;

    [SerializeField] private RuntimeAnimatorController explosionController;

   
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
   
        playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth missing on PlayerShip at Start!");
        }


    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        rb = GetComponent<Rigidbody>();

        if (weaponController == null)
        {
            Debug.LogError("WeaponController missing on PlayerShip!");
        }

        if (gameMode == null)
        {
            Debug.LogError("GameMode not assigned in Inspector!");
            return;
        }

        var col = GetComponent<Collider>();

        if (col == null)
        {
            Debug.LogError($"{name}: NO COLLIDER");
        }
        

        playerHealth = GetComponent<PlayerHealth>();

        playerHealth.OnDeath += gameMode.TriggerGameOver;
    }

    // Update is called once per frame
    void Update()
    {
         
        
        if (gameMode != null && gameMode.CurrentState == GameState.GameOver)
            return;

        // --- WEAPON INPUT ---
        if (InputManager.Instance.FirePressed)
        {
            weaponController.FireCurrent();

            if (AudioManager.Instance != null && shootClip != null)
            {
                AudioManager.Instance.PlaySFX(shootClip);
            }
        }

        if (InputManager.Instance.NextWeaponPressed)
        {
            weaponController.SwitchWeapon(+1);
        }

        if (InputManager.Instance.PreviousWeaponPressed)
        {
            weaponController.SwitchWeapon(-1);
        }


        // --- MOVEMENT ---
        Rigidbody rb = GetComponent<Rigidbody>();

        Vector2 input = InputManager.Instance.MoveInput;

        Vector3 direction = new Vector3(input.x, input.y, 0f);

        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        float speed = 5f;

        rb.MovePosition(rb.position + direction * speed * Time.deltaTime);

        // --- Clamp to Camera ---
        Vector3 pos = transform.position;

        // Camera world bounds
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        // Clamp player inside visible area
        pos.x = Mathf.Clamp(
            pos.x,
            mainCamera.transform.position.x - camWidth + screenPadding,
            mainCamera.transform.position.x + camWidth - screenPadding
        );

        pos.y = Mathf.Clamp(
            pos.y,
            mainCamera.transform.position.y - camHeight + screenPadding,
            mainCamera.transform.position.y + camHeight - screenPadding
        );

        transform.position = pos;

    }

    private void OnCollisionEnter(Collision collisionInfo)
    {
        if (!collisionInfo.gameObject.CompareTag("EnemyShip"))
            return;

        EnemyBase enemy = collisionInfo.gameObject.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            // BOTH SIDES DAMAGE VIA COMBAT RESOLVER
            CombatResolver.DealDirectDamage(gameObject, enemy.gameObject, ramDamage);
            CombatResolver.DealDirectDamage(enemy.gameObject, gameObject, ramDamage);
        }
        Debug.Log($"COLLISION: {gameObject.name} hit {collisionInfo.gameObject.name}");
        Debug.Log($"Collision with {collisionInfo.gameObject.name} - Ram damage applied");
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath += HandleDeath;
            playerHealth.OnDeath += gameMode.TriggerGameOver;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandleDeath;
            playerHealth.OnDeath -= gameMode.TriggerGameOver;
        }
    }

    private void HandleDeath()
    {
        Explode();
    }

    private void Explode()
    {
        GameObject fx = new GameObject("PlayerExplosionFX");

        fx.transform.position = transform.position;
        fx.transform.rotation = Quaternion.identity;

        var sr = fx.AddComponent<SpriteRenderer>();
        var anim = fx.AddComponent<Animator>();

        anim.runtimeAnimatorController = explosionController;
        sr.sortingOrder = 10;

        Destroy(fx, 1f);

        Destroy(gameObject);
    }

    public virtual void TakeDamage(float damage)
    {
        Debug.Log(" TakeDamage() called");
    }


    private void OnCollisionStay(Collision collision)
    {
        Debug.Log($"[COLLISION STAY] {name} touching {collision.gameObject.name}");
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log($"[COLLISION EXIT] {name} left {collision.gameObject.name}");
    }

}