using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ConstellationPedestalBehavior : MonoBehaviour
{
    public GameObject socketVisuals;
    public GameObject domeVisuals;
    public Image solutionImage;

    private GameObject currentSpawnedPuzzle;
    private PuzzleCard currentCard;
    public UnityEvent<PuzzleCard> puzzleLoading, puzzleUnloading;
    public void SelectEvent(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.gameObject.TryGetComponent(out PuzzleCard card))
        {
            currentCard = card;

            StopCoroutine(nameof(DomeDespawn));

            if (!domeVisuals.activeInHierarchy)
            {
                StartCoroutine(nameof(DomeSpawn));
            }
            else
            {
                RevealComponents();
            }
        }
    }
    public void DeselectEvent(SelectExitEventArgs args)
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(nameof(DomeDespawn)); 
        }
    }

    private void RevealComponents()
    {
        socketVisuals.SetActive(true);

        if (currentCard.GetPuzzle())
            puzzleLoading.Invoke(currentCard);

        solutionImage.sprite = currentCard.GetSolution();
        solutionImage.gameObject.SetActive(true);
        iTween.FadeFrom(solutionImage.gameObject, 0, 1f);
    }
    private void HideComponents()
    {
        puzzleUnloading.Invoke(null);

        socketVisuals.SetActive(false);

        solutionImage.sprite = null;
        solutionImage.gameObject.SetActive(false);
    }
    IEnumerator DomeSpawn()
    {
        StopCoroutine(nameof(DomeDespawn));

        domeVisuals.SetActive(true);
        iTween.ScaleTo(domeVisuals, Vector3.one, 2f);

        yield return new WaitForSeconds(2);

        RevealComponents();
        yield return null;
    }
    IEnumerator DomeDespawn()
    {
        StopCoroutine(nameof(DomeSpawn));

        HideComponents();

        yield return new WaitForSeconds(10f);

        iTween.ScaleTo(domeVisuals, Vector3.zero, 2f);

        yield return new WaitForSeconds(2);

        domeVisuals.SetActive(false);
        yield return null;
    }

    public void HideMenu()
    {
        solutionImage.gameObject.SetActive(false);
    }
}
