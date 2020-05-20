using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// IntervalWorkScript is class used to decreate the Update calls to certain time
// Update can be called a lot of times per second, but this class allows to set minimal time step
// It is very useful to decrease the amount of expensive computation
public abstract class IntervalWorkScript : MonoBehaviour
{
    // The minimal time between calls
    public float _workStep = .5f;
    
    // Last time of call
    private float _lastWorkTime = 0;

    // Update is called once per frame
    protected virtual void Update()
    {
        // Check the difference between current and last time, and minimal work step 
        float currentTime = Time.time;

        if (currentTime - _lastWorkTime < _workStep) {
            return;
        }

        _lastWorkTime = currentTime;
        UpdateAction();
    }

    // Method should be overriden in every implementation
    protected abstract void UpdateAction();
}
