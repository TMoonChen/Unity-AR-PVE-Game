using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyViewModel
{

    public BindableProperty<int> HP = new BindableProperty<int>().SetType("EnemyViewModel.HP");
}
