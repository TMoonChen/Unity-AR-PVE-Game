using PlayerState;
using System;
using System.Collections;
using System.Collections.Generic;
using TMoonMapManager;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MachineBehaviour
{
    /// <summary>
    /// 怀疑是Vuforial SDK的问题，当每一次进入这个脚本时所有的资源都被重置或者释放了，放在正常场景中(非AR场景)则不会产生这个问题
    /// 因为在这个脚本第一次实例化的时候就用一个静态变量存储起来，所有用到这个脚本的都直接使用这个静态变量
    /// </summary>
    public static PlayerManager Instance;

    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public CharacterController cc;
    [HideInInspector]
    public Vector3 direction;
    [HideInInspector]
    public bool isCombos = false;
    [HideInInspector]
    public bool canCombos = false;
    [HideInInspector]
    public bool isAttackClick = false;

    /// <summary>
    /// 玩家移动速率
    /// </summary>
    public float moveSpeed = 3f;
    /// <summary>
    /// 玩家旋转速率
    /// </summary>
    public float rotateSpeed = 2f;
    /// <summary>
    /// 玩家对敌人造成的伤害
    /// </summary>
    public int damage = 10;
    public bool isAttack = false;

    private void Awake()
    {
        Instance = this;
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
    }

    public override void AddStates()
    {
        AddState<PlayerIdleState>();
        AddState<PlayerWalkState>();
        AddState<PlayerDieState>();
        AddState<PlayerAttackState>();
        AddState<PlayerGetHitState>();

        SetCurrentState<PlayerIdleState>();
    }


    public override void Start()
    {
        base.Start();
        MapManager.Instance.AddMapIconByGO(this.gameObject, MapIconType.Player);
    }

    public override void Update()
    {
        currentState = Instance.currentState;
        states = Instance.states;
        
        base.Update();

    }

    /** Input Handler **/

    public void OnBeginControl()
    {

    }

    public void OnControlling(Vector3 pos)
    {
        Instance.direction = new Vector3(pos.x, 0, pos.y);
    }

    public void OnEndControl()
    {
        Instance.direction = Vector3.zero;
    }

    public void OnAttackBtnClick()
    {
        if (Instance.IsCurrentState<PlayerDieState>())
        {
            return;
        }

        if (Instance.isCombos)
        {
            Instance.canCombos = true;
        }

        Instance.isAttackClick = true;

    }

    public void Combos()
    {
        Instance.isCombos = true;
    }

    public void DisCombos()
    {
        Instance.isCombos = false;
    }

    public void Attacking()
    {
        Instance.isAttack = true;
    }

    /** Input Handler **/
}
