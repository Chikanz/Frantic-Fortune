using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FortuneRunner : MonoBehaviour
{
    public DialougeRunner DR;
    
    public CustomerData[] Customers;
    
    public Image Hand;
    public Image Character;

    public TextMeshProUGUI[] ResponseOptions;

    private int customerIndex;

    public float TimePerQuestion = 15;
    private float questionTimer;

    public Slider QuestionTimeSlider;

    private int? response;

    private string CurrentReview;

    private WaitUntil waitForAnyKey;
    
    // Start is called before the first frame update
    void Start()
    {
        waitForAnyKey = new WaitUntil(() => Input.anyKey);
        StartCoroutine(GameLoop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GameLoop()
    {
        var c = Customers[customerIndex];
        
        //character intro
        DR.RunString(c.introText);
       
        //Hand intro
        Hand.gameObject.SetActive(true);
        Hand.sprite = c.HandSprite;

        //Your hand intro
        Character.gameObject.SetActive(false);
        Character.sprite = c.CharacterSprite;

        //question loop
        foreach (Question q in c.Questions)
        {
            //Read prompt
            yield return DR.RunStringCoroutine(q.prompt);

            //show answers
            for (int i = 0; i < 3; i++)
            {
                ResponseOptions[i].text = q.answers[i].smallAnswer;
            }

            //Show answer progress bar + decrement
            questionTimer = TimePerQuestion;
            response = null;
            while (questionTimer >= 0 && response == null)
            {
                questionTimer -= Time.deltaTime;
                QuestionTimeSlider.value = questionTimer / TimePerQuestion;
                yield return null;
            }

            //hide progress bar
            QuestionTimeSlider.gameObject.SetActive(false);

            var selectedResponse = q.answers[response.Value];
            
            //Teller response
            yield return DR.RunStringCoroutine(selectedResponse.extendedAnswer);

            yield return waitForAnyKey;
            //character response
            yield return DR.RunStringCoroutine(selectedResponse.customerResponse);

            //Add review
            yield return waitForAnyKey;
            CurrentReview += selectedResponse.review;
        }

        //character and hand intro leave
        yield return DR.RunStringCoroutine("Cool, thanks!");

        //Get review
        
    }

    public void GiveAnswer(int answer)
    {
        response = answer;
        
        //Maybe fade other answers here
    }
}
