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

        foreach (var data in so.monologueDataList)
        {
            // 1. Tampilkan UI
            uiMonologue.Show();
            
            // Pastikan AnimationDuration ada isinya, kalau 0 beri jeda sangat kecil
            float waitShow = uiMonologue.AnimationDuration > 0 ? uiMonologue.AnimationDuration : 0.1f;
            // Debug.Log($"Waiting for show animation: {waitShow} seconds");
            yield return new WaitForSeconds(waitShow);
            // Debug.Log("Show animation completed.");

            // 2. Load Data dan Start Typing
            Debug.Log($"Monologue Type: {data.type}"); // Sekarang pasti terpanggil
            uiMonologue.OnMonologueLoad(data);
            
            // 3. Tunggu sampai selesai ngetik
            // Gunakan WaitUntil dengan aman
            yield return new WaitUntil(() => !uiMonologue.isTyping);
            
            yield return new WaitForSeconds(data.displayDuration); 

            uiMonologue.Hide();
            float waitHide = uiMonologue.AnimationDuration > 0 ? uiMonologue.AnimationDuration : 0.1f;
            yield return new WaitForSeconds(waitHide);
            
            yield return new WaitForSeconds(so.intervalBetweenTexts); 
        }

        isPlaying = false;
        currentMonologueCoroutine = null;
    }
}