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
        StopCoroutine(RunStringCoroutine(""));
        TMP.text = "";
        CurrentText = s;
        
        int index = 0;
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(textAddSpeed);
        while (index < s.Length)
        {
            TMP.text += s[index];
            index++;
            yield return wait;
        }
        
        yield return new WaitForSecondsRealtime(1);
    }
    
}
