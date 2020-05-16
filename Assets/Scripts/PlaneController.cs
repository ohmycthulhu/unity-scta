using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlaneController : MonoBehaviour
{
    public RectTransform trajectoryLine;
    public PlanePosition planePositions;
    public float mSpeed, mHeightSpeed;
    public float mDesiredHeight;
    private float mCurrentHeight;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = planePositions.source;
        mCurrentHeight = mDesiredHeight;
    }

    // Update is called once per frame
    void Update()
    {
        float step = Time.deltaTime * mSpeed;
        transform.position = Vector3.MoveTowards(transform.position, planePositions.destination, step);
        
        float neededDHeight = mDesiredHeight - mCurrentHeight;
        float speedH = Mathf.Sign(neededDHeight) * mHeightSpeed * Time.deltaTime;
        if (Mathf.Abs(neededDHeight) > Mathf.Abs(speedH)) {
            mCurrentHeight += speedH;
        } else {
            mCurrentHeight += neededDHeight;
        }
        
        trajectoryLine.rotation =  Quaternion.LookRotation(Vector3.RotateTowards(trajectoryLine.forward, planePositions.destination, step, 0.0f));
        trajectoryLine.localScale = new Vector3(3.0f, 3.0f, Mathf.Min((planePositions.destination - transform.position).magnitude / 2.0f, 3.0f));
    }
    
}


[Serializable]
public struct PlanePosition {
    public Vector3 source;
    public Vector3 destination;
}

