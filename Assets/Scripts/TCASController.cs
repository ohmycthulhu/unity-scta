using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCASController : MonoBehaviour
{

    public CollisionDetector _collisionDetector;
    public float _timeStep = .4f;
    public float _speedStep = .3f;
    public float _heightStep = .5f;
    
    private float _lastUpdateTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Late update is called after all updates are called
    void LateUpdate() {
        float currentTime = Time.time;
        if (currentTime - _lastUpdateTime < _timeStep) {
            return;
        }
        _lastUpdateTime = currentTime;

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
