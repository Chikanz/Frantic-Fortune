using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FortuneRunner : MonoBehaviour
{
    public DialougeRunner DR;

    public CustomerData[] Customers;

    [FormerlySerializedAs("Hand")] [Header("Character Sprites")]
    public Image CharacterHand;
    public Image Character;
    public Image Head;

    public HandIntro YourHand;
    private HandIntro characterHandIntro;

    public TextMeshProUGUI[] ResponseOptions;

    public HandMover HM;

    private int customerIndex;

    public float TimePerQuestion = 15;
    private float questionTimer;

    public Slider QuestionTimeSlider;

    private int? response;

    private WaitForSeconds waitASec;
    private bool canExclaimFocus = true;
    private bool canExclaimPalm = true;

    public string[] MysticShit;
    private string GetMysticShit => MysticShit[Random.Range(0, MysticShit.Length)];
    public response defaultResponse;

    public SocialProfileController SPC;
    
    //Review
    private int reviewScore;
    private string reviewText;

    private GameObject ResponseOptionsParent => ResponseOptions[0].transform.parent.parent.gameObject;


    // Start is called before the first frame update
    void Start()
    {
        //waitForAnyKey = new WaitUntil(() => Input.anyKey);
        waitASec = new WaitForSeconds(1.5f);
        ResponseOptionsParent.SetActive(false);
        QuestionTimeSlider.gameObject.SetActive(false);

        characterHandIntro = CharacterHand.GetComponentInParent<HandIntro>();
        
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
            SPC.SetupProfile(c.FacebookData); //load facebook data
            reviewScore = 2;
            reviewText = string.Empty;
            
            //Character intro
            Character.gameObject.SetActive(true);
            Character.sprite = c.CharacterSprite;

            StartCoroutine(FadeSprite(Head, 0, 1, 1.5f));
            yield return FadeSprite(Character, 0, 1, 1.5f);
            
            //character intro text
            yield return DR.RunStringCoroutine($"{c.CharacterName}: {c.introText}");
            
            //Hand intro
            CharacterHand.gameObject.SetActive(true);
            CharacterHand.sprite = c.HandSprite;
            characterHandIntro.TogglePos(true);
            
            yield return waitASec;
            
            //Your hand intro
            YourHand.enabled = true;
            YourHand.TogglePos(true);
            yield return waitASec;

            //setup default response
            defaultResponse.customerResponse = c.tooLong;
            
            yield return waitASec;
            YourHand.enabled = false;

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
                        StartCoroutine(Exclaim($"{c.CharacterName}: {c.palmBounds}"));
                        //Deduct points here
                    }

                    //check for hand still
                    if (HM.isStill && canExclaimPalm && pastInitTime)
                    {
                        canExclaimPalm = false;
                        StartCoroutine(Exclaim($"{c.CharacterName}: {c.palmStill}"));
                    }

                    yield return null;
                }

                ResponseOptionsParent.SetActive(false);

                response selectedResponse;
                if (questionTimer <= 0 && response == null) //Default answer
                {
                    //Neg points
                    reviewScore -= 1;
                    selectedResponse = defaultResponse;
                }
                else
                {
                    selectedResponse = q.answers[response.Value];
                    reviewScore += selectedResponse.points;
                    reviewText += $"{selectedResponse.review} ";
                }

                //hide progress bar
                QuestionTimeSlider.gameObject.SetActive(false);

                //Teller response
                yield return DR.RunStringCoroutine($"You: {GetMysticShit}... {selectedResponse.extendedAnswer}.");

                yield return waitASec;
                //character response
                if (selectedResponse.points > 0) StartCoroutine(React(c.GoodReact, c.HeadSprite, c.reactOnBody));
                if (selectedResponse.points < 0) StartCoroutine(React(c.BadReact, c.HeadSprite, c.reactOnBody));
                yield return DR.RunStringCoroutine($"{c.CharacterName}: {selectedResponse.customerResponse}");
                
                yield return waitASec;
            }
            
            //Outtro
            yield return DR.RunStringCoroutine($"{c.CharacterName}: {c.OutroText}");
            
            //leave hand
            YourHand.enabled = true;
            YourHand.TogglePos(false);

            yield return waitASec;
            
            //leave character
            
            
            //todo ourtro
            reviewScore = Mathf.Clamp(reviewScore, 0, 5);
            SPC.OpenReview(reviewText, reviewScore); 

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

    private IEnumerator React(Sprite s, Sprite normal, bool onBody)
    {
        Image partToChange = onBody ? Character : Head;
        
        if (partToChange.sprite == s || s == null) yield break; //Already set
        
        partToChange.sprite = s;
        yield return new WaitForSeconds(5);
        partToChange.sprite = normal;
    }

    private IEnumerator FadeSprite(Image i, float from, float to, float time)
    {
        float elapsed = 0;
        var c = i.color;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            i.color = new Color(c.r, c.g, c.b, elapsed/time);
            yield return null;
        }
    }
}
