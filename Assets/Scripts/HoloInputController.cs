using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityEngine.Windows.Speech;

public class HoloInputController : SingletonMonobehaviour<HoloInputController>
{
    public GameObject cursorParent;

    public event System.Action<InteractionSourcePressedEventArgs> InteractionSourcePressed;
    public event System.Action<InteractionSourceReleasedEventArgs> InteractionSourceReleased;
    public event System.Action<InteractionSourceDetectedEventArgs> InteractionSourceDetected;
    public event System.Action<InteractionSourceLostEventArgs> InteractionSourceLost;
    public event System.Action<InteractionSourceUpdatedEventArgs> InteractionSourceUpdated;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    void Start ()
    {
        InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;
        InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
        InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
        InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;

        keywords.Add("select", SelectInteractable);
        AddKeywords(new Dictionary<string, System.Action>()
        {
            { "test" , () => { Debug.Log("test keyword recognized"); } }
        });
    }

    void Update()
    {
        UpdateGaze();
    }

    private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs obj)
    {
        SelectInteractable();
        if (InteractionSourcePressed != null) InteractionSourcePressed(obj);
    }

    private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs obj)
    {
        if (InteractionSourceReleased != null) InteractionSourceReleased(obj);
    }

    private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
    {
        if (InteractionSourceDetected != null) InteractionSourceDetected(obj);
    }

    private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
    {
        if (InteractionSourceLost != null) InteractionSourceLost(obj);
    }

    private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj)
    {
        if (InteractionSourceUpdated != null) InteractionSourceUpdated(obj);
    }

    private Interactable current = null;
    void UpdateGaze()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20.0f, Physics.DefaultRaycastLayers))
        {
            var o = hitInfo.collider.gameObject.GetComponent<Interactable>();
            if (o != null)               
            {
                if(current != o)
                {
                    current = o;
                    if (current.onFocus != null) current.onFocus.Invoke();
                }
            }
            if (cursorParent)
            {
                cursorParent.transform.position = hitInfo.point;
                cursorParent.transform.forward = hitInfo.normal;
            }
        }
        else if(current)
        {
            if (current.outFocus != null) current.outFocus.Invoke();
            if (cursorParent) cursorParent.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 20.0f;
            current = null;
        }
        else
        {
            if (cursorParent) cursorParent.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 20.0f;
        }
    }

    void SelectInteractable()
    {
        if (current && current.selected != null) current.selected.Invoke();
    }

    public void AddKeyword(string keyword, System.Action action)
    {
        if (keyword.ToLower() == "select") throw new System.Exception("Can't override the keyword \"select\"");
        keywords.Add(keyword, action);
        InitKeywordRecognizer();
    }

    public void AddKeywords(Dictionary<string, System.Action> kws)
    {
        foreach(var k in kws)
        {
            if (k.Key.ToLower() == "select") throw new System.Exception("Can't override the keyword \"select\"");
            keywords.Add(k.Key, k.Value);
        }
        InitKeywordRecognizer();
    }

    public void RemoveKeyword(string keyword)
    {
        keywords.Remove(keyword);
        InitKeywordRecognizer();
    }

    void InitKeywordRecognizer()
    {
        if(keywordRecognizer != null)
        {
            if (keywordRecognizer.IsRunning) keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction)) keywordAction.Invoke();
    }

    private void OnDestroy()
    {
        InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;
        InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;
        InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;
        InteractionManager.InteractionSourceUpdated -= InteractionManager_InteractionSourceUpdated;
    }
}

