using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCTAController : IntervalWorkScript
{

    public CollisionDetector _collisionDetector;
    public UIPlaneController _firstPlaneController;
    public UIPlaneController _secondPlaneController;
    private PossibleCollision _possibleCollision;

    // Start is called before the first frame update
    void Start()
    {
        _firstPlaneController.SetHeightLimits(Globals.minHeight, Globals.maxHeight);
        _firstPlaneController.SetSpeedLimits(Globals.minHorSpeed, Globals.maxHorSpeed);

        _secondPlaneController.SetHeightLimits(Globals.minHeight, Globals.maxHeight);
        _secondPlaneController.SetSpeedLimits(Globals.minHorSpeed, Globals.maxHorSpeed);
    }

    protected override void Update() {
        base.Update();
        bool shouldShowPanels = _possibleCollision != null;
        
        _firstPlaneController.canvas.enabled = shouldShowPanels;
        _secondPlaneController.canvas.enabled = shouldShowPanels;

        if (shouldShowPanels) {
            _firstPlaneController.UpdateTexts();
            _secondPlaneController.UpdateTexts();

            _firstPlaneController.ReadAndUpdateValue();
            _secondPlaneController.ReadAndUpdateValue();

            _firstPlaneController.planeStatus.text = $"Distance: {(_possibleCollision.Distance)}";
        }
    }

    // Update is called once per frame
    protected override void UpdateAction()
    {
        if (!IsCollisionValid()) {
            ClearSelection();
            return;
        }
    }

    private bool IsCollisionValid() {
        return _collisionDetector.IsCollisionValid(_possibleCollision);
    }

    public void ClearSelection() {
        if (_possibleCollision == null) {
            return;
        }
        _collisionDetector.RemoveSTCAMode(_possibleCollision);
        _possibleCollision = null;
        _firstPlaneController.Plane = null;
        _secondPlaneController.Plane = null;
    }

    public void TakeControlOfCollision(PossibleCollision collision) {
        // If collision is null or on TCAS control, then do nothing
        if (collision == null || collision.controlMode == ControlMode.TCAS) {
            return;
        }

        ClearSelection();

        // If control mode is None, then go to SCTA mode
        PossibleCollision collisionToSave = collision.controlMode == ControlMode.None ?
            _collisionDetector.FindAndSetToSCTAMode(collision) : collision;

        if (collisionToSave == null) {
            return;
        }

        _firstPlaneController.Plane = collision.first;
        _secondPlaneController.Plane = collision.second;
        _possibleCollision = collision;
    }
    public void TakeControlOfCollision(PlaneController plane) {
        PossibleCollision collision = _collisionDetector.FindCollision(plane);
        if (collision == null) {
            return;
        }
        TakeControlOfCollision(collision);
    }
}
