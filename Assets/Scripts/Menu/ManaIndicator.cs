using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ManaIndicator : MonoBehaviour
{
    public Image manaFill;
    public TMP_Text manaValue;

    private void OnEnable()
    {
        PlayerStats.ManaChanged += UpdateGraphic;
        PlayerStats.MaxManaChanged += UpdateGraphic;
        PlayerStats.ManaChanged += UpdateText;
        PlayerStats.MaxManaChanged += UpdateText;


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
        PlayerStats.ManaChanged -= UpdateText;
        PlayerStats.MaxManaChanged -= UpdateText;
    }

    void UpdateGraphic()
    {

        if (PlayerSingleton.Instance._Stats._CurrentMana != 0 && PlayerSingleton.Instance._Stats.MaximumManaValue.Value != 0)
        {
            float currentFill = (float)PlayerSingleton.Instance._Stats._CurrentMana / (float)PlayerSingleton.Instance._Stats.MaximumManaValue.Value;
            //iTween.ScaleTo(manaFill, new Vector3(currentFill, currentFill), .5f);
            manaFill.fillAmount = currentFill; 
        }
    }
    void UpdateText()
    {
        manaValue.text = PlayerSingleton.Instance._Stats._CurrentMana + "/" + PlayerSingleton.Instance._Stats.MaximumManaValue.Value;
    }
}
