using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using System.Linq;

public class Enemy : MonoBehaviour {
    private int health;
    public Transform target;
    public float speed = 6f, damage = 2f, attackRange = 1.25f;
    public new ParticleSystem particleSystem;
    public float attackSpeed = 0.5f;
    private float tmrAttack;
    private bool canAttack;
    private Animator animator;
    private bool dead;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        SetHealth();
        SetTarget();
    }

    private void SetHealth() {
        health = GetEyes().Length;
    }

    protected virtual void SetTarget() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate() {
        if (dead) return;

        tmrAttack += Time.deltaTime;
        if(tmrAttack >= attackSpeed) {
            canAttack = true;
            tmrAttack = 0;
        }

        if (target != null) {
            MoveTowardsTarget();
            if (Vector2.Distance(target.position, transform.position) <= attackRange) {
                OnAttack();
            }
        }
    }

    protected virtual void MoveTowardsTarget() {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (target.position.x < transform.position.x) {
            transform.localScale = new Vector3(-1, 1, 1);
        } else {
            transform.localScale = Vector3.one;
        }
    }

    public void OnHit(Vector2 force) {
        particleSystem.Play();

        GetComponent<Rigidbody2D>().AddRelativeForce(force);
        
        if (health == 1) {
            FindObjectOfType<ScoreManager>().EnemyKill();
            OnKill();
        } else if (health > 1) {
            PlaySound();
            Destroy(transform.GetChild(Random.Range(0, health)).gameObject);
            health -= 1;
        }
    }

    public void OnKill() {
        StartCoroutine(DestroyAllEyes());

        Arrow[] arrows = GetComponentsInChildren<Arrow>();
        for (int i = 0; i < arrows.Length; i++) {
            arrows[i].Detach();
        }

        GameObject deathSound = Instantiate(Resources.Load("Nightmare Death Sound") as GameObject);
        deathSound.GetComponent<AudioSource>().clip = GetComponent<AudioSource>().clip;
        deathSound.GetComponent<AudioSource>().Play();

        animator.SetTrigger("death");
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<Rigidbody2D>());
        dead = true;
    }

    public void OnDeathAnimationFinish() {
        Destroy(gameObject);
    }

    protected virtual void OnAttack() {
        if (canAttack && target.GetComponent<Player>()) {
            target.GetComponent<Player>().OnHit(damage);
            canAttack = false;
        }
    }

    private Light2D[] GetEyes() {
        return GetComponentsInChildren<Light2D>().Where(l => l.gameObject.name.StartsWith("Nightmare Eye")).ToArray();
    }

    IEnumerator DestroyAllEyes() {
        Light2D[] eyes = GetEyes();
        for (int i = 0; i < eyes.Length; i++) {
            particleSystem.Play();
            eyes[i].intensity = 0;
            eyes[i].pointLightOuterRadius = 0;
            yield return new WaitForSeconds(0.5f);
        }
        yield return null;
    }

    private void PlaySound() {
        AudioSource source;
        if (TryGetComponent(out source)) {
            source.pitch = Random.Range(0.75f, 1.25f);
            source.Play();
        }
    }
}
