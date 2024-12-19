using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SimpleVisualizer;

public class Visualizer : MonoBehaviour
{
    public LSystemGenerator lSystem;
    List<Vector3> positions = new();

    public RoadHelper roadHelper;

    public StructureHelper structureHelper;

    int length = 8;
    float angle = 90;

    public int Length
    {
        get => length > 0 ? length : 1;
        set => length = value;
    }

    private void Start()
    {
        var sequence = lSystem.GenerateSentence();
        VisualizeSequence(sequence);
    }
    private void VisualizeSequence(string sequence)
    {
        Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
        var currentPosition = Vector3.zero;

        Vector3 direction = Vector3.forward;
        Vector3 tempPosition = Vector3.zero;

        positions.Add(currentPosition);

        foreach (var letter in sequence)
        {
            EncodingLetters encoding = (EncodingLetters)letter;
            switch (encoding)
            {
                case EncodingLetters.save:
                    savePoints.Push(new AgentParameters
                    {
                        position = currentPosition,
                        direction = direction,
                        length = Length,
                    });
                    break;
                case EncodingLetters.load:
                    if (savePoints.Count > 0)
                    {
                        var agentParameter = savePoints.Pop();
                        currentPosition = agentParameter.position;
                        direction = agentParameter.direction;
                        length = agentParameter.length;
                    }
                    else
                    {
                        throw new System.Exception("Doesn't have a saved point in our stack");
                    }

                    break;
                case EncodingLetters.draw:
                    tempPosition = currentPosition;
                    currentPosition += direction * length;
                    roadHelper.PlaceStreetPositions(tempPosition, Vector3Int.RoundToInt(direction), length);
                    Length -= 2;

                    positions.Add(currentPosition);

                    break;
                case EncodingLetters.turnRight:
                    direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;

                    break;
                case EncodingLetters.turnLeft:
                    direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;

                    break;
                default:
                    break;
            }
        }
        roadHelper.FixRoad();
        structureHelper.PlaceStructuresAroundRoad(roadHelper.GetRoadPositions());

    }
}
