using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
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

    public GameObject PhoneParent;  

    private int? response;

    private WaitForSeconds waitASec;
    private WaitUntil waitForAnyKey;
    private WaitUntil waitForDialougeFinished;
    private bool canExclaimFocus = true;
    private bool canExclaimPalm = true;

    public string[] MysticShit;
    private string GetMysticShit => MysticShit[Random.Range(0, MysticShit.Length)];
    public response defaultResponse;

    public SocialProfileController SPC;
    
    //Review
    private int reviewScore;
    private string reviewText;

    [ReorderableList]
    public List<string> tutorialText;

    private GameObject ResponseOptionsParent => ResponseOptions[0].transform.parent.parent.gameObject;
    
    // Start is called before the first frame update
    void Start()
    {
        waitForAnyKey = new WaitUntil(() => Input.anyKey);
        waitASec = new WaitForSeconds(2.0f);
        waitForDialougeFinished = new WaitUntil( () => !DR.isRunning);
        ResponseOptionsParent.SetActive(false);
        QuestionTimeSlider.gameObject.SetActive(false);

        characterHandIntro = CharacterHand.GetComponentInParent<HandIntro>();
        
        PhoneParent.SetActive(false);
        
        StartCoroutine(GameLoop());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator GameLoop()
    {
        //Tutorial
        for (var i = 0; i < tutorialText.Count; i++)
        {
            string s = tutorialText[i];
            if (i == 0) s += "\n(Any key to continue)";
            DR.RunString($"{s}");
            
            if(i == 2) YourHand.TogglePos(true);
            if(i == 4) PhoneParent.SetActive(true);
            if(i == 6) ResponseOptionsParent.SetActive(true);
            
            //yield return waitForDialougeFinished;
            yield return waitForAnyKey;
        }
        
        //Cleanup tutorial 
        DR.RunString(string.Empty);
        YourHand.TogglePos(false);
        ResponseOptionsParent.SetActive(false);

        yield return waitASec;
        yield return waitASec;

        foreach (CustomerData c in Customers)
        {
            SPC.SetupProfile(c.FacebookData); //load facebook data
            reviewScore = 2;
            reviewText = string.Empty;
            
            //Character intro
            Character.gameObject.SetActive(true);
            Character.sprite = c.CharacterSprite;
            
            Head.gameObject.SetActive(true);
            Head.sprite = c.HeadSprite;
            Head.color = new Color(1,1,1,0);

            if(c.HeadSprite) StartCoroutine(FadeSprite(Head, 0, 1, 1.5f));
            yield return FadeSprite(Character, 0, 1, 1.5f);
            
            //character intro text
            foreach (var s in c.introText)
            {
                DR.RunString($"{c.CharacterName}: {s}");
                yield return waitForDialougeFinished;
            }

            //Customer Hand intro
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
            
            SPC.ToggleLoading(false);

            yield return waitASec;

            //question loop 
            foreach (Question q in c.Questions)
            {
                //Read prompt
                DR.RunString($"{c.CharacterName}: {q.prompt}");
                yield return waitForDialougeFinished;

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
                        DR.Exclaim($"{c.CharacterName}: {c.palmBounds}");
                        React(c, false);
                        reviewScore -= 1;
                    }

                    //check for hand still
                    if (HM.isStill && canExclaimPalm && pastInitTime)
                    {
                        canExclaimPalm = false;
                        DR.Exclaim($"{c.CharacterName}: {c.palmStill}");
                        React(c, false);
                        reviewScore -= 1;
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
                DR.RunString($"You: {GetMysticShit}... {selectedResponse.extendedAnswer}");
                yield return waitForDialougeFinished;

                yield return waitASec;
                //character response
                if (selectedResponse.points > 0) React(c, true); 
                if (selectedResponse.points < 0) React(c, false); 
                
                DR.RunString($"{c.CharacterName}: {selectedResponse.customerResponse}");
                yield return waitForDialougeFinished;
                
                yield return waitASec;
            }
            
            //Outtro
            DR.RunString($"{c.CharacterName}: {c.OutroText}");
            yield return waitForDialougeFinished;
            
            //leave hand
            YourHand.enabled = true;
            YourHand.TogglePos(false);

            yield return waitASec;
            
            //leave character
            characterHandIntro.TogglePos(false);

            yield return waitASec;
            
            if(c.HeadSprite) StartCoroutine(FadeSprite(Head, 1, 0, 1.5f));
            yield return FadeSprite(Character, 1, 0, 1.5f);

            yield return waitASec;
            
            //Get review
            reviewScore = Mathf.Clamp(reviewScore, 0, 5);
            SPC.OpenReview(reviewText, reviewScore);
            
            //todo continue and toggle loading on ok pressed  
            
        }
    }

    void BadAction()
    {
        
    }

    void React(CustomerData c, bool isGood)
    {
        var restingReact = c.reactOnBody ? c.CharacterSprite : c.HeadSprite;
        StartCoroutine(isGood
            ? React(c.GoodReact, restingReact, c.reactOnBody)
            : React(c.BadReact, restingReact, c.reactOnBody));
    }

    public void GiveAnswer(int answer)
    {
        response = answer;

        //Maybe fade other answers here
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
            i.color = new Color(c.r, c.g, c.b, Mathf.Lerp(from, to, elapsed/time));
            yield return null;
        }
    }
}
