using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTargetIndicator : MonoBehaviour
{
    private GameObject spawnedIndicator;
    private void Start()
    {
        spawnedIndicator = new GameObject("Indicator");
    }

    public Transform MoveIndicator(Vector3 position)
    {
        spawnedIndicator.transform.position = position;
        return spawnedIndicator.transform;
    }

    public Transform GetCurrentTran()
    {
        return spawnedIndicator.transform;
    }

    public void ToggleIndicator(bool show)
    {
        spawnedIndicator.SetActive(show);
    }
}
