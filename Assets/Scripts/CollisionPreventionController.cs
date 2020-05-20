using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionPreventionController : IntervalWorkScript
{
    public SCTAController _scta;
    public TCASController _tcas;
    public CollisionDetector _CollisionDetector;

    // Update is called once per frame
    protected override void UpdateAction()
    {
        // Get all collisions
        List<PossibleCollision> collisions = _CollisionDetector.PossibleCollisions;

        foreach (var collision in collisions) {
            // Determine the right control mode
            collision.controlMode = GetControlModeForCollision(collision);
        }

        _CollisionDetector.SetCollisionsList(collisions);
    }

    ControlMode GetControlModeForCollision(PossibleCollision collision) {
        // If collision is not already in TCAS mode and time limit exceeds, then switch to TCAS mode
        if (collision.controlMode != ControlMode.TCAS && ShouldSwitchToTCAS(collision)) {
            return ControlMode.TCAS;
        }

        return collision.controlMode;
    }

    bool ShouldSwitchToTCAS(PossibleCollision collision) {
        return (Time.time - collision.startTime) >= Globals.TCASSwitchTime;
    }
}
