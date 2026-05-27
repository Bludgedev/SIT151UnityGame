using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class PlayerShipController : MonoBehaviour, IDamageable
{
    public static PlayerShipController Instance { get; private set; }

    public ShipGameMode gameMode;
    public AudioClip shootClip;

    public float RamDamage => ramDamage;

    [SerializeField] private float ramDamage = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float screenPadding = 0.5f;

    [SerializeField] private RuntimeAnimatorController explosionController;
    [SerializeField] private WeaponController weaponController;

    private PlayerHealth playerHealth;
    private Camera mainCamera;
    private Rigidbody rb;

    private float lastRamTime;
    private const float ramCooldown = 0.25f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.OnDeath += HandleDeath;

        if (gameMode != null)
            playerHealth.OnDeath += gameMode.TriggerGameOver;
    }

    private void Update()
    {
        if (gameMode != null && gameMode.CurrentState == GameState.GameOver)
            return;

        HandleInput();
    }

    private void FixedUpdate()
    {
        if (gameMode != null && gameMode.CurrentState == GameState.GameOver)
            return;

        Move();
        ClampToScreen();
    }

    private void HandleInput()
    {
        if (InputManager.Instance.FirePressed)
            weaponController.FireCurrent();

        if (InputManager.Instance.NextWeaponPressed)
            weaponController.SwitchWeapon(+1);

        if (InputManager.Instance.PreviousWeaponPressed)
            weaponController.SwitchWeapon(-1);
    }

    private void Move()
    {
        Vector2 input = InputManager.Instance.MoveInput;
        Vector3 dir = new Vector3(input.x, input.y, 0f);

        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }

    private void ClampToScreen()
    {
        Vector3 pos = rb.position;

        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        pos.x = Mathf.Clamp(pos.x,
            mainCamera.transform.position.x - camWidth + screenPadding,
            mainCamera.transform.position.x + camWidth - screenPadding);

        pos.y = Mathf.Clamp(pos.y,
            mainCamera.transform.position.y - camHeight + screenPadding,
            mainCamera.transform.position.y + camHeight - screenPadding);

        rb.position = pos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("PLAYER COLLISION WITH " + collision.gameObject.name);
        
        if (Time.time - lastRamTime < ramCooldown)
            return;

        lastRamTime = Time.time;

        CombatResolver.ProcessRam(gameObject, collision.gameObject, collision);
    }

    private void HandleDeath()
    {
        Explode();
    }

    private void Explode()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayExplosion();
        }

        GameObject fx = new GameObject("PlayerExplosionFX");
        fx.transform.position = transform.position;

        var sr = fx.AddComponent<SpriteRenderer>();
        var anim = fx.AddComponent<Animator>();

        anim.runtimeAnimatorController = explosionController;
        sr.sortingOrder = 10;

        Destroy(fx, 1f);

        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Player took damage: " + damage);
    }
}