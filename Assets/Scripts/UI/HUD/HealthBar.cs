using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI tmp;

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

    public void updateHealthBar(Health.HealthChangeData data)
    {
        slider.value = (float)data.currentHealth / data.maxHealth;
        tmp.SetText("{0} / {1}", data.currentHealth, data.maxHealth);
    }
}
