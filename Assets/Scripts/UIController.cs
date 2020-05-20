using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{    
    public UIPlaneController _controller;

    // Button to switch TCAS mode  
    public Button _toTCASButton;

    public TCASCameraController _tcasCamera;

    // Indicates whether slider values should be updated
    private bool shouldUpdateSliderValues = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set limits and event listeners
        _controller.SetHeightLimits(Globals.minHeight, Globals.maxHeight);

        _controller.SetSpeedLimits(Globals.minHorSpeed, Globals.maxHorSpeed);

        _toTCASButton.onClick.AddListener(GoToTCAS);
    }

    // Update is called once per frame
    void Update()
    {
        // If there is selected plane and it is in Normal status 
        bool shouldDoSomething = _controller.Plane != null
            && _controller.Plane.currentStatus == PlaneController.Status.Normal;
        _controller._canvas.enabled = shouldDoSomething;
        if (shouldDoSomething) {
            // Update slider values if necessary
            if (shouldUpdateSliderValues) {
                shouldUpdateSliderValues = false;
                _controller.UpdateSliderValues();
            }
            SetInformation();
            _controller.ReadAndUpdateValue();
        } else {
            // If canvas is displayed, other scripts can alter the height
            // not changing slider values will cause unexpected errors
            shouldUpdateSliderValues = true;
        }
    }

    void SetInformation() {
        _controller.UpdateTexts();
        
        // Set status for planes 
        if (_controller.Plane.currentStatus == PlaneController.Status.Normal) {
            _controller._planeStatus.text = "Normal Status";
            _controller._planeStatus.color = Color.white;
        } else {
            _controller._planeStatus.text = "Possible Collision!";
            _controller._planeStatus.color = Color.red;
        }        
    }

    void GoToTCAS() {
        // Enable TCAS
        if (_controller.Plane == null) {
            return;
        }
        _tcasCamera.Enable(_controller.Plane);
    }

    public PlaneController SelectedPlane {
        get { return _controller.Plane; }
        set {
            if (_controller.Plane != null) {
                _controller.Plane.IsSelected = false;
            }

            _controller.Plane = value;

            if (value != null) {
                _controller.Plane.IsSelected = true;
            }
        }
    }
}

// Class that encapsulates interaction between user and canvas
// Used to control plane by sliders and displaying information
[System.Serializable]
public class UIPlaneController {
    [HideInInspector]
    private PlaneController _plane;
    public Text _planeName;
    public Text _planeHeight;
    public Text _planeSpeed;
    public Text _planeStatus;
    public Slider _speedControlSlider;
    public Slider _heightControlSlider;
    public Canvas _canvas;

    public PlaneController Plane {
        get { return _plane; }
        set {
            _plane = value;
            if (value != null) {
                UpdateSliderValues();
            }
        }
    }

    // Read and set slider values
    public void UpdateSliderValues() {
        if (_plane != null) {
            _speedControlSlider.value = _plane.Speed;
            _heightControlSlider.value = _plane.TargetHeight;
        }
    }

    // Set texts
    public void UpdateTexts() {
        if (_plane == null) {
            return;
        }
        _planeName.text = _plane.Name;
        _planeHeight.text =
            $"Current Height: {_plane.Height}\n" +
            $"Target Height: {_plane.TargetHeight}\n";
        _planeSpeed.text = $"Speed: {_plane.Speed}";
    }

    public void ReadAndUpdateValue() {
        if (_plane == null) {
            return;
        }
        _plane.TargetHeight = _heightControlSlider.value;
        _plane.Speed = _speedControlSlider.value;
    }

    // Set limits on speed slider
    public void SetSpeedLimits(float min, float max) {
        _speedControlSlider.minValue = Globals.minHorSpeed;
        _speedControlSlider.maxValue = Globals.maxHorSpeed;
    }

    // Set limits on height slider
    public void SetHeightLimits(float min, float max) {
        _heightControlSlider.minValue  = Globals.minHeight;
        _heightControlSlider.maxValue = Globals.maxHeight;
    }
}
