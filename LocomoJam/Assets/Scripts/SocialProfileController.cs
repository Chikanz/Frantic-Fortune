using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Loader
{
	public Animation Anim;
	public float Speed = 1f;
	public float Start = 0f;
}

public class SocialProfileController : MonoBehaviour
{
	[Header("Fakebook Profile")]
	public GameObject profilePanel;
	public Image profileImage;
	public Text	profileName;
	public Text	statusText;
	public GameObject interestsPanel;
	public SocialInterestController[] interests;
	public SocialPostController postTemplate;
	public List<SocialPostController> posts;
	[Header("Yelp Review")]
	public GameObject reviewPanel;
	public Text reviewText;
	public Image[] reviewStars;
	public Button buttonOkay;
	public Color reviewStarOff;
	public Color reviewStarOn;
	[Header("Fakebook Loader")]
	public GameObject loaderPanel;
	public Loader[] loaders;
	public string loaderState = "Animation_Loader";

	public float loaderDuration;
	private float loaderTimer;

	public EventHandler OnReviewButtonPressed;

	void Start()
	{
		buttonOkay.onClick.AddListener(OnReviewButtonOkayClicked);
		OnReviewButtonOkayClicked();
	}

	private void OnDestroy()
	{
		buttonOkay.onClick.RemoveAllListeners();
		OnReviewButtonPressed = null;
	}

	private void Update()
	{
		if (loaderPanel == null || profilePanel == null || reviewPanel == null)
			return;

		if (loaderPanel.activeInHierarchy == true)
		{
			loaderTimer += Time.deltaTime;
			if (loaderTimer >= loaderDuration)
			{
				loaderPanel.SetActive(false);
				//profilePanel.SetActive(true);
				reviewPanel.SetActive(false);
			}
		}
	}

	private void OnReviewButtonOkayClicked()
	{
		loaderPanel.SetActive(true);
		//profilePanel.SetActive(false);
		reviewPanel.SetActive(false);

		foreach (Loader loader in loaders)
		{
			loader.Anim[loaderState].normalizedSpeed = loader.Speed;
			loader.Anim[loaderState].normalizedTime = loader.Start;
		}
		loaderTimer = 0;

		if (OnReviewButtonPressed == null)
			return;

		OnReviewButtonPressed.Invoke(this, null);
	}

	public void SetupProfile(SocialProfileData data)
	{
		profileImage.sprite = data.ProfileImage;
		profileName.text = data.ProfileName;
		statusText.text = data.StatusText;
		if (data.Interests.Length == 0)
		{
			interestsPanel.SetActive(false);
		}
		else
		{
			for (int indexInterest = 0;  indexInterest < interests.Length; indexInterest++)
			{
				SocialInterestController interest = interests[indexInterest];
				if (data.Interests == null || data.Interests.Length <= indexInterest)
				{
					interest.interestImage.color = new Color(0, 0, 0, 0);
					interest.interestName.color = new Color(0, 0, 0, 0);
				}
				else
				{
					interest.interestImage.color = new Color(1, 1, 1, 1);
					interest.interestName.color = new Color(0, 0, 0, 1);
					interest.Setup(data.Interests[indexInterest]);
				}
			}
		}
		for (int indexPost = 0; indexPost < posts.Count; indexPost++)
		{
			posts[indexPost].gameObject.SetActive(false);
		}
		for (int indexPostData = 0; indexPostData < data.Posts.Length; indexPostData++)
		{
			if(indexPostData >= posts.Count)
			{
				GameObject newPost = Instantiate(postTemplate.gameObject);
				newPost.transform.parent = profilePanel.transform;
				newPost.transform.SetSiblingIndex(posts[posts.Count - 1].transform.GetSiblingIndex() + 1);
				newPost.transform.localScale = Vector3.one;
				posts.Add(newPost.GetComponent<SocialPostController>());
			} 
			SocialPostController post = posts[indexPostData];
			post.gameObject.SetActive(true);
			post.Setup(data.Posts[indexPostData]);
		}
	}

	public void OpenReview(string text, int starCount)
	{
		loaderPanel.SetActive(false);
		//profilePanel.SetActive(false);
		reviewPanel.SetActive(true);
		reviewText.text = text;
		for(int index = 0; index < reviewStars.Length; index++)
		{
			Image star = reviewStars[index];
			if (index < starCount)
			{
				star.color = reviewStarOn;
			}
			else
			{
				star.color = reviewStarOff;
			}
		}
	}
}
