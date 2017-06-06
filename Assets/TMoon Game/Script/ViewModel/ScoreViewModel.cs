using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreViewModel
{
    public BindableProperty<int> Score = new BindableProperty<int>().SetType("ScoreViewModel.Score");
}
