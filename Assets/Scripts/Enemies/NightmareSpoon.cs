using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareSpoon : Enemy {
    public float magnitude = 0.5f;

    protected override void SetTarget() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void MoveTowardsTarget() {
        base.MoveTowardsTarget();

        transform.position = transform.position + transform.right * Mathf.Sin(Time.time * speed) * magnitude;
    }
}
