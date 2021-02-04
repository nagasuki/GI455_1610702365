using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchEngine : MonoBehaviour
{
    public InputField findWord;
    public Text wordData;
    public string[] messages;

    private string statName;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            FindAnswer();
        }
    }

    public void FindAnswer()
    {
        string outputWord = statName;

        for (int i = 0; i < messages.Length; i++)
        {
            if (messages[i] == findWord.text)
            {
                outputWord = "[ " + $"<color=green>{findWord.text}</color>" + " ] " + " is found.";
                break;
            }
            else
            {
                outputWord = "[ " + $"<color=red>{findWord.text}</color>" + " ] " + " is not found.";
            }
        }
        wordData.text = outputWord;
    }
}
