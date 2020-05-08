using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bow : MonoBehaviour {
    private void FixedUpdate() {
        if (Mouse.current == null) return;
        Vector3 perpendicular = Vector3.Cross(transform.position - mouseWorldPos(), Vector3.forward);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);
    }

    private Vector3 mouseWorldPos() {
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
}
