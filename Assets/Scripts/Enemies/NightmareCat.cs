using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareCat : Enemy {
    protected override void SetTarget() {
        target = GameObject.FindGameObjectWithTag("Fire").transform;
    }

    protected override void OnAttack() {
        target.GetComponent<Fire>().OnHit(damage);
        OnKill();
    }
}
