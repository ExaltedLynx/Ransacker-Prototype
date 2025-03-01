using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemies
{
    private static List<Enemy> enemies;
    private static ILookup<DungeonType, Boss> bosses; //this is basically a Dictionary<DungeonType, IEnumerable<Boss>>

    public static void LoadEnemies()
    {
        enemies = Resources.LoadAll<Enemy>("Enemies").ToList();
        bosses = Resources.LoadAll<Boss>("Bosses").ToLookup(boss => boss.dungeonType, boss => boss);
    }

    public static List<Enemy> GetAllEnemies()
    {
        return enemies;
    }

    public static Enemy GetRandomEnemy()
    {
        return enemies[Random.Range(0, enemies.Count)];
    }

    public static Boss GetRandomBoss(DungeonType dungeonType)
    {
        IEnumerable<Boss> floorBosses = bosses[dungeonType];
        return floorBosses.ElementAt(Random.Range(0, floorBosses.Count()));
    }
}
