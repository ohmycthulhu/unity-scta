using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCASController : IntervalWorkScript
{

    public CollisionDetector _collisionDetector;
    public float _speedStep = .3f;
    public float _heightStep = .5f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Late update is called after all updates are called
    protected override void UpdateAction() {
        List<PossibleCollision> possibleCollisions = _collisionDetector.PossibleCollisions;

        foreach (var collision in possibleCollisions) {
            if (collision.controlMode == ControlMode.TCAS) {
                PerformTCASStep(collision);
            }
        }

    }

    private void PerformTCASStep(PossibleCollision collision) {
        PlaneController first = collision.first;
        PlaneController second = collision.second;

        // It is height adjust
        if (first.Height > second.Height) {
            // If first is flying higher, then it should fly higher
            // And the second one should fly lower

            first.mDesiredHeight = Mathf.Min(first.mDesiredHeight + _heightStep, Globals.maxHeight);
            second.mDesiredHeight = Mathf.Max(second.mDesiredHeight - _heightStep, Globals.minHeight);
        } else {
            // Otherwise the second should fly higher
            second.mDesiredHeight = Mathf.Min(second.mDesiredHeight + _heightStep, Globals.maxHeight);
            first.mDesiredHeight = Mathf.Max(first.mDesiredHeight - _heightStep, Globals.minHeight);
        }

    }
}
