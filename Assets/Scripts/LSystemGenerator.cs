
using System;
using System.Text;
using UnityEngine;

public class LSystemGenerator : MonoBehaviour
{
    public Rule[] rules;
    public string rootSentence;
    [Range(0f, 10f)]
    public int iterationLimit = 1;

    public bool randomIgnoreRuleModifier = true;
    [Range(0f, 1f)]
    public float chanceToIgnoreRule = 0.2f;

    private void Start()
    {
        Debug.Log(GenerateSentence());
    }

    public string GenerateSentence(string sentence = null)
    {
        if (sentence == null)
        {
            sentence=rootSentence;
        }
        return GrowRecursive(sentence);
    }

    private string GrowRecursive(string sentence, int iterationIndex = 0)
    {
        if (iterationIndex >= iterationLimit)
        {
            return sentence;
        }
        
        StringBuilder newWord = new StringBuilder();

        foreach (var c in sentence) 
        { 
            newWord.Append(c);
            ProcessRulesRecursively(newWord, c, iterationIndex);
        }

        return newWord.ToString();
    }

    private void ProcessRulesRecursively(StringBuilder newWord, char c, int iterationIndex)
    {
        foreach (var rule in rules)
        {
            if (rule.letter == c.ToString()) 
            {
                if (randomIgnoreRuleModifier && iterationIndex > 1) 
                {
                    if (UnityEngine.Random.value < chanceToIgnoreRule)
                    {
                        return;
                    }
                }
                newWord.Append(GrowRecursive(rule.GetResult(), iterationIndex + 1));

            }
        }
    }
}
