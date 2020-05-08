using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareBat : Enemy {
    public float jitter = 5, changeDirRate = 0.5f;
    private float tmrChangeDir;
    private Vector2 nextPos;

    protected override void SetTarget() {
        target = GameObject.FindGameObjectWithTag("Fire").transform;
    }

    protected override void MoveTowardsTarget() {
        tmrChangeDir += Time.deltaTime;
        if (nextPos == null || Vector2.Distance(transform.position, nextPos) <= 0.1f || tmrChangeDir >= changeDirRate) {
            ChangeDir();
        }

        transform.position = Vector2.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
    }

    protected override void OnAttack() {
        target.GetComponent<Fire>().OnHit(damage);
        OnKill();
    }

    private void ChangeDir() {
        Vector2 randomPos = new Vector2(target.position.x + Jitter(), target.position.y + Jitter());
        nextPos = randomPos;
        tmrChangeDir = 0;
    }

    private float Jitter() {
        return Random.Range(-jitter, jitter);
    }
}
