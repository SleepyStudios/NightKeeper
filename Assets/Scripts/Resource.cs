using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {
    public bool canPickUp;

    private void Start() {
        transform.localScale = Vector3.zero;
    }

    private void FixedUpdate() {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.1f);
        canPickUp = transform.localScale.x >= 0.8f;
    }

    public void OnPickup(Transform t) {
        transform.localScale = Vector3.one;
        transform.parent = t;
        transform.localPosition = new Vector2(0f, 0.5f);
    }
}
