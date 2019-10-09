using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialougeRunner : MonoBehaviour
{
    public float textAddSpeed = 0.15f;
    private TextMeshProUGUI TMP;
    public string CurrentText { get; private set;}
    
    public bool isRunning { get; private set; }

    private Coroutine currentRoutine;
    private Coroutine currentExclaimRoutine;
    
    // Start is called before the first frame update
    void Awake()
    {
        TMP = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Run normal dialouge text, stops all other text
    public void RunString(string s)
    { 
        if(currentRoutine != null) StopCoroutine(currentRoutine);
        if(currentExclaimRoutine != null) StopCoroutine(currentExclaimRoutine);
        
        currentRoutine = StartCoroutine(RunStringCoroutine(s));
    }
    
    private IEnumerator RunStringCoroutine(string s)
    {
        yield return RunStringCoroutineInternal(s);
    }
    
    private IEnumerator RunStringCoroutineInternal(string s)
    {
        TMP.text = ""; 
        CurrentText = s;
        isRunning = true;
        
        int index = 0;
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(textAddSpeed);
        WaitForSecondsRealtime waitLonger = new WaitForSecondsRealtime(0.5f);
        while (index < s.Length)
        {
            var c = s[index];
            TMP.text += c; //todo use string builder
            index++;
            if (c == '?' || c == '.' || c == ',') 
                yield return waitLonger;
            else
                yield return wait;
            yield return new WaitForEndOfFrame();
        }
        
        yield return new WaitForSecondsRealtime(1);
        isRunning = false;
    }
    
    //Exclaim something in the middle of dialouge
    public void Exclaim(string s)
    {
        currentExclaimRoutine = StartCoroutine(ExclaimRoutine(s));
    }
    
    
    private IEnumerator ExclaimRoutine(string s)
    {
        string previous = CurrentText;
        yield return RunStringCoroutineInternal(s);
        yield return new WaitForSeconds(3);
        yield return RunStringCoroutineInternal(previous);
    }

}
