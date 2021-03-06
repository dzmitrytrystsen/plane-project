using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] planePrefabs;
    public Canvas canvas;
    public Camera previewCamera;

    public bool IsCanvasActive { set => canvas.enabled = value; }
    public enum PreviewType { Up, Down, Face, Left };

    private int currentPlaneIndex = 0;
    public GameObject currentPlane;
    public GameObject spaceship;
    private GameObject currentPlanePartToFollow;

    private Vector3 movePlanePosition = new Vector3(20f, 0f, 0f);
    private Vector3 startPosition;
    private float transitionSpeed = 20f;

    private bool inPlaneTransition;
    private PreviewType previewType = PreviewType.Up;

    private Touch touch;
    private Quaternion touchRotation;
    private float rotationSpeed = 0.1f;

    private Vector3 previewCameraDefaultPosition = new Vector3(0f, 12f, 0f);
    private Vector3 previewCameraDefaultRotation = new Vector3(90, 0, 0);

    void Start()
    {
        startPosition = planePrefabs[0].transform.position;
        spaceship = planePrefabs[1];

        CreatePlane(currentPlaneIndex, startPosition);
        currentPlanePartToFollow = currentPlane;
    }

    void Update()
    {
        ChangePlane();
        RenderSettings.skybox.SetFloat("_Rotation", 1f * Time.time);

        RotatePlaneByTouch();
        PositionPreviewCamera();
    }

    private void CreatePlane(int planeIndex, Vector3 planePosition)
    {
        if(planeIndex + 1 > planePrefabs.Length)
        {
            currentPlaneIndex = 0;
        }
        else if(planeIndex < 0)
        {
            currentPlaneIndex = planePrefabs.Length - 1;
        }

        currentPlane = Instantiate(planePrefabs[currentPlaneIndex], planePosition, planePrefabs[currentPlaneIndex].transform.rotation);

        if(previewType == PreviewType.Up || previewType == PreviewType.Down)
        {
            currentPlanePartToFollow = currentPlane;
        }
        else if (previewType == PreviewType.Face)
        {
            currentPlanePartToFollow = currentPlane.transform.GetChild(0).gameObject;
        }
        else
        {
            currentPlanePartToFollow = currentPlane.transform.GetChild(1).gameObject;
        }
    }

    private void ChangePlane()
    {
        if (inPlaneTransition)
        {
            currentPlane.transform.position = Vector3.MoveTowards(currentPlane.transform.position, movePlanePosition, transitionSpeed * Time.deltaTime);
            if (Vector3.Distance(currentPlane.transform.position, movePlanePosition) < 0.1f)
            {
                Destroy(currentPlane);
                CreatePlane(currentPlaneIndex, startPosition);

                inPlaneTransition = false;
            }
        }
        else
        {
            currentPlane.transform.position = Vector3.MoveTowards(currentPlane.transform.position,
                new Vector3(0f, 0f, planePrefabs[currentPlaneIndex].transform.position.z), transitionSpeed * Time.deltaTime);
            if (Vector3.Distance(currentPlane.transform.position, new Vector3(0f, 0f, planePrefabs[currentPlaneIndex].transform.position.z)) < 0.1f)
            {
                IsCanvasActive = true;
            }
        }
    }

    private void PositionPreviewCamera()
    {
        previewCamera.transform.position = currentPlanePartToFollow.transform.position + previewCameraDefaultPosition;
        previewCamera.transform.eulerAngles = currentPlanePartToFollow.transform.eulerAngles + previewCameraDefaultRotation;
    }

    // Rotation logic
    private void RotatePlaneByTouch()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                touchRotation = Quaternion.Euler(0f, -touch.deltaPosition.x * rotationSpeed, 0f);
                currentPlane.transform.rotation *= touchRotation;
            }
        }
    }

    // Buttons logic

    // Prev/Next button
    public void SelectNextPlane()
    {
        currentPlaneIndex++;
        movePlanePosition = new Vector3(20f, 0f, 0f);
        startPosition = -movePlanePosition;

        IsCanvasActive = false;
        inPlaneTransition = true;
    }

    public void SelectPrevPlane()
    {
        currentPlaneIndex--;
        movePlanePosition = new Vector3(-20f, 0f, 0f);
        startPosition = -movePlanePosition;

        IsCanvasActive = false;
        inPlaneTransition = true;
    }

    // Change color buttons
    public void ChangePlaneColorToRed()
    {
        currentPlane.GetComponent<Renderer>().material.color = Color.red;
    }

    public void ChangePlaneColorToBlue()
    {
        currentPlane.GetComponent<Renderer>().material.color = Color.blue;
    }

    public void ChangePlaneColorToYellow()
    {
        currentPlane.GetComponent<Renderer>().material.color = Color.yellow;
    }

    // Change preview buttons
    public void ChangePreviewToUp()
    {
        previewType = PreviewType.Up;
        currentPlanePartToFollow = currentPlane;
        previewCameraDefaultPosition = new Vector3(0f, 12f, 0f);
        previewCameraDefaultRotation = new Vector3(90, 0, 0);
    }

    public void ChangePreviewToDown()
    {
        previewType = PreviewType.Down;
        currentPlanePartToFollow = currentPlane;
        previewCameraDefaultPosition = new Vector3(0f, -12f, 0f);
        previewCameraDefaultRotation = new Vector3(270, 180, 0);
    }

    public void ChangePreviewToFace()
    {
        previewType = PreviewType.Face;
        currentPlanePartToFollow = currentPlane.transform.GetChild(0).gameObject;
        previewCameraDefaultPosition = new Vector3(0f, 0f, 0f);
        previewCameraDefaultRotation = new Vector3(180, 0, 0);

    }

    public void ChangePreviewToLeft()
    {
        previewType = PreviewType.Left;
        currentPlanePartToFollow = currentPlane.transform.GetChild(1).gameObject;
        previewCameraDefaultPosition = new Vector3(0f, 0f, 0f);
        previewCameraDefaultRotation = new Vector3(0, 90, 0);
    }

    // Exit button
    public void ExitApplication()
    {
        Application.Quit();
    }
}
