using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionPreventionController : IntervalWorkScript
{

    public float _timeBeforeSwithToTCAS;
    public SCTAController _scta;
    public TCASController _tcas;
    public CollisionDetector _CollisionDetector;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void UpdateAction()
    {
        // Get all collisions
        List<PossibleCollision> collisions = _CollisionDetector.PossibleCollisions;

        foreach (var collision in collisions) {
            // Determine the right control mode
            ControlMode c = GetControlModeForCollision(collision);
            collision.controlMode = c;
        }

        _CollisionDetector.SetCollisionsList(collisions);
    }

    ControlMode GetControlModeForCollision(PossibleCollision collision) {
        if (collision.controlMode == ControlMode.None) {
            return ControlMode.STCA;
        }
        if (collision.controlMode == ControlMode.STCA && ShouldSwitchToTCAS(collision)) {
            return ControlMode.TCAS;
        }

        return collision.controlMode;
    }

    bool ShouldSwitchToTCAS(PossibleCollision collision) {
        return Time.time - collision.startTime >= _timeBeforeSwithToTCAS;
    }
}
