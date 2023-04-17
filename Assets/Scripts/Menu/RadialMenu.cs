using UnityEngine.Events;
using UnityEngine;
using UnityEngine.InputSystem;

public class RadialMenu : MonoBehaviour
{
    public RadialSection top, right, bottom, left;
    public Transform selectionCursor;
    private RadialSection currentSelection;

    [Header("Settings")]
    public float minimumDistance = .1f;
    public UnityEvent NoSelection = new UnityEvent();
    public AudioClip invokeSound;
    [Header("Inputs")]
    public InputActionReference spawnButton;



    private RadialSection[] radialSections;
    private GameObject player;


    private Vector3 currentVector;
    private void Start()
    {
        spawnButton.action.started += ShowMenu;
        spawnButton.action.canceled += HideMenu;

        player = Camera.main.gameObject;

        SetupSections();

        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        spawnButton.action.started -= ShowMenu;
        spawnButton.action.canceled -= HideMenu;
    }

    private void SetupSections()
    {
        radialSections = new RadialSection[4]
        {
            left,
            top,
            right,
            bottom
        };
    }
    private void Update()
    {
        if (Vector3.Distance(transform.position, selectionCursor.position) > minimumDistance)
        {
            #region Calculating Angle and Setting Index
            //currentVector.x = selectionCursor.position.x - transform.position.x;
            //currentVector.y = selectionCursor.position.y - transform.position.y;
            currentVector = selectionCursor.position - transform.position;


            //currentVector.x = Vector3.Project(currentVector, transform.right).magnitude;
            //currentVector = Vector3.ProjectOnPlane(currentVector, transform.forward);
            currentVector = transform.InverseTransformDirection(currentVector);

            currentVector.x = -currentVector.x;


            var ang = Mathf.Atan2(currentVector.y, currentVector.x) * Mathf.Rad2Deg;
            if (ang < 0)
            {
                ang += 360;
            }
            var snapAng = Mathf.RoundToInt(ang / 90);
            if (snapAng == 4)
                snapAng = 0;
            #endregion
            UpdateSelectedItem(snapAng);
        }
        else
        {
            ResetSelectedItem();
            currentSelection = null;
        }
    }
    void InvokeSelection()
    {
        if (currentSelection)
        {
            currentSelection.onSelect.Invoke();
            AudioSource.PlayClipAtPoint(invokeSound, selectionCursor.position, .7f);
        }
        else
        {
            NoSelection?.Invoke();
        }
    }
    #region Visual Stuff
    void UpdateSelectedItem(int index)
    {
        if (currentSelection != radialSections[index])
            ResetSelectedItem();

        currentSelection = radialSections[index];

        if (currentSelection.transform.localScale != Vector3.one * 1.3f)
        {
            currentSelection.transform.localScale = Vector3.one * 1.3f;
        }
    }
    void ResetSelectedItem()
    {
        if (currentSelection)
        {
            currentSelection.transform.localScale = Vector3.one;
        }
    }
    private void HideMenu(InputAction.CallbackContext obj)
    {
        InvokeSelection();
        gameObject.SetActive(false);
        gameObject.transform.localScale = Vector3.one;
    }
    private void ShowMenu(InputAction.CallbackContext obj)
    {
        gameObject.transform.position = selectionCursor.position;

        Vector3 targetRot = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        gameObject.transform.LookAt(targetRot);

        gameObject.SetActive(true);

        iTween.ScaleFrom(gameObject, Vector3.zero, .2f);
    }
    #endregion
    
}
