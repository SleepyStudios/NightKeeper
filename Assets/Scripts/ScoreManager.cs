using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public int fireArrowHitPoints;
    public int enemyKillPoints;
    public int fireStokePoints;

    private int fireArrowHits;
    private int enemiesKilled;
    private int fireStokes;

    private void Start() {
        // where else would you put this lol
        Application.targetFrameRate = 60;
    }

    public void FireArrowHit() {
        fireArrowHits++;
    }

    public void EnemyKill() {
        enemiesKilled++;
    }

    public void FireStoked() {
        fireStokes++;
    }

    public int CalculateScore() {
        return (fireArrowHitPoints * fireArrowHits) + (enemyKillPoints * enemiesKilled) + (fireStokePoints * fireStokes);
    }
}
