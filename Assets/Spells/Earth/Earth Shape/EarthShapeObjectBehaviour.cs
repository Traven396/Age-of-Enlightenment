using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthShapeObjectBehaviour : MonoBehaviour
{

    [Space(10)]
    public Transform _mainPart;
    [Space(10)]
    [Range(0.01f, 0.2f)]
    public float maxChangeAmount = .09f;


    private float lastHeight;
    private Vector3 startingScale;

    private void Start()
    {
        startingScale = _mainPart.localScale;
    }

    public void RotateTowardsPoint(Vector3 point, Vector3 otherUp)
    {
        transform.LookAt(point, otherUp);
    }

    public void ChangeHeight(float amount)
    {
        //Clamp the height change between a range decided by maxChangeAmount
        //This way the player shouldn't fall through the pillar because it moves too fast. 
        float clampedAmount = Mathf.Clamp(amount, lastHeight - maxChangeAmount, lastHeight + maxChangeAmount);

        _mainPart.position = transform.position + (transform.up * (clampedAmount / 2));
        _mainPart.localScale = new Vector3(_mainPart.localScale.x, startingScale.y + clampedAmount, _mainPart.localScale.z);
        

        lastHeight = clampedAmount;
    }

    public void ChangeMaterial(Color newColor)
    {
        _mainPart.GetComponent<Renderer>().material.color = newColor;
    }

}
