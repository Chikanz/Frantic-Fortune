
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SocialInterestController : MonoBehaviour
{
	public Image interestImage;
	public Text interestName;

	public void Setup(SocialInterest data)
	{
		interestImage.sprite = data.interestImage;
		interestName.text = data.interestName;
	}
}
