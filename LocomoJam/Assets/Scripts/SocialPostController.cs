
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SocialPostController : MonoBehaviour
{
	public Image senderImage;
	public Text senderText;
	public Image contentImage;
	public Text contentText;
	public GameObject commentPanel;
	public Image commentImage;
	public Text commentText;
	public Button likesButton;
	public Text likesText;
	public ColorBlock likeColors1;
	public ColorBlock likeColors2;
	private bool liked;
	private string likesTextDefault;

	private void Start()
	{
		liked = false;
		likesButton.colors = likeColors1;
		if (senderImage != null)
		{
			senderImage.color = Color.white;
		}
		if (contentImage != null)
		{
			contentImage.color = Color.white;
		}
		if (commentImage != null)
		{
			commentImage.color = Color.white;
		}
	}

	public bool IsLiked()
	{
		return liked;
	}

	public void LikeButtonClicked()
	{
		liked = !liked;
		if (liked)
		{
			likesButton.colors = likeColors2;
			if (string.IsNullOrEmpty(likesTextDefault))
			{
				likesText.text = "You liked this!";
			}
			else
			{
				likesText.text = "You and " + likesTextDefault;
			}
		}
		else
		{
			likesButton.colors = likeColors1;

			likesText.text = likesTextDefault;
		}
	}

	public void Setup(SocialPost data)
	{
		senderImage.sprite = data.senderImage;
		senderText.text = data.senderText;
		if(data.contentImage == null)
		{
			contentImage.gameObject.SetActive(false);
		}
		else
		{
			contentImage.gameObject.SetActive(true);
			contentImage.sprite = data.contentImage;
		}
		contentText.text = data.contentText;
		if (data.commentImage == null || string.IsNullOrEmpty(data.commentText))
		{
			commentPanel.SetActive(false);
		}
		else
		{
			commentPanel.SetActive(true);
			commentImage.sprite = data.commentImage;
			commentText.text = data.commentText;
		}
		likesText.text = likesTextDefault = data.likesText;
	}
}
