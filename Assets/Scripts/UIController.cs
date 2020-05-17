using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{

    private PlaneController _selectedPlane;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            }
            Debug.Log($"You selected - {(value != null ? value.Name : "nothing")}");
        }
    }    
}
