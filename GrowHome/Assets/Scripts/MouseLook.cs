using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSens = 100f;
    public Transform target;
    public float dstFromTarget = 6f;
    public Vector2 pitchMinMax = new Vector2(-40, 85);

    public float rotationSmoothTime = .12f;
    
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    float pitch;
    float yaw;

    public int fovZoom = 20;
    public int fovNormal = 60; //60 is default fov
    public float zoomSmoothTime = 5f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // when in game, lock cursor to center and make it invisible
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, fovZoom, Time.deltaTime * zoomSmoothTime);
        }
        else
        {
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, fovNormal, Time.deltaTime * zoomSmoothTime);

        }
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime; //get mouse x position
        pitch -= Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime; // get mouse y position
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y); // clamp the camera so you rotate around the top or bottom of your character

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime); // Smooths your cursor over time (adjust rotationSmoothTime in Unity editor to change)

        //Vector3 targetRotation = new Vector3(pitch, yaw);
        transform.eulerAngles = currentRotation; // take the smoothed rotation and move it to the transform of the current object (in this case Camera)

        transform.position = target.position - transform.forward * dstFromTarget; // Offset the camera to be rotaing around a central Target.Position (editable in Editor)

    }
}
