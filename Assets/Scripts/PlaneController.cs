using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlaneController : MonoBehaviour
{

    public enum Status {
        Selected,
        STCAControlled,
        TCASControlled,
        NearCollision,
        Normal
    }

    private static Dictionary<Status, Color> colors = new Dictionary<Status, Color>() {
        { Status.Selected, new Color(0, 1, 0, 1) },
        { Status.STCAControlled, new Color(1, 1, 0, 1) },
        { Status.NearCollision, new Color(1, 0, 0, 1) },
        { Status.TCASControlled, new Color(0, 0, 1, 1) },
        { Status.Normal, new Color(1, 1, 1, 1) },
    };

    private static int ID = 1;

    private int myId;

    public RectTransform trajectoryLine;
    public Text informationHolder;
    public PlanePosition planePositions;
    public float mSpeed, mHeightSpeed;
    public float mDesiredHeight;
    private float mCurrentHeight;

    public UIController uiController;

    public bool destroyOnDestination = true;

    public Status currentStatus = Status.Normal;

    private SpriteRenderer mSpriteRenderer;

    PlaneController () {
        // Get id and increase by 1
        this.myId = ID;

        ID++;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Setup intial values
        transform.position = planePositions.source;
        mCurrentHeight = mDesiredHeight;
        mSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (DestroyIfNeeded()) {
            return;
        }
        
        MovePlane();

        ChangeHeight();

        AdjustTrajectoryLine();

        UpdateTexts();

        SetColor();
    }

    // True is returned if plane should be destroyed
    // Method destroys object
    private bool DestroyIfNeeded() {
        // If got to position and need to destroy, then destroy
        if (transform.position == planePositions.destination && destroyOnDestination) {
            Destroy(transform.gameObject);
            return true;
        }
        return false;
    }

    private void MovePlane() {
        float step = Time.deltaTime * mSpeed;

        // Move object toward the destionation and rotate toward it
        transform.position = Vector3.MoveTowards(transform.position, planePositions.destination, step);
    }

    private void ChangeHeight() {
        // Move to minimum between altitude difference and actual travel distance
        float neededDHeight = mDesiredHeight - mCurrentHeight;
        float speedH = Mathf.Sign(neededDHeight) * mHeightSpeed * Time.deltaTime;
        if (Mathf.Abs(neededDHeight) > Mathf.Abs(speedH)) {
            mCurrentHeight += speedH;
        } else {
            mCurrentHeight += neededDHeight;
        }
    }

    private void AdjustTrajectoryLine() {
        float step = Time.deltaTime * mSpeed;
        trajectoryLine.rotation = Quaternion.LookRotation(Vector3.RotateTowards(trajectoryLine.forward, planePositions.destination, step, 0.0f));        
        
        // Change the scale of trajectory line to match the distance to the destination 
        trajectoryLine.localScale = new Vector3(3.0f, 3.0f, Mathf.Min((planePositions.destination - transform.position).magnitude / 2.0f, 3.0f));
    }

    private void UpdateTexts() {
        // Update information on text
        informationHolder.text = $"{Name}\nHeight: {Height}";
    }

    private void SetColor() {
        Color c = colors[currentStatus];
        mSpriteRenderer.color = c;
        informationHolder.color = c;
    }    

    public String Name {
        get {
            return $"Plane #{this.myId}";
        }
    }

    public float Height {
        get {
            return mCurrentHeight;
        }
    }

    void OnMouseDown() {
        if (uiController) {
            if (uiController.SelectedPlane == this) {
                uiController.SelectedPlane = null;
            } else {
                uiController.SelectedPlane = this;
            }
        }
    }
}


[Serializable]
public struct PlanePosition {
    public Vector3 source;
    public Vector3 destination;
}
