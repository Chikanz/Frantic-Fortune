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
    
    // Start is called before the first frame update
    void Awake()
    {
        TMP = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RunString(string s)
    { 
        StartCoroutine(RunStringCoroutine(s));
    }
    
    public IEnumerator RunStringCoroutine(string s)
    {
        StopAllCoroutines();
        yield return RunStringCoroutineInternal(s);
    }
    
    private IEnumerator RunStringCoroutineInternal(string s)
    {
        TMP.text = ""; 
        CurrentText = s;
        
        int index = 0;
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(textAddSpeed);
        WaitForSecondsRealtime waitLonger = new WaitForSecondsRealtime(textAddSpeed * 2);
        while (index < s.Length)
        {
            var c = s[index];
            TMP.text += c;
            index++;
            if (c == '!' || c == '?') 
                yield return waitLonger;
            else
                yield return wait;
            yield return new WaitForEndOfFrame();
        }
        
        yield return new WaitForSecondsRealtime(1);
    }
}
