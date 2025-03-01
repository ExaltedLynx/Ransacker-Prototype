using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private int MaxHealth;
    [SerializeField][ReadOnly] private int currentHealth;
    [ReadOnly] public HealthBar healthBar;

    public struct HealthChangeData
    {
        public int maxHealth { get; set; }
        public int currentHealth { get; set; }
        public int amountChanged { get; set; }
        public ChangeType changeType { get; set; }

        public void setData(int maxHealth, int currentHealth, int amountChanged, ChangeType type)
        {
            this.maxHealth = maxHealth;
            this.currentHealth = currentHealth;
            this.amountChanged = amountChanged;
            changeType = type;
        }

        public void setData(int currentHealth, int amountChanged, ChangeType type)
        {
            this.currentHealth = currentHealth;
            this.amountChanged = amountChanged;
            changeType = type;
        }

        public void setData(int maxHealth)
        {
            this.maxHealth = maxHealth;
        }
    }

    public enum ChangeType
    {
        damage,
        heal,
        other
    }

    public HealthChangeData healthChangeData;

    public OnHealthChangedEvent onHealthChanged;

    public OnDeathEvent onDeathEvent;


    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void subtract(int damageTaken)
    {
        currentHealth -= damageTaken;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            onDeathEvent.Invoke();
        }
        healthChangeData.setData(currentHealth, damageTaken, ChangeType.damage);
        onHealthChanged.Invoke(healthChangeData);
    }
    public void add(int healthIncreased)
    {

        currentHealth += healthIncreased;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
        healthChangeData.setData(currentHealth, healthIncreased, ChangeType.heal);
        onHealthChanged.Invoke(healthChangeData);
    }

    public int getCurrentHealth()
    {
        return currentHealth;
    }

    public void addMaxHealth(int x)
    {
        MaxHealth += x;
        healthChangeData.setData(MaxHealth);
        onHealthChanged.Invoke(healthChangeData);
    }

    public void setMaxHealth(int MaxHealth)
    {
        this.MaxHealth = MaxHealth;
        healthChangeData.setData(MaxHealth);
        onHealthChanged.Invoke(healthChangeData);
    }

    public int GetMaxHealth()
    {
        return MaxHealth;
    }    

    public void setCurrentHealth(int currentHealth)
    {
        this.currentHealth = currentHealth;
        if (currentHealth <= 0)
        {
            onDeathEvent.Invoke();
        }

        else
        {
            healthChangeData.setData(this.currentHealth, currentHealth, ChangeType.other);
            onHealthChanged.Invoke(healthChangeData);
        }
    }

    public void InitHealthBar()
    {
        currentHealth = MaxHealth;
        healthChangeData.setData(MaxHealth, currentHealth, 0, ChangeType.other);
        onHealthChanged.AddListener(healthBar.updateHealthBar);
        onHealthChanged.Invoke(healthChangeData);
    }
}

[System.Serializable]
public class OnHealthChangedEvent : UnityEvent<Health.HealthChangeData> { }

[System.Serializable]
public class OnDeathEvent : UnityEvent { }
