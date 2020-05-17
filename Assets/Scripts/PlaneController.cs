using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private static string[] availableNames = {
        "THY",
        "VV",
        "AHY"
    };

    private static int ID = 1;

    private int myId;
    private string _name;

    public LineRenderer trajectoryLine;
    public Text informationHolder;
    public PlanePosition planePositions;
    public float mSpeed, mVerticalSpeed;
    public float mDesiredHeight;
    private float mCurrentHeight;

    public float _lineMaxLength = 6.0f;

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
        _name = GetRandomName();
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

        // Move object toward the destionation
        transform.position = Vector3.MoveTowards(transform.position, planePositions.destination, step);
    }

    private void ChangeHeight() {
        // Move to minimum between altitude difference and actual travel distance
        float neededDHeight = mDesiredHeight - mCurrentHeight;
        float speedH = Mathf.Sign(neededDHeight) * mVerticalSpeed * Time.deltaTime;
        if (Mathf.Abs(neededDHeight) > Mathf.Abs(speedH)) {
            mCurrentHeight += speedH;
        } else {
            mCurrentHeight += neededDHeight;
        }
    }

    private void AdjustTrajectoryLine() {
        // Get destination position relative to current position
        Vector3 relativePosition = planePositions.destination - transform.position;
        
        // Set end of to the destination if length is lower than length max or if object is selected
        Vector3 lineEnd = (currentStatus == Status.Selected || relativePosition.magnitude < _lineMaxLength)
            ? relativePosition : (relativePosition.normalized * _lineMaxLength);

        // Set end position
        trajectoryLine.SetPosition(1, lineEnd / 2);
    }

    private void UpdateTexts() {
        // Update information on text
        informationHolder.text = $"{Name}\nHeight: {Height}";
    }

    private string GetRandomName() {
        string code = availableNames[Random.Range(0, availableNames.Length)];
        int number = Random.Range(100, 999);

        return $"{code} {number}";
    }

    private void SetColor() {
        mSpriteRenderer.color = CurrentColor;
        informationHolder.color = CurrentColor;
        trajectoryLine.startColor = CurrentColor;
        trajectoryLine.endColor = CurrentColor;
    }    

    public string Name {
        get {
            return _name;
        }
    }

    private Color CurrentColor {
        get {
            return colors[currentStatus];
        }
    }

    public float Height {
        get {
            return Mathf.Round(mCurrentHeight * 100) / 100.0f;
        }
    }
    public float TargetHeight {
        get {
            return Mathf.Round(mDesiredHeight * 100) / 100.0f;
        }
        set {
            mDesiredHeight = value;
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


[System.Serializable]
public struct PlanePosition {
    public Vector3 source;
    public Vector3 destination;
}
