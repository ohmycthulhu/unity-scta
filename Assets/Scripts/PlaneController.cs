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
        { Status.Selected, Color.green },
        { Status.STCAControlled, Color.magenta },
        { Status.NearCollision, Color.red },
        { Status.TCASControlled, Color.blue },
        { Status.Normal, Color.white },
    };

    private static string[] availableNames = {
        "THY",
        "VV",
        "AHY"
    };

    private static int ID = 1;

    private int myId;
    private string _name;

    public bool _isSelected = false, _isFocused = false;

    private static bool _showInformation = true;

    public LineRenderer trajectoryLine;
    public Text informationHolder;
    public PlanePosition planePositions;
    public SCTAController _sctaController;

    public Sprite _planeSCTASprite, _tcasFocusedSprite, _tcasCommonSprite;

    public float mSpeed, mVerticalSpeed;
    public float mDesiredHeight;
    private float mCurrentHeight;

    public float _lineMaxLength = 6.0f;

    public Vector3 _sctaScale = new Vector3(3, 3, 3);
    public Vector3 _tcasFocusedScale = new Vector3(3, 3, 3);
    public Vector3 _tcasScale = new Vector3(1, 1, 1);

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
        
        if (ShouldEnableSCTA()) {
            EnableSCTA();
        }

        MovePlane();

        ChangeHeight();

        AdjustTrajectoryLine();

        UpdateSprite();
        AdjustScale();

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

        if (_isFocused) {
            // Rotate plane
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, -planePositions.TurnAngle);
        } else {
            // Disable rotation
            transform.rotation = Quaternion.identity;
        }

    }

    private void AdjustScale() {
        if (_showInformation) {
            transform.localScale = _sctaScale;
        } else {
            if (_isFocused) {
                transform.localScale = _tcasFocusedScale;
            }
            else {
                transform.localScale = _tcasScale;
            }
        }
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
        Vector3 lineEnd = (_isSelected || relativePosition.magnitude < _lineMaxLength)
            ? relativePosition : (relativePosition.normalized * _lineMaxLength);

        // Set end position
        trajectoryLine.SetPosition(1, lineEnd / 2);

        trajectoryLine.enabled = _showInformation;
    }

    private void EnableSCTA() {
        _sctaController.TakeControlOfCollision(this);
    }

    private bool ShouldEnableSCTA() {
        return IsSelected && currentStatus == Status.NearCollision;
    }

    private void UpdateTexts() {
        if (_showInformation) {
            // Update information on text
            informationHolder.text = $"{Name}\nHeight: {Height}";
        } else {
            informationHolder.text = "";
        }
    }

    private string GetRandomName() {
        string code = availableNames[Random.Range(0, availableNames.Length)];
        int number = Random.Range(100, 999);

        return $"{code} {number}";
    }

    private void UpdateSprite() {
        if (_showInformation) {
            mSpriteRenderer.sprite = _planeSCTASprite;
        } else {
            if (_isFocused) {
                mSpriteRenderer.sprite = _tcasFocusedSprite;
            } else {
                mSpriteRenderer.sprite = _tcasCommonSprite;
            }
        }
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
            if (_isFocused) {
                return Color.yellow;
            }
            if (_isSelected) {
                return Color.green;
            }
            return colors[currentStatus];
        }
    }


    public float Height {
        get {
            return Globals.FormatNumber(mCurrentHeight);
        }
    }
    public float TargetHeight {
        get {
            return Globals.FormatNumber(mDesiredHeight);
        }
        set {
            mDesiredHeight = value;
        }
    }

    public float Speed {
        get {
            return Globals.FormatNumber(mSpeed);
        }
        set {
            mSpeed = value;
        }
    }

    public bool IsSelected {
        get { return _isSelected; }
        set { 
            _isSelected = value;
            // If plane is in near-collision mode and user is selecting it,
            if (value && ShouldEnableSCTA()) {
                EnableSCTA();
            }
            if (!value) {
                _sctaController.ClearSelection();
            }
            // Then it should go to SCTA control mode
        }
    }

    public static bool ShowInfo {
        get { return _showInformation;  }
        set { _showInformation = value;}
    }

    public bool IsFocused {
        get { return _isFocused; }
        set { _isFocused = value; }
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
public class PlanePosition {
    public Vector3 source;
    public Vector3 destination;

    public float TurnAngle {
        get {
            return Vector3.Angle(new Vector3(0.0f, 1.0f), destination - source);
        }
    }
}
