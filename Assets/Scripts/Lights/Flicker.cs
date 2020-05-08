using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Flicker : MonoBehaviour {
    public new Light2D light;
    public float flickerRange = 3f, flickerUpdateTime = 0.1f;
    public float itensityRange, startingOuterRadius, startingItensity;
    float tmrFlicker, targetOuterRadius, targetIntensity;
    public bool isPutOut;

    private void Start() {
        startingOuterRadius = light.pointLightOuterRadius;
        startingItensity = light.intensity;
    }

    private void Update() {
        if (isPutOut) {
            light.pointLightOuterRadius = Mathf.Lerp(light.pointLightOuterRadius, 0, 0.05f);
            return;
        }

        if (light.enabled && light.pointLightOuterRadius > 0) {
            light.pointLightOuterRadius = Mathf.Lerp(light.pointLightOuterRadius, targetOuterRadius, 0.05f);
            light.intensity = Mathf.Lerp(light.intensity, targetIntensity, 0.05f);

            tmrFlicker += Time.deltaTime;
            if (tmrFlicker >= flickerUpdateTime) {
                targetOuterRadius = Random.Range(startingOuterRadius - flickerRange, targetOuterRadius + flickerRange);
                targetIntensity = Random.Range(startingItensity - itensityRange, targetIntensity + itensityRange);
                tmrFlicker = 0;
            }
        } else if (light.enabled && light.pointLightOuterRadius <= 0) {
            light.pointLightOuterRadius = Mathf.Lerp(light.pointLightOuterRadius, 0, 0.05f);
        }
    }

    public void ChangeRadius(float offset) {
        startingOuterRadius += offset;
        targetOuterRadius += offset;
    } 
}
