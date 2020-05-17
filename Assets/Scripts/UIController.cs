using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    private PlaneController _selectedPlane;
    
    public Text _planeInfomation;
    public Canvas _planeInformationCanvas;

    public Slider _heightControlSlider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _planeInformationCanvas.enabled = _selectedPlane != null;
        if (_selectedPlane != null) {
            _planeInfomation.text = 
                $"{_selectedPlane.Name}\n" +
                $"Current Height: {_selectedPlane.Height}\n" +
                $"Target Height: {_selectedPlane.TargetHeight}";
            _selectedPlane.TargetHeight = _heightControlSlider.value;
        }
    }

    public PlaneController SelectedPlane {
        get { return _selectedPlane; }
        set {
            if (_selectedPlane != null) {
                _selectedPlane.currentStatus = PlaneController.Status.Normal;
            }
            _selectedPlane = value;

            if (_selectedPlane != null) {
                _selectedPlane.currentStatus = PlaneController.Status.Selected;
                _heightControlSlider.value = _selectedPlane.Height;
            }
        }
    }    
}
