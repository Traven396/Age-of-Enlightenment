using System;
using UnityEngine;
using UnityEngine.Events;

public class ConstellationPuzzleHead : MonoBehaviour
{
    private ConstellationStarBehavior[] childStars;
    public bool PuzzleSolved = false;

    public UnityEvent puzzleSolved, puzzleUnsolved;
    // Start is called before the first frame update
    void Awake()
    {
        childStars = GetComponentsInChildren<ConstellationStarBehavior>();
        foreach (ConstellationStarBehavior star in childStars)
        {
            star.SetPuzzleHead(this);
        }
    }
    public void CheckPuzzleSolve()
    {
        foreach (ConstellationStarBehavior star in childStars)
        {
            if (star.partOfPuzzle)
            {
                if (!star.CheckValid())
                {
                    PuzzleSolved = false;
                    puzzleUnsolved?.Invoke();
                    break;
                }
                else
                {
                    puzzleSolved?.Invoke();
                    PuzzleSolved = true;
                } 
            }
        }
    }
}
