using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoaderPanelCtrl : BasePanel
{
    public Slider loaderSlider;
    public CanvasGroup cg;

    private AsyncOperation ao;

    public override void OnEnter()
    {
        OpenPanel();
        ao = SceneManager.LoadSceneAsync(1);
        StartCoroutine(LoadNewScene());
    }

    public override void OnExit()
    {
        ClosePanel();
    }

    private void Update()
    {
        if (ao != null && !ao.isDone)
        {
            loaderSlider.value = ao.progress;
        }
    }

    private IEnumerator LoadNewScene()
    {
        yield return ao;
        UIManager.Instance.PopPanel();
    }

    private void OpenPanel()
    {
        cg.alpha = 1;
    }

    private void ClosePanel()
    {
        cg.alpha = 0;
    }
}
