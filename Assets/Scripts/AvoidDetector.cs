using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidDetector : MonoBehaviour
{
    public float avoidPath = 0;
    public float avoidTime = 0;
    public float wanderDistance = 4;
    public float avoidLength = 1;
    public bool reverse = false;
    Rigidbody rb;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void OnTriggerExit(Collider col)
    {
        reverse = false;
        if (col.gameObject.tag != "car") return;
        avoidTime = 0;
    }

    void OnTriggerStay(Collider col)
    {
        Vector3 collisionDir = this.transform.InverseTransformPoint(col.gameObject.transform.position);

        if (collisionDir.x > 0 && collisionDir.z > 0)
        {
            if (rb.velocity.magnitude < 1) reverse = true;
            else if (col.gameObject.tag == "car")
            {
                Rigidbody otherCar = col.GetComponent<Rigidbody>();
                avoidTime = Time.time + avoidLength;

                Vector3 otherCarLocalTarget = transform.InverseTransformPoint(otherCar.gameObject.transform.position);
                float otherCarAngle = Mathf.Atan2(otherCarLocalTarget.x, otherCarLocalTarget.z);
                avoidPath = wanderDistance * -Mathf.Sign(otherCarAngle);
            }
        }
    }
}
