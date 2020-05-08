using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLoop : MonoBehaviour {
    public AudioSource loop;
    public LightCollider lightCollider;
    public float maxDistanceMultiplier = 5f;

    private void FixedUpdate() {
        loop.maxDistance = lightCollider.collider.radius * maxDistanceMultiplier;
    }
}
