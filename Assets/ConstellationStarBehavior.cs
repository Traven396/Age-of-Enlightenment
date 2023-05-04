using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ConstellationStarBehavior : MonoBehaviour
{
    [SerializeField]
    public bool partOfPuzzle = true;
    [SerializeField]
    private Vector2[] desiredPos;



    private ConstellationPuzzleHead puzzleHead;
    private ConstellationSocketCoords currentSocket;

    private List<ConstellationStarBehavior> siblings = new List<ConstellationStarBehavior>();
    private int currentPuzzleConfig = -69;

    private bool isValid = false;
    private bool allFather = false;

    private void Start()
    {
        SetupSiblings();
        if (!partOfPuzzle)
            isValid = true;
    }

    private void SetupSiblings()
    {
        var protoSiblings = transform.parent.GetComponentsInChildren<ConstellationStarBehavior>();

        siblings.AddRange(protoSiblings);

        siblings.Remove(this);
    }
    private void InformSiblings(int theNews)
    {
        foreach (ConstellationStarBehavior star in siblings)
        {
            star.SetCurrentConfig(theNews);
        }
    }
    public void SelectEvent(SelectEnterEventArgs args)
    {
        if(args.interactorObject is XRSocketInteractor)
        {
            if (partOfPuzzle)
            {
                currentSocket = args.interactorObject.transform.GetComponent<ConstellationSocketCoords>();
                CheckPosition(); 
            }
            else
            {
                isValid = false;
            }
        }
        else
        {
            if (partOfPuzzle)
            {
                isValid = false;
                if (allFather)
                {
                    allFather = false;
                    currentPuzzleConfig = -69;
                    InformSiblings(-69);
                } 
            }
            else
            {
                isValid = true;
            }
        }
        puzzleHead.CheckPuzzleSolve();
    }
    
    private void CheckPosition()
    {
        if (currentPuzzleConfig == -69)
        {
            for (int i = 0; i < desiredPos.Length; i++)
            {
                if(desiredPos[i] == new Vector2(currentSocket.xPos, currentSocket.yPos))
                {
                    isValid = true;
                    allFather = true;

                    currentPuzzleConfig = i;
                    InformSiblings(currentPuzzleConfig);
                    break;
                }
                isValid = false;
            } 
        }
        else
        {
            if(currentSocket.xPos == desiredPos[currentPuzzleConfig].x && currentSocket.yPos == desiredPos[currentPuzzleConfig].y)
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }
        }
    }
    public void SetPuzzleHead(ConstellationPuzzleHead cph)
    {
        puzzleHead = cph;
    }

    public bool CheckValid()
    {
        return isValid;
    }
    public void SetCurrentConfig(int newConfig)
    {
        currentPuzzleConfig = newConfig;
    }
}
