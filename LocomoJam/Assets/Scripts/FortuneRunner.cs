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

    private WaitForSeconds waitForAnyKey;
    private bool canExclaimFocus = true;
    private bool canExclaimPalm = true;

    public response defaultResponse;

    public SocialProfileController SPC;

    private GameObject ResponseOptionsParent => ResponseOptions[0].transform.parent.parent.gameObject;


    // Start is called before the first frame update
    void Start()
    {
        //waitForAnyKey = new WaitUntil(() => Input.anyKey);
        waitForAnyKey = new WaitForSeconds(1);
        ResponseOptionsParent.SetActive(false);
        QuestionTimeSlider.gameObject.SetActive(false);

        StartCoroutine(GameLoop());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator GameLoop()
    {
        foreach (CustomerData c in Customers)
        {
            SPC.Setup(c.FacebookData); //load facebook data
            
            //Hand intro
            Hand.gameObject.SetActive(true);
            Hand.sprite = c.HandSprite;

            //Your hand intro //todo
            Character.gameObject.SetActive(true);
            Character.sprite = c.CharacterSprite;
            
            //character intro
            yield return DR.RunStringCoroutine($"{c.CharacterName}: {c.introText}");

            yield return waitForAnyKey;

            //question loop 
            foreach (Question q in c.Questions)
            {
                //Read prompt
                yield return DR.RunStringCoroutine($"{c.CharacterName}: {q.prompt}");

                //show answers
                QuestionTimeSlider.gameObject.SetActive(true);
                ResponseOptionsParent.SetActive(true);
                for (int i = 0; i < 3; i++)
                {
                    ResponseOptions[i].text = q.answers[i].smallAnswer;
                }

                //Show answer progress bar + decrement
                questionTimer = TimePerQuestion;
                response = null;
                canExclaimFocus = true;
                while (questionTimer > 0 && response == null)
                {
                    questionTimer -= Time.deltaTime;
                    QuestionTimeSlider.value = questionTimer / TimePerQuestion;

                    //Check for like

                    //Check for hand out of bounds
                    bool pastInitTime = questionTimer <= TimePerQuestion - 2;
                    if (!HM.inBounds && canExclaimFocus && pastInitTime)
                    {
                        canExclaimFocus = false;
                        StartCoroutine(Exclaim($"{c.CharacterName}: Hey! Focus up!"));
                        //Deduct points here
                    }

                    //check for hand still
                    if (HM.isStill && canExclaimPalm && pastInitTime)
                    {
                        canExclaimPalm = false;
                        StartCoroutine(Exclaim($"{c.CharacterName}: Hey! do you have a palm fetish or something???"));
                    }

                    yield return null;
                }

                ResponseOptionsParent.SetActive(false);

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
                yield return DR.RunStringCoroutine($"{c.CharacterName}: {selectedResponse.customerResponse}");

                //Add review
                yield return waitForAnyKey;
                CurrentReview += selectedResponse.review;
            }

            //character and hand intro leave
            yield return DR.RunStringCoroutine($"{c.CharacterName}: {c.OutroText}");

            //Get review
        }
    }

    public void GiveAnswer(int answer)
    {
        response = answer;

        //Maybe fade other answers here
    }

    //Exclaim something in the middle of dialouge
    private IEnumerator Exclaim(string s)
    {
        //canExclaimFocus = false;
        string previous = DR.CurrentText;
        DR.RunString(s);
        yield return new WaitForSeconds(3);
        DR.RunString(previous);
    }
}
