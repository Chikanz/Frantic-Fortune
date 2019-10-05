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

    public HandMover HM;

    private int customerIndex;

    public float TimePerQuestion = 15;
    private float questionTimer;

    public Slider QuestionTimeSlider;

    private int? response;

    private string CurrentReview;

    private WaitUntil waitForAnyKey;
    private bool canExclaim = true;

    public response defaultResponse;
    
    
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
        DR.RunString($"{c.name}: {c.introText}");
       
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
            yield return DR.RunStringCoroutine($"{c.name}: {q.prompt}");

            //show answers
            for (int i = 0; i < 3; i++)
            {
                ResponseOptions[i].text = q.answers[i].smallAnswer;
            }

            //Show answer progress bar + decrement
            questionTimer = TimePerQuestion;
            response = null;
            canExclaim = true;
            while (questionTimer > 0 && response == null)
            {
                questionTimer -= Time.deltaTime;
                QuestionTimeSlider.value = questionTimer / TimePerQuestion;
                
                //Check for like
                
                //Check for hand out of bounds
                if (!HM.inBounds && canExclaim)
                {
                    StartCoroutine(Exclaim($"{c.name}: Hey! Focus up!"));
                    //Deduct points here
                }
                //check for hand still
                if (HM.isStill && canExclaim)
                {
                    StartCoroutine(Exclaim($"{c.name}: Hey! do you have a palm fetish or something???"));
                }
                    
                yield return null;
            }

            response selectedResponse;
            if (questionTimer <= 0 && response == null) //Default answer
            {
                //Neg points
                selectedResponse = defaultResponse;
            }
            else
            {
                selectedResponse = q.answers[response.Value];
            }

            //hide progress bar
            QuestionTimeSlider.gameObject.SetActive(false);

            //Teller response
            yield return DR.RunStringCoroutine($"You: {selectedResponse.extendedAnswer}");

            yield return waitForAnyKey;
            //character response
            yield return DR.RunStringCoroutine($"{c.name}: {selectedResponse.customerResponse}");

            //Add review
            yield return waitForAnyKey;
            CurrentReview += selectedResponse.review;
        }

        //character and hand intro leave
        yield return DR.RunStringCoroutine($"{c.name}: Cool, thanks!");

        //Get review
        
    }

    public void GiveAnswer(int answer)
    {
        response = answer;
        
        //Maybe fade other answers here
    }

    //Exclaim something in the middle of dialouge
    private IEnumerator Exclaim(string s)
    {
        canExclaim = false;
        string previous = DR.CurrentText;
        DR.RunString(s); 
        yield return new WaitForSeconds(3);
        DR.RunString(previous);
    }
}
