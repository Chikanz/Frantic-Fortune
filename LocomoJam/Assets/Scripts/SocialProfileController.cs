using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SocialProfileController : MonoBehaviour
{
	public Image profileImage;
	public Text	profileName;
	public Text	statusText;
	public GameObject interestsPanel;
	public SocialInterestController[] interests;
	public SocialPostController postTemplate;
	public List<SocialPostController> posts;

	public void Setup(SocialProfileData data)
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
				newPost.transform.parent = this.transform;
				newPost.transform.SetSiblingIndex(posts[posts.Count - 1].transform.GetSiblingIndex() + 1);
				newPost.transform.localScale = Vector3.one;
				posts.Add(newPost.GetComponent<SocialPostController>());
			} 
			SocialPostController post = posts[indexPostData];
			post.gameObject.SetActive(true);
			post.Setup(data.Posts[indexPostData]);
		}
	}
}
