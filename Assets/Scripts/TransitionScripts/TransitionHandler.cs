using UnityEngine;

public class TransitionHandler : MonoBehaviour
{
    [SerializeField] private SceneField targetScene;

    public void TransitionToTargetScene()
    {
        TransitionManager.Instance.ChangeScene(targetScene);
    }
}
