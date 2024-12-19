using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class CarSpawner : MonoBehaviour
{
    public int NumCars;
    public GameObject CarPrefab;
    public List<GameObject> Cars;
    public int generationTime;
    float startTime;
    public int generation = 1;
    public TextMeshProUGUI TextMesh;
    public int TimeScale = 3;
    public ScrObj_CarSettings FittestCar;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < NumCars; i++)
        {
            GameObject c = Instantiate(CarPrefab, transform.position, transform.rotation);
            AIController ai = c.GetComponent<AIController>();
            ai.carSettings.steeringSensitivity = Random.Range(0.01f, 0.03f);
            ai.carSettings.lookAhead = Random.Range(18.0f, 22.0f);
            ai.carSettings.maxTorque = Random.Range(180.0f, 220.0f);
            ai.carSettings.maxSteerAngle = Random.Range(50.0f, 70.0f);
            ai.carSettings.maxBrakeTorque = Random.Range(4500.0f, 5500.0f);
            ai.carSettings.accelCornerMax = Random.Range(18.0f, 22.0f);
            ai.carSettings.brakeCornerMax = Random.Range(3.0f, 7.0f);
            ai.carSettings.accelVelocityThreshold = Random.Range(18.0f, 22.0f);
            ai.carSettings.brakeVelocityThreshold = Random.Range(8.0f, 12.0f);
            ai.carSettings.antiroll = Random.Range(4500.0f, 5500.0f);
            Cars.Add(c);
        }
        Time.timeScale = TimeScale;
        TextMesh.text = $"Trial: {generation}";
    }

    GameObject GeneSwap(AIController parent1, AIController parent2)
    {
        GameObject c = Instantiate(CarPrefab, transform.position, transform.rotation) ;
        AIController ai = c.GetComponent<AIController>();

        ai.carSettings.steeringSensitivity = (parent1.carSettings.steeringSensitivity + parent2.carSettings.steeringSensitivity)/2.0f;
        ai.carSettings.lookAhead = (parent1.carSettings.lookAhead + parent2.carSettings.lookAhead) / 2.0f;
        ai.carSettings.maxTorque = (parent1.carSettings.maxTorque + parent2.carSettings.maxTorque) / 2.0f;
        ai.carSettings.maxSteerAngle = (parent1.carSettings.maxSteerAngle + parent2.carSettings.maxSteerAngle) / 2.0f;
        ai.carSettings.maxBrakeTorque = (parent1.carSettings.maxBrakeTorque + parent2.carSettings.maxBrakeTorque) / 2.0f;
        ai.carSettings.accelCornerMax = (parent1.carSettings.accelCornerMax + parent2.carSettings.accelCornerMax) / 2.0f;
        ai.carSettings.brakeCornerMax = (parent1.carSettings.brakeCornerMax + parent2.carSettings.brakeCornerMax) / 2.0f;
        ai.carSettings.accelVelocityThreshold = (parent1.carSettings.accelVelocityThreshold + parent2.carSettings.accelVelocityThreshold) / 2.0f;
        ai.carSettings.brakeVelocityThreshold = (parent1.carSettings.brakeVelocityThreshold + parent2.carSettings.brakeVelocityThreshold) / 2.0f;
        ai.carSettings.antiroll = (parent1.carSettings.antiroll + parent2.carSettings.antiroll) / 2.0f;

        return c;
    }

    void Breed()
    {
        startTime = Time.realtimeSinceStartup;
        List<GameObject> sortedCars = Cars.OrderByDescending(o => o.GetComponent<AIController>().carSettings.fitness).ToList();

        int halfCars = (int)(sortedCars.Count / 2);
        Cars.Clear();
        for (int i = 0; i < halfCars; i++)
        {
            Cars.Add(GeneSwap(sortedCars[i].GetComponent<AIController>(), sortedCars[i+1].GetComponent<AIController>()));
            Cars.Add(GeneSwap(sortedCars[i+1].GetComponent<AIController>(), sortedCars[i].GetComponent<AIController>()));
        }

        for (int i = 0;i < sortedCars.Count;i++)
        {
            Destroy(sortedCars[i]);
        }

        generation++;
        TextMesh.text = $"Trial: {generation}";
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.realtimeSinceStartup > startTime + generationTime)
        {
            FittestCar.carSettings = GetFittestCar();
            Breed();
        }
    }

    CarSettings GetFittestCar()
    {
        List<GameObject> sortedCars = Cars.OrderByDescending(o => o.GetComponent<AIController>().carSettings.fitness).ToList();
        CarSettings c = Cars[0].GetComponent<AIController>().carSettings;
        return c;
    }
}
