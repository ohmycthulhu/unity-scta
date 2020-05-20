using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TCASCameraController : MonoBehaviour
{
    private static PlaneController _selectedPlane;

    public Camera _sctaCamera;

    public Button _sctaCameraButton;

    public Canvas _tcasCanvas;

    private Camera _camera;

    public float _planeOffset = 8.0f;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        _sctaCameraButton.onClick.AddListener(Disable);
    }

    // Update is called once per frame
    void Update()
    {
        // If the camera is in TCAS mode, but it shouldn't be, then disable the camera
        if (_camera.isActiveAndEnabled && _selectedPlane == null) {
            Disable();
            return;
        }
        if (_selectedPlane != null) {
            // Move and rotate camera toward the plane
            // And add move little too much, so plane will be in the start 
            Vector3 currentPos = _selectedPlane.transform.position;
            Vector3 move = _planeOffset * _selectedPlane.transform.up;
            Vector3 p = currentPos + move;

            float angle = _selectedPlane.planePositions.TurnAngle;
            transform.position = new Vector3(p.x, p.y, transform.position.z);
            transform.rotation = _selectedPlane.transform.rotation;
        }
    }

    public void Enable(PlaneController plane) {
        // Assign plane, disable all cameras and enable the needed one
        SelectedPlane = plane;
        _tcasCanvas.enabled = true;
        Camera.main.enabled = false;
        _camera.enabled = true;
        // Also switch planes to TCAS radar mode
        PlaneController.ShowInfo = false;
    }

    public void Disable() {
        // Disable current camera and enable SCTA camera
        if (Camera.main != null) {
            Camera.main.enabled = false;
        }
        _camera.enabled = false;
        _sctaCamera.enabled = true;
        SelectedPlane = null;
        _tcasCanvas.enabled = false;

        // Switch planes back to normal state
        PlaneController.ShowInfo = true;
    }

    public static PlaneController SelectedPlane {
        get { return _selectedPlane; }
        set {
            if (_selectedPlane != null) {
                _selectedPlane.IsFocused = false;
            }

            _selectedPlane = value;

            if (value != null) {
                _selectedPlane.IsFocused = true;
            }
        }
    }
}
