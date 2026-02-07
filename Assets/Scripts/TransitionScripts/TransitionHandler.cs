using System.Collections;
using UnityEngine;

public class TransitionHandler : MonoBehaviour
{
    [SerializeField] private SceneField targetScene;
    [SerializeField] private float delayBeforeTransition = 0f;

    public void TransitionToTargetScene()
    {
        if (delayBeforeTransition > 0f)
        {
            StartCoroutine(TransitionAfterDelay());
        }
        else
            TransitionManager.Instance.ChangeScene(targetScene);
    }
    public IEnumerator TransitionAfterDelay()
    {
        yield return new WaitForSecondsRealtime(delayBeforeTransition);
        TransitionManager.Instance.ChangeScene(targetScene);
    }
}
