
using UnityEngine;

[CreateAssetMenu(menuName = "ProceduralTown/Rule")]
public class Rule : ScriptableObject
{
    public string letter;
    [SerializeField]
    string[] results = null;
    [SerializeField]
    bool randomResult = false;

    public string GetResult()
    {
        if (randomResult)
        {
            int randomIndex = UnityEngine.Random.Range(0, results.Length);
            return results[randomIndex];
        }
        return results[0];
    }
}
