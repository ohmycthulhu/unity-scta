﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : IntervalWorkScript
{
    [Tooltip("Error Canvas with text to display on Collisions")]
    public Canvas _errorMessagesCanvas;
    private List<PossibleCollision> _possibleCollisions = new List<PossibleCollision>();

    // Start is called before the first frame update
    void Start()
    {
        // Disable error canvas
        _errorMessagesCanvas.enabled = false;
    }

    // Update is called once per frame
    protected override void UpdateAction()
    {
        DetectCollisions();
    }

    void DetectCollisions() {   
        // Get all planes
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Plane");

        List<PossibleCollision> collisions = new List<PossibleCollision>();
        // Compare all planes and check whether has collisions
        // Using for instead of foreach to reduce computing time twice
        for (int i = 0; i < objects.Length; i++) {
            PlaneController p1 = objects[i].GetComponent<PlaneController>();
            for (int j = i + 1; j < objects.Length; j++) {
                PlaneController p2 = objects[j].GetComponent<PlaneController>();
                if (!isDistanceSafe(p1, p2)) {
                    collisions.Add(
                        new PossibleCollision() {
                            first = p1,
                            second = p2,
                            startTime = Time.time,
                            controlMode = ControlMode.None
                        }
                    );
                }
            }
        }

        // Update collisions list
        UpdateCollisionsList(collisions);

        // If needed, show error canvas
        _errorMessagesCanvas.enabled = ShouldShowErrors();
    }

    private bool ShouldShowErrors() {
        // If there is a collision that is not under any control, show error
        foreach (var collision in _possibleCollisions) {
            if (collision.controlMode == ControlMode.None) {
                return true;
            }
        }
        return false;
    }

    private void UpdateCollisionsList(List<PossibleCollision> collisions) {
        List<PossibleCollision> newCollisionsList = new List<PossibleCollision>();
        // For every collision
        foreach (var collision in collisions) {
            // Check if it is already in collection
            PossibleCollision match = getExistingCollision(collision);
            newCollisionsList.Add(match != null ? match : collision);
        }

        SetCollisionsList(newCollisionsList, true);
    }

    private void ClearCollisionsList() {
        foreach (var collision in _possibleCollisions) {
            // Return planes in Normal status before removing from collisions list
            if (collision.first != null) {
                collision.first.currentStatus = PlaneController.Status.Normal;
            }
            if (collision.second != null) {
                collision.second.currentStatus = PlaneController.Status.Normal;
            }
        }
        _possibleCollisions.Clear();
    }

    public void SetCollisionsList(List<PossibleCollision> collisions, bool clear = false) {
        if (clear) {
            ClearCollisionsList();
        }
        
        _possibleCollisions = collisions;
        
        // Update every plane's status
        foreach (var collision in _possibleCollisions) {
            UpdatePlaneStates(collision);
        }
    }

    private void UpdatePlaneStates(PossibleCollision c) {
        PlaneController.Status status = c.ControlModeToStatus();
        c.first.currentStatus = status;
        c.second.currentStatus = status;
    }

    public PossibleCollision FindAndSetToSCTAMode (PossibleCollision collision) {
        // Iterate over all collisions, if there is collision between presented planes
        // Set mode to STCA and return the result
        for (int i = 0; i < _possibleCollisions.Count; i++) {
            if (_possibleCollisions[i].first == collision.first
            && _possibleCollisions[i].second == collision.second) {
                _possibleCollisions[i].SetControlMode(ControlMode.STCA);
                UpdatePlaneStates(_possibleCollisions[i]);
                return _possibleCollisions[i];
            }
        }
        return null;
    }

    public PossibleCollision RemoveSTCAMode (PossibleCollision collision) {
        if (collision == null) {
            return null;
        }

        // Check if there is collision with the presented planes and in STCA mode
        // And return it no None mode
        for (int i = 0; i < _possibleCollisions.Count; i++) {
            if (_possibleCollisions[i].first == collision.first
            && _possibleCollisions[i].second == collision.second
            && _possibleCollisions[i].controlMode == ControlMode.STCA
            ) {
                _possibleCollisions[i].SetControlMode(ControlMode.None);
                UpdatePlaneStates(_possibleCollisions[i]);
                return _possibleCollisions[i];
            }
        }
        return null;
    }

    public PossibleCollision FindCollision(PlaneController plane) {
        foreach (PossibleCollision p in _possibleCollisions) {
            if (p.first == plane || p.second == plane) {
                return p;
            }
        }
        return null;
    }

    private PossibleCollision getExistingCollision(PossibleCollision collision) {
        foreach(var c in _possibleCollisions) {
            if (c.first == collision.first && c.second == collision.second) {
                return c;
            }
        }
        return null;
    }

    bool isDistanceSafe(PlaneController p1, PlaneController p2) {
        // If distance between planes, without height, is more than minimal, then it is safe
        float distance = Vector3.Distance(p1.transform.position, p2.transform.position);
        if (distance >= Globals.minSafeDistance) {
            return true;
        }

        // Normalize and check the values
        float heightDiff = Mathf.Abs(p1.Height - p2.Height);
        return (new Vector2(heightDiff / Globals.minSafeHeightDifference, distance / Globals.minSafeDistance)).magnitude >= 1.0f;
    }

    public List<PossibleCollision> PossibleCollisions {
        get { return _possibleCollisions; }
    }

    public bool IsCollisionValid(PossibleCollision collision) {
        if (collision == null) {
            return false;
        }
        // Check if collision exists in collection
        foreach (var c in _possibleCollisions) {
            // If exists, check the validity of presented mode
            if (c.first == collision.first && c.second == collision.second) {
                return c.controlMode == collision.controlMode;
            }
        }
        return false;
    }
}

public enum ControlMode {
    TCAS, // This mode is used for automated adjustments
    STCA, // This mode is used to manuaaly control,
    None  // If plane is not controlled yet
}

[System.Serializable]
public class PossibleCollision{
    public PlaneController first;
    
    public PlaneController second;
    
    public float startTime;

    public ControlMode controlMode;

    public void SetControlMode (ControlMode mode) {
        PlaneController.Status planeStatus = Globals.ControlModeToStatus[mode];
        first.currentStatus = planeStatus;
        second.currentStatus = planeStatus;
        controlMode = mode;
    }

    // Function that converts current Control Mode to PlaneController.Status
    public PlaneController.Status ControlModeToStatus() {
        if (first.currentStatus == PlaneController.Status.STCAControlled
            || second.currentStatus == PlaneController.Status.STCAControlled
        ) {
            return PlaneController.Status.STCAControlled;
        }
        switch (controlMode) {
            case ControlMode.TCAS:
                return PlaneController.Status.TCASControlled;
            case ControlMode.STCA:
                return PlaneController.Status.STCAControlled;
            default:
                return PlaneController.Status.NearCollision;
        }
    }

    // The distance between planes
    public float Distance {
        get {
            if (first == null || second == null) {
                return 0;
            }
            float heightDiffrence = second.Height - first.Height;
            float flatDistance = Vector3.Distance(first.transform.position, second.transform.position);
            return Globals.GetUniversalDistance(flatDistance, heightDiffrence);
        }
    }
}
