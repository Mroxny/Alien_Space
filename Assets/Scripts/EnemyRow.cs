using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRow : MonoBehaviour
{
    public GameObject enemy;
    public float speed = .2f;
    public int enemiesInRow = 4;

    private float startPosX = -.75f;
    private Vector2 direction = new Vector2(1, 0);
    private GameObject[] enemies;
    void Start()
    {
        enemies = new GameObject[enemiesInRow];
        SpawnEnemies();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x >= .5f) direction = new Vector2(-1, 0);
        if (transform.position.x <= -.5f) direction = new Vector2(1, 0);

        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void SpawnEnemies() {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = Instantiate(enemy, transform);
            enemies[i].transform.position = new Vector2(startPosX + (.5f * i), transform.position.y);
        }
    }

    public void MakeHarder() {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null) {
                enemies[i].GetComponent<EnemyController>().shootDelay *= .5f;
                enemies[i].GetComponent<EnemyController>().projectileSpeed *= 1.25f;
            } 
        }
    }
}
