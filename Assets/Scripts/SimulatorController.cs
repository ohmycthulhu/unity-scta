using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simulator controller generates the planes and assings the needed variables
public class SimulatorController : MonoBehaviour
{
    [Tooltip("The main camera")]
    public Camera _viewCamera;

    [Space(4)]

    [Header("Initial planes")]

    [Tooltip("Initial planes count")]
    public int _planesCount = 0;

    [Tooltip("The initial planes")]
    public PlanePosition[] _planes;

    [Space(4)]
    
    [Header("Plane attributes")]
    [Tooltip("Prefab of plane")]
    public GameObject _planePrefab;
    
    [Tooltip("UI Controller")]
    public UIController _uiController;

    [Tooltip("SCTA Controller")]
    public SCTAController _sctaController;


    public float _timeBetweenGeneration = 1.0f;

    private float _lastGenerationTime = 0.0f;

    // Coordinates that limits camera view bound
    private float _cameraLeftBound, _cameraRightBound, _cameraTopBound, _cameraBottomBound;

    // Start is called before the first frame update
    void Start()
    {       

        // Get and assign camera bounds
        Vector3 p = _viewCamera.ViewportToWorldPoint(_viewCamera.transform.position);
        _cameraRightBound = Mathf.Abs(p.x);
        _cameraLeftBound = -Mathf.Abs(p.x);
        _cameraTopBound = Mathf.Abs(p.y);
        _cameraBottomBound = -Mathf.Abs(p.y);

        // Generate plane for every initial position
        foreach (var planePosition in _planes) {
            GeneratePlane(planePosition);
        }

        // Generate the needed count of planes
        for (int i = 0; i < _planesCount; i++) {
            GeneratePlane();
        }

        // Start plane generating
        StartCoroutine("GeneratePlanes");
    }

    public void GeneratePlane(PlanePosition p = null) {
        // Get value if passed, otherwise generate
        PlanePosition position = p != null ? p : GenerateRandomTrack();
        
        // Get random values
        float speed = Random.Range(Globals.minHorSpeed, Globals.maxHorSpeed);
        float verticalSpeed = Random.Range(Globals.minVerSpeed, Globals.maxVerSpeed);
        float height = Random.Range(Globals.minHeight, Globals.maxHeight);

        // Generate plane
        var plane = Instantiate(_planePrefab, Vector3.zero, Quaternion.identity);

        // Get controller and assign values
        var controller = plane.GetComponent<PlaneController>();
        controller.planePositions = position;
        controller.mSpeed = speed;
        controller.mDesiredHeight = height;
        controller.mVerticalSpeed = verticalSpeed;
        controller.uiController = _uiController;
        controller._sctaController = _sctaController;

        // Assign this as parent, so plane will be the child
        plane.transform.parent = this.transform;
    }

    // Method that generates plane every _timeBetweenGeneration secons
    IEnumerator GeneratePlanes() {
        while(true) {
            yield return new WaitForSeconds(_timeBetweenGeneration);
            GeneratePlane();
        }
    }

    Vector3 GetPointInArea(int code) {
        float xMin, xMax;
        float yMin, yMax;

        // The are four limits:
        // 0: Left
        // 1: Right
        // 2: Bottom
        // 3: Top
        // Depending on the code, it generates point in selected limit
        if (code > 1) {
            xMin = _cameraLeftBound;
            xMax = _cameraRightBound;
            if (code == 2) {
                yMax = _cameraBottomBound;
                yMin = _cameraBottomBound * 1.1f;
            } else {
                yMin = _cameraTopBound;
                yMax = _cameraTopBound * 1.1f;
            }
        } else {
            yMin = _cameraBottomBound;
            yMax = _cameraTopBound;
            if (code == 0) {
                xMin = _cameraLeftBound * 1.1f;
                xMax = _cameraLeftBound;
            } else {
                xMin = _cameraRightBound;
                xMax = _cameraRightBound * 1.1f;
            }
        }
        
        return new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
    }

    PlanePosition GenerateRandomTrack() {
        // Get source and destination limit
        int sourcePos = (int)Random.Range(0, 3.99f);
        int destPos = (sourcePos + 1) % 4;
        
        return new PlanePosition{
            source = GetPointInArea(sourcePos),
            destination = GetPointInArea(destPos)
        };
    }
}
