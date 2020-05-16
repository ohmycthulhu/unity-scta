using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatorController : MonoBehaviour
{
    public Camera viewCamera;
    public GameObject planePrefab;
    
    public PlanePosition[] planes;

    public int planesCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
        foreach (var planePosition in planes) {
            var plane = Instantiate(planePrefab, planePosition.source, Quaternion.identity);
            var c = plane.GetComponent<PlaneController>();
            c.planePositions = planePosition;
            c.mSpeed = Random.Range(1.0f, 3.0f);
        }


        Debug.Log(viewCamera.orthographicSize);

        for (int i = 0; i < planesCount; i++) {
            PlanePosition position = new PlanePosition {
                source = new Vector3(Random.Range(-20.0f, 20.0f), Random.Range(-10.0f, 10.0f)),
                destination = new Vector3(Random.Range(-20.0f, 20.0f), Random.Range(-10.0f, 10.0f))
            };
            float speed = Random.Range(1.0f, 2.0f);
            var plane = Instantiate(planePrefab, Vector3.zero, Quaternion.identity);
            var controller = plane.GetComponent<PlaneController>();
            controller.planePositions = position;
            controller.mSpeed = speed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
