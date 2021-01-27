using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreGrabbedCollisions : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<OVRGrabbable>() && collision.gameObject.GetComponent<OVRGrabbable>().isGrabbed)
        {
            foreach(Collider col in collision.gameObject.GetComponent<OVRGrabbable>().grabPoints)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), col, true);
            }
        }
    }
}
