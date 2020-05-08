using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    private float tmrSpawn;
    public float spawnRate;
    public bool gameOver;
    public TutorialManager tutorialManager;

    private void FixedUpdate() {
        if (gameOver || !tutorialManager.isFinished) return;

        tmrSpawn += Time.deltaTime;
        if (tmrSpawn >= spawnRate) {
            Vector3 pos = Vector3.zero;

            switch (Random.Range(0, 4)) {
                case 0:
                    pos = Camera.main.ViewportToWorldPoint(new Vector3(-1, 1, 1));
                    break;
                case 1:
                    pos = Camera.main.ViewportToWorldPoint(new Vector3(-1, -1, 1));
                    break;
                case 2:
                    pos = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 1));
                    break;
                case 3:
                    pos = Camera.main.ViewportToWorldPoint(new Vector3(1, -1, 1));
                    break;
            }

            GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], pos, Quaternion.identity, transform);;
            tmrSpawn = 0;
        }
    }

    public void KillAllEnemies() {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        for (int i = 0; i < enemies.Length; i++) {
            enemies[i].OnKill();
        }
    }
}
