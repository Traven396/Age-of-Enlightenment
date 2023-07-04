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
        Player.ManaChanged += UpdateGraphic;
        Player.ManaChanged += UpdateText;

        
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
        float currentFill = (float)Player.Instance.currentMana / (float)Player.Instance.maximumMana;
        //iTween.ScaleTo(manaFill, new Vector3(currentFill, currentFill), .5f);
        manaFill.fillAmount = currentFill;
    }
    void UpdateText()
    {
        manaValue.text = Player.Instance.currentMana + "/" + Player.Instance.maximumMana;
    }
}
