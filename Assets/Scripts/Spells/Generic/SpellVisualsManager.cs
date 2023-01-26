using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellVisualsManager : MonoBehaviour
{
    public GameObject leftCircleHolder;
    public GameObject rightCircleHolder;

    private GameObject currentLeftCircle;
    private GameObject currentRightCircle;

    public void ChangeRightCircle(GameObject circleToBe)
    {
        DespawnCircle(LeftRight.Right);
        if(circleToBe)
            SpawnNewCircle(circleToBe, LeftRight.Right);
    }

    public void ChangeLeftCircle(GameObject circleToBe)
    {
        DespawnCircle(LeftRight.Left);
        if(circleToBe)
            SpawnNewCircle(circleToBe, LeftRight.Left);
    }

    private void DespawnCircle(LeftRight side)
    {
        if (side == 0)
        {
            Destroy(currentLeftCircle);
        }
        else
        {
            Destroy(currentRightCircle);
        }
    }

    private void SpawnNewCircle(GameObject circleToBe, LeftRight side)
    {
        if (side == 0)
        {
            currentLeftCircle = Instantiate(circleToBe, leftCircleHolder.transform);
        }
        else
        {
            currentRightCircle = Instantiate(circleToBe, rightCircleHolder.transform);
        }
    }
}
