using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cinemachine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    public float speed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    public new ParticleSystem particleSystem;

    private float xUnitSpeed, yUnitSpeed;
    private float xSpeed, ySpeed;
    private PlayerInputActions controls;

    [Header("Shooting")]
    public GameObject arrowPrefab;
    private bool isHoldingDown, canShoot;
    private float heldDownTime;
    public SpriteRenderer chargeBarBack;
    public Transform chargeBar;

    [Header("Camera")]
    public Animator camAnimator;
    public CinemachineCameraShaker shaker;

    public Volume postProcessing;
    private ChromaticAberration chromaticAberration;
    private FilmGrain filmGrain;

    [Header("Fire")]
    public Fire fire;
    private bool isHoldingResource;

    [Header("Sanity")]
    public float sanity = 100f;
    public float sanityDecayRate, sanityDecayAmount, sanityGrowRate, sanityGrowAmount;
    private float tmrSanity;
    private bool inLight;
    public AudioSource music;

    [Header("Sounds")]
    public AudioClip hitSound;
    public AudioClip woodPickupSound;
    public AudioClip footStepSound;
    private float tmrFootStep;
    public float footStepRate = 0.4f;

    private void Awake() {
        controls = new PlayerInputActions();
    }

    public void OnEnable() {
        controls.Enable();
        
        controls.Player.Fire.started += context => {
            isHoldingDown = true;
            camAnimator.SetBool("aiming", true);
        };
        controls.Player.Fire.performed += context => {
            canShoot = true;
        };
        controls.Player.Fire.canceled += context => {
            if (canShoot) ShootArrow();
            isHoldingDown = canShoot = false;
            camAnimator.SetBool("aiming", false);
        };
        controls.Player.Move.performed += context => {
            if (!isHoldingDown) {
                Vector2 axis = context.ReadValue<Vector2>();
                if (axis.x < 0) sr.flipX = false;
                if (axis.x > 0) sr.flipX = true;
                animator.SetFloat("ySpeed", -1);
            }
        };
    }

    public void OnDisable() {
        controls.Disable();
    }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        
        postProcessing.profile.TryGet(out chromaticAberration);
        postProcessing.profile.TryGet(out filmGrain);

        chargeBar.localScale = new Vector3(0, 1f, 1f);
        chargeBarBack.GetComponent<SpriteRenderer>().enabled = false;

        Instantiate(arrowPrefab, GetComponent<Transform>());
        
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate() {
        if (sanity <= 0) return;

        HandleChargeUp();
        HandleSanity();
        HandleMovement();
        HandleAim();
    }

    private void LateUpdate() {
        sr.sortingOrder = (int)(Camera.main.WorldToScreenPoint(sr.bounds.min).y + 50) * -1;
    }

    private void HandleSanity() {
        tmrSanity += Time.deltaTime;

        if (!inLight) {
            if (tmrSanity >= sanityDecayRate) {
                sanity -= sanityDecayAmount;
                tmrSanity = 0;
            }
        } else {;
            if (tmrSanity >= sanityGrowRate) {
                sanity += sanityGrowAmount;
                tmrSanity = 0;
            }
        }
        sanity = Mathf.Clamp(sanity, 0f, 100f);
        
        float targetPitch = ((sanity * 2f) / 100f) - 1f;
        music.pitch = Mathf.Lerp(music.pitch, targetPitch, 0.1f);
        chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, (100f - sanity) / 100f, 0.1f);
        filmGrain.intensity.value = Mathf.Lerp(filmGrain.intensity.value, (100f - sanity) / 100f, 0.1f);
    }

    private void HandleMovement() {
        Vector2 axis = controls.Player.Move.ReadValue<Vector2>();
        xUnitSpeed = axis.x;
        yUnitSpeed = axis.y;

        xSpeed = xUnitSpeed * speed * Time.fixedDeltaTime;
        ySpeed = yUnitSpeed * speed * Time.fixedDeltaTime;

        // Magnitude to make diagonal movements the same speed as vertical and horizontal
        float magnitude = 1.0f;
        if (!(Mathf.Approximately(xUnitSpeed, 0) && Mathf.Approximately(yUnitSpeed, 0))) {
            magnitude = Mathf.Sqrt(Mathf.Pow(xUnitSpeed, 2) + Mathf.Pow(yUnitSpeed, 2));
        } else {
            magnitude = 1.0f;
        }

        animator.SetBool("running", !Mathf.Approximately(xUnitSpeed, 0) || !Mathf.Approximately(yUnitSpeed, 0));
        if (!isHoldingDown && !Mathf.Approximately(yUnitSpeed, 0)) {
            animator.SetFloat("ySpeed", yUnitSpeed);
        }

        if (animator.GetBool("running")) {
            tmrFootStep += Time.deltaTime;
            if (tmrFootStep >= footStepRate) {
                PlaySound(footStepSound);
                tmrFootStep = 0;
            }
        }
        rb.MovePosition(rb.position + new Vector2(xSpeed / magnitude, ySpeed / magnitude));
    }

    private void HandleAim() {
        if (isHoldingDown) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 direction = mousePos - transform.position;
            sr.flipX = direction.x > 0f;
            animator.SetFloat("ySpeed", direction.y);
        }
    }

    private void ShootArrow() {
        GetComponentInChildren<Arrow>().Fire(heldDownTime);
        Instantiate(arrowPrefab, GetComponent<Transform>());
    }

    private void HandleChargeUp() {
        if (isHoldingDown) {
            heldDownTime += Time.deltaTime;
        } else {
            heldDownTime = 0;
        }

        float width = Mathf.Lerp(chargeBar.localScale.x, heldDownTime / 0.5f, 0.2f);
        chargeBar.localScale = new Vector3(Mathf.Clamp01(width), 1f, 1f);
        chargeBarBack.enabled = width >= 0.01f;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Resource")) {
            Resource r = collision.GetComponent<Resource>();
            if (!isHoldingResource && r.canPickUp) {
                r.OnPickup(transform);
                isHoldingResource = true;
                PlaySound(woodPickupSound);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Sticks")) {
            if (isHoldingResource) {
                fire.StokeFire();
                Destroy(GetComponentInChildren<Resource>().gameObject);
                isHoldingResource = false;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Light Collider")) {
            inLight = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Light Collider")) {
            inLight = false;
        }
    }

    public void OnHit(float damage) {
        shaker.ShakeCamera(0.3f);
        particleSystem.Play();

        sanity -= damage;
        sanity = Mathf.Clamp(sanity, 0f, 100f);
        PlaySound(hitSound);
    }

    private void PlaySound(AudioClip clip) {
        AudioSource source;
        if (TryGetComponent(out source)) {
            source.pitch = Random.Range(0.75f, 1.25f);
            source.clip = clip;
            source.Play();
        }
    }
}
