using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMoonEventSystem;

public class PlayerHPView : MonoBehaviour
{
    private Text hp;

    private void Awake()
    {
        hp = GetComponent<Text>();
        MessageDispatcher.AddListener("PlayerViewModel.HP", OnViewModelHPChange,"");
    }


    private void OnViewModelHPChange(IMessage m)
    {
        hp.text = "生命值:" + m.Data.ToString();
    }



}
