using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask grappelable;
    public Transform gunTip, camera, player;
    public float maxDistance = 50f;
    private SpringJoint joint;

    // Start is called before the first frame update
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0)) {
            StartGrapple();
        }

        if (Input.GetMouseButtonUp(0)) {
            EndGrapple();
        }

    }

    void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        RaycastHit hit;

        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, grappelable))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false; //see what happens if this is true
            joint.connectedAnchor = grapplePoint;

            float distanceToGrapplePoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceToGrapplePoint * 0.8f; //make these editable
            joint.minDistance = distanceToGrapplePoint * 0.2f;


            //TODO customize these and see what they do 
            joint.spring = 10f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
        }

    }

    void DrawRope()
    {
        if (!joint) { return; }
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);
    }

    void EndGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }
}
    

