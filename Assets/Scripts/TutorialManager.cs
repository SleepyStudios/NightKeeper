using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour {
    public string[] tutorials;
    public float timeToRead = 2f;
    private float tmrRead;
    private int step = -1;
    public TextMeshProUGUI textContainer;
    public bool isFinished;
    public Animator animator;
    private bool canCountdownText;

    private void FixedUpdate() {
        if (isFinished) return;

        if (canCountdownText) tmrRead += Time.deltaTime;
        if (tmrRead >= timeToRead) {
            if (step < tutorials.Length - 1) {
                UpdateTutorialStep();
            } else {
                animator.SetBool("hide", true);
                isFinished = true;
            }
            tmrRead = 0;
        }
    }

    private void UpdateTutorialStep() {
        canCountdownText = false;
        animator.SetBool("hide", true);
    }

    private void OnEnterAnimStarted() {
        step++;
        textContainer.text = tutorials[step];
    }

    private void OnEnterAnimFinished() {
        canCountdownText = true;
    }

    private void OnExitAnimFinished() {
        if (!isFinished) animator.SetBool("hide", false);
    }
}
