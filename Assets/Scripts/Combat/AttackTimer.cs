using System.Collections;
using UnityEngine;

public class AttackTimer : MonoBehaviour
{
    [SerializeField] public float StartTime;
    [ReadOnly] public float TimeLeft;
    [ReadOnly] public float TimeLeftSeconds;
    public bool paused { get; private set; } = false;

    public AttackTimerController attackTimerController { get; set; }

    void Update()
    {
        if (paused)
            return;

        if (TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
            calculateSeconds(TimeLeft);
        }
        else
        {
            HandleAttack();
            TimeLeft = StartTime;
        }
    }

    private void calculateSeconds(float timeLeft)
    {
        timeLeft += 1;
        float seconds = Mathf.FloorToInt(timeLeft % 60);
        TimeLeftSeconds = seconds;
    }

    public void SetStartTime(float StartTime)
    {
        this.StartTime = StartTime;
        TimeLeft = StartTime;
        if (attackTimerController != null)
            attackTimerController.RefreshTimerVisual();
    }

    public void toggleTimer(bool toggle)
    {
        enabled = toggle;
        TimeLeft = StartTime;
    }

    private void HandleAttack()
    {
        if (TryGetComponent(out Hero hero))
        {
            hero.Attack();
            CombatManager.Instance.onHeroAttackEndEvent();
        }
        else
        {
            GetComponent<Enemy>().Attack();
        }
    }

    private IEnumerator PauseAttackTimerCR(float seconds)
    {
        paused = true;
        yield return new WaitForSeconds(seconds);
        paused = false;
    }

    public WaitForSeconds PauseAttackTimer(float seconds)
    {
        StartCoroutine(PauseAttackTimerCR(seconds));
        return new WaitForSeconds(seconds);
    }
}
