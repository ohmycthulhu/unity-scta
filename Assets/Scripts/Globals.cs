using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    public const float minHorSpeed = 0.5f;
    public const float maxHorSpeed = 10.0f;

    public const float minVerSpeed = .7f;
    public const float maxVerSpeed = 2.0f;

    public const float minHeight = 1.0f;
    public const float maxHeight = 15.0f;

    public const float TCASSwitchTime = 4.0f;

    public const float minSafeDistance = 10.0f;
    public const float minSafeHeightDifference = 5.0f;

    public static Dictionary<ControlMode, PlaneController.Status> ControlModeToStatus = new Dictionary<ControlMode, PlaneController.Status>() {
        { ControlMode.None, PlaneController.Status.NearCollision },
        { ControlMode.STCA, PlaneController.Status.STCAControlled },
        { ControlMode.TCAS, PlaneController.Status.TCASControlled },
    };
    
    public static float FormatNumber(float f) {
        return Mathf.Round(f * 100) / 100.0f;
    }

    public static float ConvertToUniversal(float distance) {
        // Convert distance to match height distance and add height
        return distance / minSafeDistance * minSafeHeightDifference;
    }

    public static float GetUniversalDistance(float distance, float height) {
        return Mathf.Sqrt(Mathf.Pow(ConvertToUniversal(distance), 2) + Mathf.Pow(height, 2));
    }
}
