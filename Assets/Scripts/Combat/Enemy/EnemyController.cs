using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance { get; private set; }
    public LinkedList<Enemy> Enemies { get; private set; }
    public OnEnemySpawnEvent onEnemySpawn;

    private Vector3 enemySpawn;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        enemySpawn = Vector3.zero;
        Enemies = new LinkedList<Enemy>();
    }


    void Update()
    {

    }

    private void OnDisable()
    {
        resetEnemies();
    }

    private void OnEnable()
    {
        enemySpawn = Vector3.zero;
        //SpawnEnemy(Random.Range(1, 5));
        //enableAllTimers();
    }

    public void SpawnEnemies(List<Enemy> enemies)
    {
        for (int x = 0; x < enemies.Count; x++)
        {
            StartCoroutine(InstantiateEnemy(enemies[x], x, enemies.Count));
        }
        onEnemySpawn.Invoke();
    }

    public void SpawnBoss(Boss boss)
    {
        enemySpawn = Vector3.zero;
        StartCoroutine(InstantiateEnemy(boss, 1, 1));
    }

    private IEnumerator InstantiateEnemy(Enemy enemy, int enemyNum, int totalEnemies)
    {
        
        Vector3 enemyTransform = transform.position + CalcSpawnPos(enemyNum, totalEnemies, enemy.transform);
        Enemy spawnedEnemy = Instantiate(enemy, enemyTransform, Quaternion.identity, transform);
        if (enemy is not Boss)
        {
            spawnedEnemy.name += enemyNum + 1;
        }

        Enemies.AddLast(new LinkedListNode<Enemy>(spawnedEnemy));

        if(enemyNum > 0 && enemyNum < totalEnemies) //enemies from 1 through totalEnemies - 1
        {
            //sets currently spawning enemy's right neighbor
            Enemies.Last.Value.RightEnemy = Enemies.Last.Previous.Value;
            //sets previously spawned enemy's left neighbor
            Enemies.Last.Previous.Value.LeftEnemy = Enemies.Last.Value;
        }
        else if (totalEnemies != 1 && enemyNum == totalEnemies - 1) //only the last enemy
        {
            Enemies.Last.Value.RightEnemy = Enemies.Last.Previous.Value;
        }
        yield return null;
    }


    //Calculates where the enemy is supposed to spawn in world space
    private Vector3 CalcSpawnPos(int x, int count, Transform enemyTransform)
    {
        float enemyScaleX = enemyTransform.localScale.x;
        float scaleMult = Mathf.Clamp(enemyScaleX / 2, 1, enemyScaleX);
        enemySpawn.y = enemyTransform.position.y;
        if (count == 1)
        {
            return enemySpawn;
        }

        if(x == 0) // position for first enemy spawned
        {
            int enemyXPos = count + Mathf.FloorToInt(count / 2);
            enemySpawn.x = enemyXPos * scaleMult;
            return enemySpawn;
        }

        //remaining enemy spawn positions
        enemySpawn.x -= 4 * scaleMult;
        return enemySpawn;
    }

    public void toggleAllTimers(bool toggle)
    {
        if (Enemies.Count != 0)
        {
            //Debug.Log("in for loop");
            foreach (Enemy enemy in Enemies)
            {
                enemy.GetAttackTimer().toggleTimer(toggle);
            }
        }
    }

    public void resetEnemies()
    {
        if (Enemies.Count != 0)
        {
            foreach (Enemy enemy in Enemies)
            {
                Destroy(enemy.gameObject);
            }
        }
        Enemies.Clear();
    }
}

[System.Serializable]
public class OnEnemySpawnEvent : UnityEvent { }
