using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class SocialInterest
{
	public Sprite interestImage;
	public string interestName;
}

[System.Serializable]
public class SocialPost
{
	public Sprite senderImage;
	public string senderText;
	public Sprite contentImage;
	[TextArea(4, 4)]
	public string contentText;
	public Sprite commentImage;
	[TextArea(4,4)]
	public string commentText;
	public Sprite likesImage;
	public string likesText;
}

[CreateAssetMenu(fileName = "New Profile", menuName = "Data/SocialProfile", order = 1)]
public class SocialProfileData : ScriptableObject
{
	public string ProfileName;
	public Sprite ProfileImage;
	[TextArea(4, 4)]
	public string StatusText;
	public SocialInterest[] Interests;
	[ReorderableList]
	public List<SocialPost> Posts;
}
