using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargettingIndicator : MonoBehaviour
{
    public GameObject indicatorVisuals;
    private GameObject spawnedIndicator;

    public void Cast(Vector3 target, bool DestroyIndic)
    {
        if (!spawnedIndicator)
        {
            spawnedIndicator = Instantiate(indicatorVisuals);
        }
        if (!DestroyIndic)
        {
            spawnedIndicator.transform.position = target;
        }
        else
        {
            Destroy(spawnedIndicator);
        }
    }

    public Transform GetCurrentTarget()
    {
        return spawnedIndicator.transform;
    }
}
