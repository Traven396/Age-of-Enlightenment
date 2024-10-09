using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GrimoireManager : MonoBehaviour
{
    [Header("Needed Locations")]
    public Transform handPoint, bookPoint;
    [Header("Animation Parameters")]
    public GameObject leftCover;
    public GameObject rightCover;
    [Header("Multiple Tab Manager")]
    public List<GameObject> tabs = new();
    public UnityEvent TabChanged;
    private List<GrimoireTab> tabScripts = new();

    private bool menuCurrentlyActive;

    void Awake()
    {
        menuCurrentlyActive = rightCover.activeInHierarchy;

        tabScripts.AddRange(GetComponentsInChildren<GrimoireTab>());

        tabScripts.ForEach(t => t.Overhead = this);
    }

    public void ActivateTab(GameObject tabParameter)
    {
        tabs.Where(t => t != tabParameter).ToList().ForEach(ta => ta.SetActive(false));

        tabParameter.SetActive(true);

        TabChanged?.Invoke();
    }
    public void ActivateTab(int index)
    {
        var newTab = tabs[index];

        tabs.Where(t => t != newTab).ToList().ForEach(ta => ta.SetActive(false));

        newTab.SetActive(true);

        TabChanged?.Invoke();
    }

    #region Animation Stuff
    private void Update()
    {
        if (menuCurrentlyActive)
        {
            iTween.MoveUpdate(gameObject, handPoint.position, .6f);
            transform.rotation = handPoint.rotation;
        }
    }
    public void BookAnimation()
    {
        if (menuCurrentlyActive)
        {
            //Closing animation
            StartCoroutine(nameof(BookClose));
        }
        else
        {
            iTween.Stop(leftCover);
            iTween.Stop(rightCover);

            //Opening Animation
            StopCoroutine(nameof(BookClose));

            //iTween.MoveTo(gameObject, handPoint.position, 1.2f);

            menuCurrentlyActive = true;

            //gameObject.transform.position = bookPoint.position;

            rightCover.SetActive(true);
            leftCover.SetActive(true);

            iTween.RotateTo(leftCover, iTween.Hash("y", -5, "easetype", iTween.EaseType.linear, "islocal", true));
            iTween.RotateTo(rightCover, iTween.Hash("y", 5, "easetype", iTween.EaseType.linear, "islocal", true));
        }
    }
    IEnumerator BookClose()
    {
        menuCurrentlyActive = false;

        iTween.Stop(leftCover);
        iTween.Stop(rightCover);

        iTween.RotateTo(leftCover, iTween.Hash("y", -90, "easetype", iTween.EaseType.linear, "islocal", true));
        iTween.RotateTo(rightCover, iTween.Hash("y", 90, "easetype", iTween.EaseType.linear, "islocal", true));
        iTween.MoveTo(gameObject, bookPoint.position, 2f);

        yield return new WaitForSeconds(1f);

        rightCover.SetActive(false);
        leftCover.SetActive(false);
        yield return null;
    }
    #endregion
}

public interface GrimoireTab
{
    GrimoireManager Overhead { get; set; }
}
