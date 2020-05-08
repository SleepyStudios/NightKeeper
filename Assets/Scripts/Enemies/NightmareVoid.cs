using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareVoid : Enemy {
    public float circlingSpeedMultiplier = 20f;

    protected override void MoveTowardsTarget() {
        base.MoveTowardsTarget();

        transform.RotateAround(target.position, new Vector3(0, 0, 1), speed * circlingSpeedMultiplier * Time.deltaTime);
    }
}
