using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class is used to store constants that define simulation speed and behaviour
public class Globals : MonoBehaviour
{
    public const float minHorSpeed = 1f;
    public const float maxHorSpeed = 10.0f;

    public const float minVerSpeed = .7f;
    public const float maxVerSpeed = 2.0f;

    public const float minHeight = 1.0f;
    public const float maxHeight = 15.0f;

    public const float TCASSwitchTime = 4.0f;

    public const float minSafeDistance = 10.0f;
    public const float minSafeHeightDifference = 5.0f;

    // Dictionary for matching ControlMode with PlaneController.Status
    public static Dictionary<ControlMode, PlaneController.Status> ControlModeToStatus = new Dictionary<ControlMode, PlaneController.Status>() {
        { ControlMode.None, PlaneController.Status.NearCollision },
        { ControlMode.STCA, PlaneController.Status.STCAControlled },
        { ControlMode.TCAS, PlaneController.Status.TCASControlled },
    };
    
    public static float FormatNumber(float f) {
        return Mathf.Round(f * 100) / 100.0f;
    }

    public static float ConvertToUniversal(float distance) {
        // Convert distance to match height distance
        return distance / minSafeDistance * minSafeHeightDifference;
    }

    public static float GetUniversalDistance(float distance, float height) {
        // Convert distance to universal and find distance in universal unit
        float distanceInUniversal = ConvertToUniversal(distance);
        return Mathf.Sqrt(Mathf.Pow(distanceInUniversal, 2) + Mathf.Pow(height, 2));
    }

    public static float GetHeightDifference(float height) {
        // If camera is in TCAS mode, return difference between heights
        if (TCASCameraController.SelectedPlane != null) {
            return height - TCASCameraController.SelectedPlane.Height;
        }
        return height;
    }

    public static float GetTurnAngle() {
        // If camera is in TCAS mode, return turn angle of plane
        if (TCASCameraController.SelectedPlane != null) {
            return -TCASCameraController.SelectedPlane.planePositions.TurnAngle;
        }
        return 0;
    }
}
