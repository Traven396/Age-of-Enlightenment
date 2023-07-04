using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SettingsTab : MonoBehaviour, ITab
{
    public BookMenuManager Overhead { get; set; }
    public GameObject LocomotionSystem;

    public TMPro.TMP_Text turnText;
    
    public void TurnSystemChange()
    {
        var conTurn = LocomotionSystem.GetComponent<ActionBasedContinuousTurnProvider>();
        var snaTurn = LocomotionSystem.GetComponent<ActionBasedSnapTurnProvider>();

        if (conTurn.enabled)
        {
            snaTurn.enabled = true;
            conTurn.enabled = false;

            turnText.text = "<b>Rotation Method:</b> Snap";
        }
        else
        {
            conTurn.enabled = true;
            snaTurn.enabled = false;

            turnText.text = "<b>Rotation Method:</b> Smooth";
        }
    }
    public void TestMethod()
    {
        Debug.Log("BOY OH BOY");
    }
}
