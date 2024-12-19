using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct CarSettings 
{
    public float steeringSensitivity;
    public float lookAhead;
    public float maxTorque;
    public float maxSteerAngle;
    public float maxBrakeTorque;
    public float accelCornerMax;
    public float brakeCornerMax;
    public float accelVelocityThreshold;
    public float brakeVelocityThreshold;
    public float antiroll;
    public int fitness;
}
public class AIController : MonoBehaviour
{
    [Header("Car Settings")]
    public ScrObj_CarSettings CarSettings_ScrObj;
    public CarSettings carSettings;

    Drive[] ds;
    public Circuit circuit;
    Vector3 target;
    int currentWP = 0;
    Rigidbody rb;
    public GameObject brakelight;
    GameObject tracker;
    int currentTrackerWP = 0;
    AvoidDetector avoid;



    // Start is called before the first frame update
    void Start()
    {
        if (CarSettings_ScrObj != null)
        {
            carSettings = CarSettings_ScrObj.carSettings;
        }
        ds = this.GetComponentsInChildren<Drive>();
        circuit = GameObject.FindGameObjectWithTag("circuit").GetComponent<Circuit>();
        target = circuit.waypoints[currentWP].transform.position;
        rb = this.GetComponent<Rigidbody>();

        tracker = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        DestroyImmediate(tracker.GetComponent<Collider>());
        tracker.GetComponent<MeshRenderer>().enabled = false;
        tracker.transform.position = this.transform.position;
        tracker.transform.rotation = this.transform.rotation;

        avoid = this.GetComponent<AvoidDetector>();

        this.GetComponent<AntiRoll>().antiRoll = carSettings.antiroll;

        foreach (Drive drive in ds)
        {
            drive.maxTorque = carSettings.maxTorque;
            drive.maxSteerAngle = carSettings.maxSteerAngle;
            drive.maxBrakeTorque = carSettings.maxBrakeTorque;
        }


    }


    float trackerSpeed = 15.0f;
    void ProgressTracker()
    {
        Debug.DrawLine(this.transform.position, tracker.transform.position);
        if (Vector3.Distance(this.transform.position, tracker.transform.position) > carSettings.lookAhead)
        {
            trackerSpeed -= 1.0f;
            if (trackerSpeed < 2) trackerSpeed = 2;
            return;
        }

        if (Vector3.Distance(this.transform.position, tracker.transform.position) < carSettings.lookAhead / 2.0f)
        {
            trackerSpeed += 1.0f;
            if (trackerSpeed > 15) trackerSpeed = 15;
        }

        tracker.transform.LookAt(circuit.waypoints[currentTrackerWP].transform.position);
        tracker.transform.Translate(0, 0, trackerSpeed * Time.deltaTime);

        if (Vector3.Distance(tracker.transform.position,
            circuit.waypoints[currentTrackerWP].transform.position) < 1)
        {
            currentTrackerWP++;
            carSettings.fitness++;
            if (currentTrackerWP >= circuit.waypoints.Length)
                currentTrackerWP = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ProgressTracker();
        target = tracker.transform.position;

        Vector3 localTarget;
        

        if (Time.time < avoid.avoidTime)
        {

            localTarget = tracker.transform.right * avoid.avoidPath;
        }
        else
        {

            localTarget = this.transform.InverseTransformPoint(target);
        }

        //float distanceToTarget = Vector3.Distance(target, this.transform.position);
        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;


        float s = Mathf.Clamp(targetAngle * carSettings.steeringSensitivity, -1, 1) * Mathf.Sign(rb.velocity.magnitude);

        float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0, 90);
        float cornerFactor = corner / 90.0f;

        float a = 1;
        if(corner > carSettings.accelCornerMax && rb.velocity.magnitude > carSettings.accelVelocityThreshold)
            a = Mathf.Lerp(0, 1, 1 - cornerFactor);

        float b = 0;
        if (corner > carSettings.brakeCornerMax && rb.velocity.magnitude > carSettings.brakeVelocityThreshold)
            b = Mathf.Lerp(0, 1, cornerFactor);

        if (avoid.reverse)
        {
            a = -1 * a;
            s = -1 * s;
        }

        for (int i = 0; i < ds.Length; i++)
        {
            ds[i].Go(a, s, b);
        }

        if (b > 0)
        {
            brakelight.SetActive(true);
        }
        else
        {
            brakelight.SetActive(false);
        }

        /*if (distanceToTarget < 4)
        {
            currentWP++;
            if (currentWP >= circuit.waypoints.Length)
                currentWP = 0;

            target = circuit.waypoints[currentWP].transform.position;
        }*/
    }
}

