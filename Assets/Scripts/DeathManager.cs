using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour {
    public Player player;
    public EnemyManager enemyManager;
    public LeaderboardManager leaderboardManager;
    public GameObject leaderboardUI;
    private bool gameOver;

    private void Start() {
        leaderboardUI.SetActive(false);
    }

    private void FixedUpdate() {
        if (!gameOver && Mathf.Approximately(player.sanity, 0f)) {
            gameOver = true;
            enemyManager.KillAllEnemies();
            enemyManager.gameOver = true;

            leaderboardUI.SetActive(true);
            leaderboardManager.SetScore();

            Flicker[] flickers = FindObjectsOfType<Flicker>();
            for (int i = 0; i < flickers.Length; i++) flickers[i].isPutOut = true;
        }
    }
}
