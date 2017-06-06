using EnemyStates;
using System;
using System.Collections;
using System.Collections.Generic;
using TMoonMapManager;
using UnityEngine;
using TMoonAI;

public class EnemyManager : MachineBehaviour
{

    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public TMoonAI.SteeringBehavior m_SteeringBehavior;  //TO DELETE

    public bool isAttack = false;
    public EnemyViewModel enemyViewModel = new EnemyViewModel();
    public int enemyId;
    public int damage = 10;
    private SkinnedMeshRenderer smr;
    public Action OnTakeDamageUpdateColorEvent;
    public GameObject m_HUD;
    public UnityEngine.UI.Slider m_Slider;
    public int HP = 100;

    private void Awake()
    {
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
        anim = GetComponent<Animator>();
        m_SteeringBehavior = GetComponent<TMoonAI.SteeringBehavior>();//TO DELETE
        enemyViewModel.HP.Value = HP;
        enemyId = this.gameObject.GetInstanceID();
        MapManager.Instance.AddMapIconByGO(this.gameObject, MapIconType.Monster);
        m_Slider = Instantiate<GameObject>(m_HUD).GetComponent<UnityEngine.UI.Slider>();
        m_Slider.GetComponent<HUDText>().target = this.transform;
        m_Slider.gameObject.transform.SetParent(GameManager.Instance.mainCanvas, false);
        m_Slider.value = 1f;
        OnTakeDamageUpdateColorEvent = OnTakeDamageUpdateColor;
    }

    public override void Start()
    {
        base.Start();
    }

    private void OnDisable()
    {
        if (MapManager.Instance.isDestroy)
        {
            return;
        }
        MapManager.Instance.GetMapIconById(enemyId).gameObject.SetActive(false);
        m_Slider.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        MapManager.Instance.GetMapIconById(enemyId).gameObject.SetActive(true);
        m_Slider.gameObject.SetActive(true);
    }

    public override void AddStates()
    {
        AddState<EnemyIdleState>();
        AddState<EnemyWalkState>();
        AddState<EnemyDieState>();
        AddState<EnemyGetHitState>();
        AddState<EnemyAttackState01>();
        AddState<EnemyAttackState02>();
        AddState<EnemyAttackState03>();

        SetCurrentState<EnemyIdleState>();
    }

    private void OnTakeDamageUpdateColor()
    {
        StartCoroutine("UpdateColor");
    }

    private IEnumerator UpdateColor()
    {
        float timer = 0.5f + Time.time;
        while (timer >= Time.time)
        {
            smr.material.color = Color.Lerp(smr.material.color, Color.red, Time.deltaTime);
            yield return 0;
        }

        smr.material.color = Color.white;
    }

    public void Combos()
    {
    }

    public void DisCombos()
    {
    }

    public void Attacking()
    {
        isAttack = true;
    }

}
