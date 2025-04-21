using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPreviewController : MonoBehaviour
{
    [SerializeField] Image enemySprite1;
    [SerializeField] EnemyTooltipHandler enemyTooltipHandler1;
    private Enemy enemy1;
    [SerializeField] Image enemySprite2;
    [SerializeField] EnemyTooltipHandler enemyTooltipHandler2;
    private Enemy enemy2;
    [SerializeField] Image enemySprite3;
    [SerializeField] EnemyTooltipHandler enemyTooltipHandler3;
    private Enemy enemy3;
    [SerializeField] TextMeshProUGUI movementLeftText;

    public static RoomPreviewController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        enemySprite1.gameObject.SetActive(false);
        enemySprite2.gameObject.SetActive(false);
        enemySprite3.gameObject.SetActive(false);
    }

    private void Start()
    {

    }

    public void SetEnemyPreviewSprites(List<Enemy> enemies)
    {
        enemy1 = null;
        enemy2 = null;
        enemy3 = null;
        List<Enemy> mostFrequent = ListUtil.GetKMostFrequent(enemies, 3);
        foreach (Enemy enemy in mostFrequent)
            Debug.Log(enemy);

        enemySprite2.gameObject.SetActive(false);
        enemySprite3.gameObject.SetActive(false);

        enemySprite1.gameObject.SetActive(true);
        enemySprite1.sprite = mostFrequent[0].GetComponent<SpriteRenderer>().sprite;
        enemy1 = mostFrequent[0];
        enemyTooltipHandler1.displayedEnemy = enemy1;
        switch (mostFrequent.Count)
        {
            case 2:

                enemySprite2.gameObject.SetActive(true);
                enemySprite2.sprite = mostFrequent[1].GetComponent<SpriteRenderer>().sprite;
                enemy2 = mostFrequent[1];
                enemyTooltipHandler2.displayedEnemy = enemy2;
                break;
            case 3:
                enemySprite2.gameObject.SetActive(true);
                enemySprite3.gameObject.SetActive(true);
                enemySprite2.sprite = mostFrequent[1].GetComponent<SpriteRenderer>().sprite;
                enemySprite3.sprite = mostFrequent[2].GetComponent<SpriteRenderer>().sprite;
                enemy2 = mostFrequent[1];
                enemy3 = mostFrequent[2];
                enemyTooltipHandler2.displayedEnemy = enemy2;
                enemyTooltipHandler3.displayedEnemy = enemy3;
                break;
        }
    }

    public void SetBossPreviewSprite(Boss boss)
    {
        enemySprite1.gameObject.SetActive(true);
        enemySprite1.sprite = boss.GetComponent<SpriteRenderer>().sprite;
        enemy1 = boss;
        enemyTooltipHandler1.displayedEnemy = boss;

    }

    public void InitStepsLeftText()
    {
        movementLeftText.SetText("Steps Left: " + GameManager.Instance.TotalMovement);
    }

    public void UpdateStepsLeftText()
    {
        int movesLeft = GameManager.Instance.TotalMovement - (GameManager.Instance.playerSelectedPath.Count - 1);
        movementLeftText.SetText("Steps Left: " + movesLeft);
    }
}
