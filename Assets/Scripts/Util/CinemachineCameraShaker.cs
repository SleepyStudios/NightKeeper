using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// https://github.com/Lumidi/CameraShakeInCinemachine/blob/master/SimpleCameraShakeInCinemachine.cs

public class CinemachineCameraShaker : MonoBehaviour {
    
    public float ShakeAmplitude = 1.2f;         // Cinemachine Noise Profile Parameter
    public float ShakeFrequency = 2.0f;         // Cinemachine Noise Profile Parameter

    private float ShakeElapsedTime = 0f;

    // Cinemachine Shake
    private CinemachineVirtualCamera VirtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    public void ShakeCamera(float duration) {
        VirtualCamera = GetComponent<CinemachineStateDrivenCamera>().LiveChild as CinemachineVirtualCamera;
        // Get Virtual Camera Noise Profile
        virtualCameraNoise = VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        ShakeElapsedTime = duration;
    }

    // Update is called once per frame
    void Update() {
        if (VirtualCamera != null) {
            // If Camera Shake effect is still playing
            if (ShakeElapsedTime > 0) {
                // Set Cinemachine Camera Noise parameters
                virtualCameraNoise.m_AmplitudeGain = ShakeAmplitude;
                virtualCameraNoise.m_FrequencyGain = ShakeFrequency;

                // Update Shake Timer
                ShakeElapsedTime -= Time.deltaTime;
            } else {
                // If Camera Shake effect is over, reset variables
                virtualCameraNoise.m_AmplitudeGain = 0f;
                ShakeElapsedTime = 0f;
            }
        }
    }
}