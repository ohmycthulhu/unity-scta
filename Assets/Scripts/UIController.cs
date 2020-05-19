using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{    
    public UIPlaneController _controller;

    public Button _toTCASButton;

    public TCASCameraController _tcasCamera;

    private bool shouldUpdateSliderValues = false;

    // Start is called before the first frame update
    void Start()
    {
        _controller.SetHeightLimits(Globals.minHeight, Globals.maxHeight);

        _controller.SetSpeedLimits(Globals.minHorSpeed, Globals.maxHorSpeed);

        _toTCASButton.onClick.AddListener(GoToTCAS);
    }

    // Update is called once per frame
    void Update()
    {
        bool shouldDoSomething = _controller.Plane != null
            && _controller.Plane.currentStatus == PlaneController.Status.Normal;
        _controller.canvas.enabled = shouldDoSomething;
        if (shouldDoSomething) {
            if (shouldUpdateSliderValues) {
                shouldUpdateSliderValues = false;
                _controller.UpdateSliderValues();
            }
            SetInformation();
            _controller.ReadAndUpdateValue();
        } else {
            shouldUpdateSliderValues = true;
        }
    }

    void SetInformation() {
        _controller.UpdateTexts();
    
        if (_controller.Plane.currentStatus == PlaneController.Status.Normal) {
            _controller.planeStatus.text = "Normal Status";
            _controller.planeStatus.color = Color.white;
        } else {
            _controller.planeStatus.text = "Possible Collision!";
            _controller.planeStatus.color = Color.red;
        }        
    }

    void GoToTCAS() {
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

[System.Serializable]
public class UIPlaneController {
    [HideInInspector]
    private PlaneController plane;
    public Text planeName;
    public Text planeHeight;
    public Text planeSpeed;
    public Text planeStatus;
    public Slider speedControlSlider;
    public Slider heightControlSlider;
    public Canvas canvas;

    public PlaneController Plane {
        get { return plane; }
        set {
            plane = value;
            if (value != null) {
                UpdateSliderValues();
            }
        }
    }

    public void UpdateSliderValues() {
        if (plane != null) {
            speedControlSlider.value = plane.Speed;
            heightControlSlider.value = plane.TargetHeight;
        }
    }

    public void UpdateTexts() {
        if (plane == null) {
            return;
        }
        planeName.text = plane.Name;
        planeHeight.text =
            $"Current Height: {plane.Height}\n" +
            $"Target Height: {plane.TargetHeight}\n";
        planeSpeed.text = $"Speed: {plane.Speed}";
    }

    public void ReadAndUpdateValue() {
        if (plane == null) {
            return;
        }
        plane.TargetHeight = heightControlSlider.value;
        plane.Speed = speedControlSlider.value;
    }

    public void SetSpeedLimits(float min, float max) {
        speedControlSlider.minValue = Globals.minHorSpeed;
        speedControlSlider.maxValue = Globals.maxHorSpeed;
    }
    public void SetHeightLimits(float min, float max) {
        heightControlSlider.minValue  = Globals.minHeight;
        heightControlSlider.maxValue = Globals.maxHeight;
    }
}
