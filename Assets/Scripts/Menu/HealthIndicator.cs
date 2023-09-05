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
        Player.HealthChanged += UpdateGraphic;
        Player.HealthChanged += UpdateText;


    }
    private void Start()
    {
        UpdateGraphic();
        UpdateText();
    }
    private void OnDisable()
    {
        Player.ManaChanged -= UpdateGraphic;
        Player.ManaChanged -= UpdateText;
    }

    void UpdateGraphic()
    {
        float currentFill = (float)Player.Instance.currentHealth / (float)Player.Instance.maximumHealth.Value;
        iTween.ScaleTo(healthFill, new Vector3(currentFill, currentFill), .5f);
        //manaFill.transform.localScale = new Vector3(currentFill, currentFill);
    }
    void UpdateText()
    {
        healthValue.text = Player.Instance.currentHealth + "/" + Player.Instance.maximumHealth.Value;
    }
}
