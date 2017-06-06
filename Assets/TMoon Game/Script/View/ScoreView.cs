using System.Collections;
using System.Collections.Generic;
using TMoonEventSystem;
using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour
{

    private Text score;

    private void Awake()
    {
        score = GetComponent<Text>();
        MessageDispatcher.AddListener("ScoreViewModel.Score", OnScoreViewModelChange,"");
    }


    private void OnScoreViewModelChange(IMessage m)
    {
        score.text = "分数:" + m.Data.ToString();
    }
}
