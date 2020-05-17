using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatorController : MonoBehaviour
{
    public Camera viewCamera;
    public GameObject planePrefab;
    
    public UIController uiController;

    public PlanePosition[] planes;

    public int planesCount = 0;

    public float timeBetweenGeneration = 1.0f;

    private float lastGenerationTime = 0.0f;

    private float cameraLeftBound, cameraRightBound, cameraTopBound, cameraBottomBound;

    // Start is called before the first frame update
    void Start()
    {       
        foreach (var planePosition in planes) {
            var plane = Instantiate(planePrefab, planePosition.source, Quaternion.identity);
            var c = plane.GetComponent<PlaneController>();
            c.planePositions = planePosition;
            c.mSpeed = Random.Range(1.0f, 3.0f);
            c.uiController = uiController;
            plane.transform.parent = this.transform;
        }

        Vector3 p = viewCamera.ViewportToWorldPoint(viewCamera.transform.position);
        cameraRightBound = Mathf.Abs(p.x);
        cameraLeftBound = -Mathf.Abs(p.x);
        cameraTopBound = Mathf.Abs(p.y);
        cameraBottomBound = -Mathf.Abs(p.y);


        for (int i = 0; i < planesCount; i++) {
            GeneratePlane();
        }

        StartCoroutine("GeneratePlanes");
    }

    private Vector3 GenerateRandomPosition() {
        return new Vector3(Random.Range(cameraLeftBound, cameraRightBound), Random.Range(cameraBottomBound, cameraTopBound));
    }

    public void GeneratePlane() {
        PlanePosition position = new PlanePosition {
            source = GenerateRandomPosition(),
            destination = GenerateRandomPosition()
        };
        float speed = Random.Range(5.0f, 20.0f);
        var plane = Instantiate(planePrefab, Vector3.zero, Quaternion.identity);
        var controller = plane.GetComponent<PlaneController>();
        controller.planePositions = position;
        controller.mSpeed = speed;
        controller.uiController = uiController;
        plane.transform.parent = this.transform;
    }

    IEnumerator GeneratePlanes() {
        while(true) {
            yield return new WaitForSeconds(timeBetweenGeneration);
            GeneratePlane();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
