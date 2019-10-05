using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu()]
public class CustomerData : ScriptableObject
{
    public string Name;
    public Sprite CharacterSprite;
    public Sprite HandSprite;
    public Question[] Questions;
    public string introText;

}

[Serializable]
public struct Question
{
    public string prompt;
    public response[] answers;
}

[Serializable]
public struct response
{
    public string smallAnswer; //Small answer shown on UI
    public string extendedAnswer; //Extended answer in text
    public string review; //Text added to review
    public string customerResponse; //How the customer responds to your answer
}
