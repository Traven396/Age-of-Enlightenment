using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chargeable : MonoBehaviour
{
    private float currentCharge = 0;
    private bool autoCharge = false;

    [SerializeField]
    private float maxCharge = 300f;

    private void Update()
    {
        if (autoCharge)
        {
            currentCharge += Time.deltaTime;
        }
    }
    public void ResetCharge()
    {
        currentCharge = 0;
    }
    public float GetCurrentCharge()
    {
        return Mathf.Min(currentCharge, maxCharge);
    }

    public void StartCharging()
    {
        autoCharge = true;
    }
    public void StopCharging()
    {
        autoCharge = false;
    }
}
