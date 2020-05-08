using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : LightDecay {
    public float minGrowAmount, maxGrowAmount;
    public TutorialManager tutorial;

    public void StokeFire() {
        flicker.ChangeRadius(Random.Range(minGrowAmount, maxGrowAmount));
        FindObjectOfType<ScoreManager>().FireStoked();
        PlaySound();
    }

    public void OnHit(float damage) {
        flicker.ChangeRadius(-damage);
    }

    public bool IsLit() {
        return flicker.light.pointLightOuterRadius >= 0.1f;
    }

    private void PlaySound() {
        AudioSource source;
        if (TryGetComponent(out source)) {
            source.pitch = Random.Range(0.75f, 1.25f);
            source.Play();
        }
    }

    protected override void FixedUpdate() {
        if (tutorial.isFinished) base.FixedUpdate();
    }
}
