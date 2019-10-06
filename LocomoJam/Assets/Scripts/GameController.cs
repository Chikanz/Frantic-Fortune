using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public SocialProfileController socialProfileController;
	public SocialProfileData socialProfileData;
	public string reviewText = "Test Review!";
	public int ReviewStarCount = 3;

    // Start is called before the first frame update
    void Start()
    {
		socialProfileController.SetupProfile(socialProfileData);
	}

    // Update is called once per frame
    void Update()
    {
    }

	public void OpenReview()
	{
		socialProfileController.OpenReview(reviewText, ReviewStarCount);
	}
}
