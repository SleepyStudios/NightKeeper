using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneryController : MonoBehaviour {
    private SpriteRenderer sr;

    private void Start() {
        sr = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate() {
        sr.sortingOrder = (int)(Camera.main.WorldToScreenPoint(sr.bounds.min).y * -1 -10) ;
    }
}