using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {
    public List<GameObject> resourcePrefabs;
    public int initialSpawns, maxSpawns;
    private float tmrSpawn;
    public Collider2D colliderBounds;
    public GameObject fire, player;
    public float distanceRequiredFromPlayerAndFire;
    public float spawnRate;

    private void Start() {
        for (int i = 0; i < initialSpawns; i++) {
            Spawn();
        }
    }

    private void FixedUpdate() {
        tmrSpawn += Time.deltaTime;
        if (tmrSpawn >= spawnRate) {
            Spawn();
            tmrSpawn = 0;
        }
    }

    private void Spawn() {
        Bounds bounds = colliderBounds.bounds;
        Vector2 center = bounds.center;
        Vector2 pos;

        do {
            float x = Random.Range(center.x - bounds.extents.x, center.x + bounds.extents.x);
            float y = Random.Range(center.y - bounds.extents.y, center.y + bounds.extents.y);
            pos = new Vector2(x, y);
        } while (!colliderBounds.OverlapPoint(pos) ||
            Vector2.Distance(pos, fire.transform.position) < distanceRequiredFromPlayerAndFire ||
            Vector2.Distance(pos, player.transform.position) < distanceRequiredFromPlayerAndFire);

        GameObject pickedResource = resourcePrefabs[Random.Range(0, resourcePrefabs.Count)];
        Instantiate(pickedResource, pos, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)), transform);
    }
}
