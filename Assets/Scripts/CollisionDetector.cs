using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{

    public float maxSafeDistance = 10.0f;
    public float minSafeHeightDifference = 5.0f;

    public float detectionTimePeriod = .1f;

    private float lastDetectionTime = 0;

    private List<PossibleCollision> _possibleCollisions = new List<PossibleCollision>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectCollisions();
    }

    void DetectCollisions() {
        // Collision detection is quite expensive operation, so we reduce the amount of time it is calculated
        float currentTime = Time.time;
        if (currentTime - lastDetectionTime < detectionTimePeriod) {
            return;
        }
        lastDetectionTime = currentTime;
        
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
                            controlMode = ControlMode.TCAS
                        }
                    );
                }
            }
        }

        // Log
        // foreach (var p in collisions) {
        //  //   Debug.Log($"Collision posibility! {p.first.Name} - {p.second.Name}");
        // }

        UpdateCollisionsList(collisions);
    }

    private void UpdateCollisionsList(List<PossibleCollision> collisions) {
        List<PossibleCollision> newCollisionsList = new List<PossibleCollision>();
        // For every collision
        foreach (var collision in collisions) {
            // Check if it is already in collection
            PossibleCollision match = getExistingCollision(collision);
            
            newCollisionsList.Add(match != null ? match : collision);
        }

        ClearCollisionsList();
        SetCollisionsList(newCollisionsList);
    }

    private void ClearCollisionsList() {
        foreach (var collision in _possibleCollisions) {
            if (collision.first != null) {
                collision.first.currentStatus = PlaneController.Status.Normal;
            }
            if (collision.second != null) {
                collision.second.currentStatus = PlaneController.Status.Normal;
            }
        }
    }

    private void SetCollisionsList(List<PossibleCollision> collisions) {
        _possibleCollisions = collisions;
        
        foreach (var collision in _possibleCollisions) {
            PlaneController.Status status = collision.controlMode == ControlMode.TCAS ? PlaneController.Status.TCASControlled : PlaneController.Status.STCAControlled;
            collision.first.currentStatus = status;
            collision.second.currentStatus = status;
        }
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
        float distance = Vector3.Distance(p1.transform.position, p2.transform.position);
        if (distance >= maxSafeDistance) {
            return true;
        }

        float heightDiff = Mathf.Abs(p1.Height - p2.Height);

        return (new Vector2(heightDiff / minSafeHeightDifference, distance / maxSafeDistance)).magnitude >= 1.0f;
    }

    public List<PossibleCollision> PossibleCollisions {
        get { return _possibleCollisions; }
    } 
}

public enum ControlMode {
    TCAS, // This mode is used for automated adjustments
    STCA  // This mode is used to manuaaly control 
}

[System.Serializable]
public class PossibleCollision{
    public PlaneController first;
    
    public PlaneController second;
    
    public float startTime;

    public ControlMode controlMode;
}
