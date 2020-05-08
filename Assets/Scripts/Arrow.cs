using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

public class Arrow : MonoBehaviour {
    private Rigidbody2D rb;
    private BoxCollider2D b2d;
    private new Light2D light;
    private bool shot;
    public float drag = 0.6f, heldDownTimeMultiplier = 10f, minVelocity = 12f, maxVelocity = 22f, knockbackForceMultiplier = 5f;
    private CinemachineCameraShaker shaker;
    public new ParticleSystem particleSystem;
    public GameObject lightCollider;
    private bool canCountAsNotMoving;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        light = GetComponent<Light2D>();
        light.enabled = false;
        GetComponent<LightDecay>().enabled = false;
        shaker = FindObjectOfType<CinemachineCameraShaker>();
        lightCollider.SetActive(false);
    }

    private void Update() {
        if (!shot) {
            if (Mouse.current == null) return;
            Vector3 perpendicular = Vector3.Cross(transform.position - mouseWorldPos(), Vector3.forward);
            transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);
        } else {
            if (!IsMoving() && transform.parent == null && !light.enabled) Detach();
        }
    }

    public void Fire(float heldDownTime) {
        gameObject.layer = LayerMask.NameToLayer("Arrow");
        transform.parent = null;
        shot = true;

        Vector2 direction = mouseWorldPos() - transform.position;
        direction.Normalize();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.drag = drag;
        rb.mass = 1f;
        rb.velocity = direction * Mathf.Clamp(heldDownTime * heldDownTimeMultiplier, minVelocity, maxVelocity);

        lightCollider.SetActive(true);
    }

    private Vector3 mouseWorldPos() {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!shot) return;

        if (collision.CompareTag("Enemy") && IsMoving()) {
            Vector2 velocity = rb.velocity * knockbackForceMultiplier;

            collision.GetComponent<Enemy>().OnHit(velocity);
            shaker.ShakeCamera(0.3f);

            if (light.enabled) {
                FindObjectOfType<ScoreManager>().FireArrowHit();
                SetParent(collision.transform);
            } else {
                rb.velocity = Vector2.zero;
            }
        }

        TestFireHit(collision.tag, collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!shot) return;

        TestFireHit(collision.tag, collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (!shot) return;

        TestFireHit(collision.tag, collision.gameObject);
    }

    private void TestFireHit(string tag, GameObject gameObject) {
        if (!light.enabled && tag == "Fire" && gameObject.GetComponent<Fire>().IsLit()) {
            light.enabled = true;
            particleSystem.Play();
            PlaySound();

            GetComponent<LightDecay>().enabled = true;
        }
    }

    private bool IsMoving() {
        if (rb == null) return false;
        return rb.velocity.x > 0.05f || rb.velocity.y > 0.05f || rb.velocity.x < -0.05f || rb.velocity.y < -0.05f;
    }

    private void SetParent(Transform t) {
        transform.SetParent(t);
        transform.localScale = new Vector2(0.35f, 0.35f);
        Destroy(rb);
    }

    private void PlaySound() {
        AudioSource source;
        if (TryGetComponent(out source)) {
            source.pitch = Random.Range(0.75f, 1.25f);
            source.Play();
        }
    }

    public void Detach() {
        transform.SetParent(null);
        transform.localScale = Vector2.one;
        gameObject.layer = LayerMask.NameToLayer("TransparentFX");
        GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        GetComponent<SpriteRenderer>().sortingOrder = 3;
        light.enabled = false;
    }
}
