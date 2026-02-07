using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIGameplayMainMenu : UIBase
{
    [SerializeField] private Image progressBar;
    [SerializeField] private UIBase progressBarUI;

    void OnEnable()
    {
        ShakeAnalogProgressEvents.OnShakeProgressUpdate += UpdateProgressBar;
    }

    void ODisable()
    {
        ShakeAnalogProgressEvents.OnShakeProgressUpdate -= UpdateProgressBar;
    }

    public void UpdateProgressBar(float progress)
    {
        if (progressBar != null)
        {
            progressBar.fillAmount = progress;
            if(progress > 0f)
            {
                StartCoroutine(SetProgressBarUI(true));
            }else if(progress <= 0f)
            {
                StartCoroutine(SetProgressBarUI(false, 0.5f));
            }
        }
    }
    public IEnumerator SetProgressBarUI(bool state, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        if(state)
        {
            if (progressBarUI.isActive) yield break;
            progressBarUI.Show();
        }
        else
        {
            if (!progressBarUI.isActive) yield break;
            progressBarUI.Hide();
        }
    }

}
