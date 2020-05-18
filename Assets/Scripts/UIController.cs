using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{    
    public UIPlaneController _controller;

    // Start is called before the first frame update
    void Start()
    {
        _controller.heightControlSlider.minValue  = Globals.minHeight;
        _controller.heightControlSlider.maxValue = Globals.maxHeight;

        _controller.speedControlSlider.minValue = Globals.minHorSpeed;
        _controller.speedControlSlider.maxValue = Globals.maxHorSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        _controller.canvas.enabled = _controller.plane != null;
        if (_controller.plane != null) {
            SetInformation();
            _controller.plane.TargetHeight = _controller.heightControlSlider.value;
            _controller.plane.Speed = _controller.speedControlSlider.value;
        }
    }

    void SetInformation() {
        _controller.planeName.text = _controller.plane.Name;
        _controller.planeHeight.text =
            $"Current Height: {_controller.plane.Height}\n" +
            $"Target Height: {_controller.plane.TargetHeight}\n";
        _controller.planeSpeed.text = $"Speed: {_controller.plane.Speed}";

        if (_controller.plane.currentStatus == PlaneController.Status.Normal) {
            _controller.planeStatus.text = "Normal Status";
            _controller.planeStatus.color = Color.white;
        } else {
            Debug.Log(_controller.plane.currentStatus);
            _controller.planeStatus.text = "Possible Collision!";
            _controller.planeStatus.color = Color.red;
        }        
    }

    public PlaneController SelectedPlane {
        get { return _controller.plane; }
        set {
            if (_controller.plane != null) {
                _controller.plane.IsSelected = false;
            }
            _controller.plane = value;

            if (value != null) {
                _controller.plane.IsSelected = true;
                _controller.heightControlSlider.value = value.Height;
                _controller.speedControlSlider.value = value.Speed;
            }
        }
    }    
}

[System.Serializable]
public struct UIPlaneController {
    [HideInInspector]
    public PlaneController plane;
    public Text planeName;
    public Text planeHeight;
    public Text planeSpeed;
    public Text planeStatus;
    public Slider speedControlSlider;
    public Slider heightControlSlider;
    public Canvas canvas;
}
