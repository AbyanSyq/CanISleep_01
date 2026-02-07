using System.Collections;
using UnityEngine;

public class MonologueManager : SingletonMonoBehaviour<MonologueManager>
{
    [Header("Settings")]
    [SerializeField] private UIMonologue uiMonologue;
    
    [Header("Current State")]
    [SerializeField, ReadOnly] private Coroutine currentMonologueCoroutine;
    public bool isPlaying = false;
    public float time;

    protected override void Awake()
    {
        Reinitialize();
        base.Awake();
    }
    void Update()
    {
        time = Time.timeScale;
    }

    private void Reinitialize()
    {
        isPlaying = false;
        if (currentMonologueCoroutine != null)
        {
            StopCoroutine(currentMonologueCoroutine);
            currentMonologueCoroutine = null;
        }
        StopAllCoroutines();
    }

    public void PlayMonologue(MonologueSO so)
    {
        if (so == null || so.monologueDataList.Count == 0) return;
        if (currentMonologueCoroutine != null) StopCoroutine(currentMonologueCoroutine);
        
        currentMonologueCoroutine = StartCoroutine(MonologueSequenceRoutine(so));
    }

    private IEnumerator MonologueSequenceRoutine(MonologueSO so)
    {                                                                                   
        isPlaying = true;

        uiMonologue.Show();

        float waitShow = uiMonologue.AnimationDuration > 0 ? uiMonologue.AnimationDuration : 0.1f;
        // Debug.Log($"Waiting for show animation: {waitShow} seconds");
        yield return new WaitForSeconds(waitShow);
        foreach (var data in so.monologueDataList)
        {
            uiMonologue.OnMonologueLoad(data);
            
            yield return new WaitUntil(() => !uiMonologue.isTyping);
            
            yield return new WaitForSeconds(data.displayDuration); 

            yield return new WaitForSeconds(so.intervalBetweenTexts); 
            
            uiMonologue.HideUIText();
        }
        uiMonologue.Hide();
        float waitHide = uiMonologue.AnimationDuration > 0 ? uiMonologue.AnimationDuration : 0.1f;
        yield return new WaitForSeconds(waitHide);

        isPlaying = false;
        currentMonologueCoroutine = null;
    }
}