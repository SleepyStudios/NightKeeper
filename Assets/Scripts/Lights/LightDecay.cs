using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightDecay : MonoBehaviour {
    public Flicker flicker;
    public float decayRate, decayAmount;
    private float tmrTick;

    protected virtual void FixedUpdate() {
        tmrTick += Time.deltaTime;
        if (tmrTick >= decayRate) {
            flicker.ChangeRadius(-decayAmount);
            tmrTick = 0;
        }
    }
}
