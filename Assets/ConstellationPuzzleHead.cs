using System;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections;

public class ConstellationPuzzleHead : MonoBehaviour
{
    public ConstellationStarBehavior[] childStars;
    public bool currentPuzzleSolved = false;

    private PuzzleCard currentPuzzleCard;
    private GameObject currentPuzzle;

    public UnityEvent<FullSpellObject> puzzleSolved;
    public void CheckPuzzleSolve()
    {
        currentPuzzleSolved = childStars.All(star => star.CheckValid());
        if (currentPuzzleSolved)
        {
            Debug.Log("Solved");
            puzzleSolved?.Invoke(currentPuzzleCard.GetSpell());
        }
    }
    public void PuzzleLoad(PuzzleCard Puzzle)
    {
        currentPuzzleCard = Puzzle;
        currentPuzzle = Instantiate(Puzzle.GetPuzzle(), transform);

        childStars = currentPuzzle.GetComponentsInChildren<ConstellationStarBehavior>();
        foreach (ConstellationStarBehavior star in childStars)
        {
            star.SetPuzzleHead(this);
            star.gameObject.SetActive(false);
        }
        StartCoroutine(nameof(CoolShowcase));
    }
    public void PuzzleUnload(PuzzleCard NotNeccessaryLmao)
    {
        if (currentPuzzle)
        {
            Destroy(currentPuzzle);
            StopCoroutine(nameof(CoolShowcase));
        }

        currentPuzzleCard = null;

        Array.Clear(childStars, 0 , childStars.Length);
        currentPuzzleSolved = false;
    }

    IEnumerator CoolShowcase()
    {
        foreach(ConstellationStarBehavior star in childStars)
        {
            star.gameObject.SetActive(true);
            yield return new WaitForSeconds(.1f);
        }
        yield return null;
    }
}
