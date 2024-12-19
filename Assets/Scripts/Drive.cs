using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public WheelCollider WC;
    public GameObject wheelMesh;
    public float maxTorque = 200;
    public float maxSteerAngle = 60;
    public float maxBrakeTorque = 500;
    public bool canTurn = false;


    // Start is called before the first frame update
    void Start()
    {
        WC = this.GetComponent<WheelCollider>();
    }

    public void Go(float accel, float steer, float brake)
    {
        accel = Mathf.Clamp(accel, -1, 1);
        float thrustTorque = accel * maxTorque;
        WC.motorTorque = thrustTorque;

        if (canTurn)
        {
            steer = Mathf.Clamp(steer, -1, 1) * maxSteerAngle;
            WC.steerAngle = steer;
        }
        else
        {
            brake = Mathf.Clamp(brake, -1, 1) * maxBrakeTorque;
            WC.brakeTorque = brake;
        }

        Quaternion quat;
        Vector3 position;
        WC.GetWorldPose(out position, out quat);
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = quat;
    }


    // Update is called once per frame
    /*void Update()
    {
        float a = Input.GetAxis("Vertical");
        float s = Input.GetAxis("Horizontal");
        Go(a, s);
    }*/
}