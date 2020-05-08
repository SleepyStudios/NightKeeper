using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightCollider : MonoBehaviour {
    public new CircleCollider2D collider;
    public new Light2D light;

    private void FixedUpdate() {
        collider.radius = light.pointLightOuterRadius;
    }
}
