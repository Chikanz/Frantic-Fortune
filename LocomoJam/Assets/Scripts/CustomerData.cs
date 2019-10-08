using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu()]
public class CustomerData : ScriptableObject
{
    public string CharacterName;
    public Sprite CharacterSprite;
    public Sprite HandSprite;
    public Sprite HeadSprite;
    public Sprite GoodReact;
    public Sprite BadReact;
    public bool reactOnBody;
    
    [Header("Custom text")]
    public string introText;
    public string OutroText;
    public string palmBounds;
    public string palmStill;
    public string tooLong;
    public Question[] Questions;
    public SocialProfileData FacebookData;

}

[Serializable]
public struct Question
{
    public string prompt;
    public List<response> answers;
}

[Serializable]
public struct response
{
    public string smallAnswer; //Small answer shown on UI
    public string extendedAnswer; //Extended answer in text
    public string review; //Text added to review
    public string customerResponse; //How the customer responds to your answer
    public int points; //Points on the review score
}
