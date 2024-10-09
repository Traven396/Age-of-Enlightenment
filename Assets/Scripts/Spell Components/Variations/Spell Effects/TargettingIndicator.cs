using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargettingIndicator : MonoBehaviour
{
    [Header("Normal Calculation Stuff")]
    public bool requiresNormal = false;
    public Vector3 normalVector;
    [HideInInspector]
    public bool readyToCast = true;

    private GameObject spellCircle;
    private Transform circleHolder;
    private SpellVisualsManager _visualsManager;
    private LeftRight currentHand;

    private bool moveCircle = true;

    public void SetupReferences(SpellBlueprint spell)
    {
        spellCircle = spell._SpellCircle;
        circleHolder = spell._CircleHolderTransform;
        _visualsManager = spell._VisualsManager;
        currentHand = spell.currentHand;
    }
    //Method called on loop, moving the spell circle to the new RaycastHit position
    public void TargetMove(RaycastHit currHit)
    {
        //Check to see if the confirm button is already pressed. 
        //Basically just a way to pass the gripPressed boolean over to here
        if (moveCircle)
        {
            //If this component has Normals required it checks to see if the object normal hit is correct
            //If it is, it continues like normal. If NOT then it breaks out of the method, and says its not a valid cast
            if (requiresNormal)
            {
                if (currHit.normal != normalVector)
                {
                    TargetReturn();
                    return;
                }
            }

            if (currHit.point != Vector3.zero)
            {
                //Calculate the rotation of the normal hit, this way it works independent of what kind of normals
                var rotationToBe = Quaternion.FromToRotation(spellCircle.transform.up, currHit.normal) * spellCircle.transform.rotation;

                //Remove the parent of the circle so that it doesnt move with the players hand
                spellCircle.transform.parent = null;

                //The moving and rotation of the spell circle for visuals
                iTween.MoveUpdate(spellCircle, currHit.point + (currHit.normal * 0.03f), .5f);
                iTween.RotateUpdate(spellCircle, rotationToBe.eulerAngles, .5f);

                //Tell the program everything is ok to cast
                readyToCast = true;
            }
            else
            {
                TargetReturn();
            } 
        }
    }
    public void TargetReturn()
    {
        spellCircle.transform.parent = circleHolder.transform;

        iTween.ScaleTo(spellCircle, Vector3.one, .6f);

        _visualsManager.ReturnCircleToHolder(currentHand);

        readyToCast = false;
    }
    public void ConfirmLocation()
    {
        if (Vector3.Distance(spellCircle.transform.position, circleHolder.transform.position) > .05)
        {
            moveCircle = false;

            readyToCast = true;

            iTween.ScaleTo(spellCircle, Vector3.one * 4, 1f); 
        }
    }
    public void UnconfirmLocation()
    {
        moveCircle = true;
        readyToCast = false;

        iTween.ScaleTo(spellCircle, Vector3.one, .6f);
    }
    public void ConfirmButtonMove(RaycastHit hit)
    {
        if(hit.point != Vector3.zero)
        {
            var finalRotation = Quaternion.FromToRotation(spellCircle.transform.up, hit.normal) * spellCircle.transform.rotation;

            iTween.MoveTo(spellCircle, (hit.point + (hit.normal * 0.03f)), .2f);

            iTween.RotateTo(spellCircle, finalRotation.eulerAngles, .5f);

            iTween.ScaleTo(spellCircle, Vector3.one * 4, 1f);
            readyToCast = true;
        }
    }
}
