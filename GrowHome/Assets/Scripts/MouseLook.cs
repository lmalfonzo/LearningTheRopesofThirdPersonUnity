using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSens = 100f;
    public Transform target;


    public Vector2 pitchMinMax = new Vector2(-40, 85);

    public float rotationSmoothTime = .12f;

    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    float pitch;
    float yaw;

    //This should only be on cameraCollision branch
    public int fovZoom = 20;
    public int fovNormal = 60; //60 is default fov
    public float zoomSmoothTime = 5f;

    // Camera Collision
    public float minDistance = 1.0f;
    public float maxDistance = 4.0f;
    public float smooth = 10.0f;
    public float distance;
    Vector3 dollyDir;
    public Vector3 dollyDirAdjusted;
    public float desiredDistance;
    bool hitCamera = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // when in game, lock cursor to center and make it invisible
    }

    void Awake()
    {
        desiredDistance = transform.localPosition.magnitude;
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, fovZoom, Time.deltaTime * zoomSmoothTime);
        }
        else
        {
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, fovNormal, Time.deltaTime * zoomSmoothTime);
        }

        yaw += Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime; //get mouse x position
        pitch -= Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime; // get mouse y position
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y); // clamp the camera so you rotate around the top or bottom of your character

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime); // Smooths your cursor over time (adjust rotationSmoothTime in Unity editor to change)

        //Vector3 targetRotation = new Vector3(pitch, yaw);
        transform.eulerAngles = currentRotation; // take the smoothed rotation and move it to the transform of the current object (in this case Camera)
        dollyDir = transform.localPosition.normalized; // update the dollyDir every frame to the camera position

        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        // Debug.DrawLine(transform.parent.position, desiredCameraPos); // For debugging line distance 


        desiredDistance = maxDistance;
        if (Physics.Linecast(transform.parent.position, desiredCameraPos, out hit))
        {
            desiredDistance = Mathf.Clamp(hit.distance * .75f, minDistance, maxDistance);
            hitCamera = true;
        }

        if (desiredDistance == maxDistance && hitCamera)
        {
            Debug.Log("CAMERA BACK");
            transform.position = Vector3.Lerp(transform.position, target.position - transform.forward * desiredDistance, Time.deltaTime * smooth);
            Debug.Log((maxDistance - distance));
            if ((maxDistance - distance) < 0.1f) { hitCamera = false; }
        } else if (desiredDistance < maxDistance)
        {
            Debug.Log("CAMERA FRONT");
            transform.position = Vector3.Lerp(transform.position, target.position - transform.forward * desiredDistance, Time.deltaTime * smooth);
        }
        else
        {
            transform.position = target.position - transform.forward * desiredDistance;
        }

        distance = Vector3.Distance(transform.parent.position, transform.position);

    }
}
