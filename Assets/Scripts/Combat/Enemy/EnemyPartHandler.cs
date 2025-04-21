using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPartHandler : MonoBehaviour
{
    public Enemy enemy;
    public Weakpoint Weakpoint { get => _weakpoint; private set { _weakpoint = value; } }
    private Weakpoint _weakpoint;
    private bool hasWeakpoint;
    [SerializeField] private Collider2D partCollider;
    private bool partDisabled = false;

    private void Awake()
    {
        hasWeakpoint = TryGetComponent(out _weakpoint);
    }

    private void OnMouseDown()
    {
        if(GameManager.Instance.battleTutorialEnabled || partDisabled)
            return;

        enemy.HandleMouseDownOnPart(this);
    }

    public void TogglePartTargeting(bool toggle)
    {
        partCollider.enabled = toggle;
        partDisabled = toggle;
    }
}
