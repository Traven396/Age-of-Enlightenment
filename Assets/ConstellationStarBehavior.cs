using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ConstellationStarBehavior : MonoBehaviour
{
    private ConstellationPuzzleHead puzzleHead;
    private ConstellationSocketCoords currentSocket;
    [SerializeField]
    public bool partOfPuzzle = true;
    [SerializeField]
    private int desiredXPos, desiredYPos;

    public bool isValid = false;
    public void SelectEvent(SelectEnterEventArgs args)
    {
        if(args.interactorObject is XRSocketInteractor)
        {
            currentSocket = args.interactorObject.transform.GetComponent<ConstellationSocketCoords>();
            CheckPosition();
        }
        else
        {
            isValid = false;   
        }
        puzzleHead.CheckPuzzleSolve();
    }
    public bool CheckValid()
    {
        return isValid;
    }
    private void CheckPosition()
    {
        if(currentSocket.xPos == desiredXPos && currentSocket.yPos == desiredYPos)
        {
            isValid = true;
        }
        else
        {
            isValid = false;
        }
    }
    public void SetPuzzleHead(ConstellationPuzzleHead cph)
    {
        puzzleHead = cph;
    }
}
