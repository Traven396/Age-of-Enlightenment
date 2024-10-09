using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthIndicator : MonoBehaviour
{
    public GameObject healthFill;
    public TMP_Text healthValue;

    private void OnEnable()
    {
        PlayerStats.HealthChanged += UpdateGraphic;
        PlayerStats.HealthChanged += UpdateText;


    }
    private void Start()
    {
        UpdateGraphic();
        UpdateText();
    }
    private void OnDisable()
    {
        PlayerStats.ManaChanged -= UpdateGraphic;
        PlayerStats.ManaChanged -= UpdateText;
    }

    void UpdateGraphic()
    {
        float currentFill = (float)PlayerSingleton.Instance._Stats._CurrentHealth / (float)PlayerSingleton.Instance._Stats.MaximumHealthValue.Value;
        iTween.ScaleTo(healthFill, new Vector3(currentFill, currentFill), .5f);
        //manaFill.transform.localScale = new Vector3(currentFill, currentFill);
    }
    void UpdateText()
    {
        healthValue.text = PlayerSingleton.Instance._Stats._CurrentHealth + "/" + PlayerSingleton.Instance._Stats.MaximumHealthValue.Value;
    }
}
