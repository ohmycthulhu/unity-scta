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
        if (_camera.isActiveAndEnabled && _selectedPlane == null) {
            Disable();
            return;
        }
        if (_selectedPlane != null) {
            // Vector3 p = _selectedPlane.transform.position;
            Vector3 currentPos = _selectedPlane.transform.position;
            Vector3 move = _planeOffset * _selectedPlane.transform.up;
            Vector3 p = currentPos + move;

            float angle = _selectedPlane.planePositions.TurnAngle;
            // float dX = _planeOffset * Mathf.Sin(angle), dY = _planeOffset * Mathf.Cos(angle);
            // transform.position = new Vector3(p.x + dX, p.y + dY, transform.position.z);
            transform.position = new Vector3(p.x, p.y, transform.position.z);
            transform.rotation = _selectedPlane.transform.rotation;
        }
    }

    public void Enable(PlaneController plane) {
        SelectedPlane = plane;
        _tcasCanvas.enabled = true;
        Camera.main.enabled = false;
        _camera.enabled = true;
        PlaneController.ShowInfo = false;
    }

    public void Disable() {
        if (Camera.main != null) {
            Camera.main.enabled = false;
        }
        _camera.enabled = false;
        _sctaCamera.enabled = true;
        SelectedPlane = null;
        _tcasCanvas.enabled = false;
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
