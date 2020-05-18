using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IntervalWorkScript : MonoBehaviour
{
    public float _workStep = .5f;
    private float _lastWorkTime = 0;

    // Update is called once per frame
    void Update()
    {
        float currentTime = Time.time;

        if (currentTime - _lastWorkTime < _workStep) {
            return;
        }

        _lastWorkTime = currentTime;
        UpdateAction();
    }

    protected abstract void UpdateAction();
}
