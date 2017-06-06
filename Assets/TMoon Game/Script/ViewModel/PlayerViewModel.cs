using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerViewModel
{
    public BindableProperty<int> HP = new BindableProperty<int>().SetType("PlayerViewModel.HP");

}
