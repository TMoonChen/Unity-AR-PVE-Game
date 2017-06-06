using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using TMoonEventSystem;
using UnityEngine;
using TMoonAI;

namespace EnemyStates
{
    public class EnemyIdleState : State
    {
        private EnemyManager EnemyMachine { get { return (EnemyManager)machine; } }

        private Animator anim;
        private Transform m_Transform;
        private Transform player;
        private TMoonAI.SteeringBehavior m_SteeringBehavior;

        public override void Initialize()
        {
            AddTransition<EnemyWalkState>();
            AddTransition<EnemyAttackState01>();
            AddTransition<EnemyDieState>();
            AddTransition<EnemyIdleState>();

            anim = EnemyMachine.anim;
            m_Transform = EnemyMachine.transform;
            m_SteeringBehavior = EnemyMachine.m_SteeringBehavior;
            player = m_SteeringBehavior.player;
        }

        public override void Enter(Type lastState)
        {
            anim.SetBool("IsIdle", true);
        }

        public override void Execute()
        {
            if (GameManager.Instance.PlayerModel.HP.Value <= 0)
            {
                return;
            }

            Vector3 targetPos = m_SteeringBehavior.GetTargetPosition(player);

            if (Vector3.Distance(m_Transform.position, targetPos) > 0.5f)
            {
                EnemyMachine.ChangeState<EnemyWalkState>();
                return;
            }
            else
            {
                EnemyMachine.ChangeState<EnemyAttackState01>();
                return;
            }

        }

        public override void Exit()
        {
            anim.SetBool("IsIdle", false);
        }

    }

    public class EnemyWalkState : State
    {
        private EnemyManager EnemyMachine { get { return (EnemyManager)machine; } }

        private Transform m_Transform;
        private Animator anim;
        private Transform player;
        private TMoonAI.SteeringBehavior m_SteeringBehavior;

        public override void Initialize()
        {
            AddTransition<EnemyIdleState>();
            AddTransition<EnemyAttackState01>();
            AddTransition<EnemyDieState>();

            m_Transform = EnemyMachine.transform;
            anim = EnemyMachine.anim;
            m_SteeringBehavior = EnemyMachine.m_SteeringBehavior;
            player = m_SteeringBehavior.player;

        }

        public override void Enter(Type lastState)
        {
            anim.SetBool("IsRun", true);
        }

        public override void Execute()
        {

            if (GameManager.Instance.PlayerModel.HP.Value <= 0)
            {
                EnemyMachine.ChangeState<EnemyIdleState>();
                return;
            }

            Vector3 targetPos = m_SteeringBehavior.GetTargetPosition(player);

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("run"))
            {
                anim.SetBool("IsRun", true);
            }


            if (m_SteeringBehavior.Arrive(targetPos))
            {
                EnemyMachine.ChangeState<EnemyAttackState01>();
                return;
            }

        }

        public override void Exit()
        {
            anim.SetBool("IsRun", false);
        }

    }

    public class EnemyDieState : State
    {
        private EnemyManager EnemyMachine { get { return (EnemyManager)machine; } }

        private float timer = 2f;

        public override void Initialize()
        {
            AddTransition<EnemyIdleState>();
        }

        public override void Enter(Type lastState)
        {
            GameManager.Instance.ScoreModel.Score.Value += 1;
            EnemyMachine.anim.SetTrigger("IsDie");
            timer = timer + Time.time;
        }

        public override void Execute()
        {
            if (timer <= Time.time)
            {
                TMoonObjectPool.PoolManager.Instance.GetPool(EnemyMachine.gameObject.name).Release(EnemyMachine.gameObject);
                Spawner.currentCount--;
                EnemyMachine.ChangeState<EnemyIdleState>();
                EnemyMachine.enemyViewModel.HP.Value = EnemyMachine.HP;
                EnemyMachine.m_Slider.value = 1f;
                timer = 2f;
            }
        }

    }

    //public class EnemyAttackState : State
    //{
    //    private EnemyManager EnemyMachine { get { return (EnemyManager)machine; } }

    //    private Animator anim;

    //    private float atktimer = 0f;
    //    private Transform m_Transform;
    //    private Transform player;
    //    private TMoonAI.SteeringBehavior m_SteeringBehavior;
    //    System.Random r = new System.Random();
    //    private int combosCount = 0;

    //    public override void Initialize()
    //    {
    //        AddTransition<EnemyIdleState>();
    //        AddTransition<EnemyDieState>();
    //        AddTransition<EnemyAttackState>();

    //        anim = EnemyMachine.anim;
    //        m_Transform = EnemyMachine.transform;
    //        m_SteeringBehavior = EnemyMachine.m_SteeringBehavior;
    //        player = m_SteeringBehavior.player;
    //    }

    //    public override void Enter(Type lastState)
    //    {
    //        anim.SetTrigger("IsAttack");
    //        atktimer = Time.time + (float)r.Next(4, 10) / 10;
    //    }

    //    public override void Execute()
    //    {
    //        m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.LookRotation(player.position - m_Transform.position), Time.deltaTime * m_SteeringBehavior.rotateSpeed);
    //        if (atktimer <= Time.time)
    //        {
    //            EnemyMachine.ChangeState<EnemyIdleState>();
    //        }

    //        if (EnemyMachine.isAttack)
    //        {
    //            if (Vector3.Distance(m_Transform.position, player.position) < 2f)
    //            {
    //                Message m = Message.Allocate();
    //                m.Type = "PlayerTakeDamage";
    //                m.Data = EnemyMachine.damage;
    //                MessageDispatcher.SendMessage(m);
    //                Message.Release(m);
    //            }
    //            EnemyMachine.isAttack = false;
    //        }

    //        Vector3 targetPos = m_SteeringBehavior.GetTargetPosition(player);

    //        if (Vector3.Distance(m_Transform.position, targetPos) > 0.5f)
    //        {
    //            EnemyMachine.ChangeState<EnemyIdleState>();
    //            return;
    //        }

    //    }

    //}

    public class EnemyAttackState01 : State
    {
        private EnemyManager EnemyMachine { get { return (EnemyManager)machine; } }

        private Animator anim;

        private float atktimer = 0f;
        private float waittimer = 1f;
        private Transform m_Transform;
        private Transform player;
        private TMoonAI.SteeringBehavior m_SteeringBehavior;
        private System.Random r = new System.Random();
        private bool canCombos = false;

        public override void Initialize()
        {
            AddTransition<EnemyIdleState>();
            AddTransition<EnemyDieState>();
            AddTransition<EnemyAttackState02>();

            anim = EnemyMachine.anim;
            m_Transform = EnemyMachine.transform;
            m_SteeringBehavior = EnemyMachine.m_SteeringBehavior;
            player = m_SteeringBehavior.player;
        }

        public override void Enter(Type lastState)
        {
            anim.SetTrigger("IsAttack_01");
            float random = (float)r.Next(4, 10) / 10; // 0.4 - 1.0
            canCombos = random > 0.8 ? false:true;
            atktimer = Time.time + random;
            waittimer = waittimer + Time.time;
        }

        public override void Execute()
        {

            m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.LookRotation(player.position - m_Transform.position), Time.deltaTime * m_SteeringBehavior.rotateSpeed);

            if (waittimer <= Time.time)
            {
                return;
            }

           
            if (canCombos)
            {
                if (atktimer <= Time.time)
                {
                    EnemyMachine.ChangeState<EnemyAttackState02>();
                }
            }
            else
            {
                if (atktimer <= Time.time)
                {
                    EnemyMachine.ChangeState<EnemyIdleState>();
                }
            }


            if (EnemyMachine.isAttack)
            {
                if (Vector3.Distance(m_Transform.position, player.position) < 2f)
                {
                    Message m = Message.Allocate();
                    m.Type = "PlayerTakeDamage";
                    m.Data = EnemyMachine.damage;
                    MessageDispatcher.SendMessage(m);
                    Message.Release(m);
                }
                EnemyMachine.isAttack = false;
            }
        }

        public override void Exit()
        {
            waittimer = 2f;
        }

    }

    public class EnemyAttackState02 : State
    {
        private EnemyManager EnemyMachine { get { return (EnemyManager)machine; } }

        private Animator anim;

        private float atktimer = 0f;
        private Transform m_Transform;
        private Transform player;
        private TMoonAI.SteeringBehavior m_SteeringBehavior;
        private System.Random r = new System.Random();
        private bool canCombos = false;

        public override void Initialize()
        {
            AddTransition<EnemyIdleState>();
            AddTransition<EnemyDieState>();
            AddTransition<EnemyAttackState03>();

            anim = EnemyMachine.anim;
            m_Transform = EnemyMachine.transform;
            m_SteeringBehavior = EnemyMachine.m_SteeringBehavior;
            player = m_SteeringBehavior.player;
        }

        public override void Enter(Type lastState)
        {
            anim.SetTrigger("IsAttack_02");
            float random = (float)r.Next(4, 10) / 10; // 0.4 - 1.0
            canCombos = random > 0.8 ? false : true;
            atktimer = Time.time + random;
        }

        public override void Execute()
        {
            m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.LookRotation(player.position - m_Transform.position), Time.deltaTime * m_SteeringBehavior.rotateSpeed);
            if (canCombos)
            {
                if (atktimer <= Time.time)
                {
                    EnemyMachine.ChangeState<EnemyAttackState03>();
                }
            }
            else
            {
                if (atktimer <= Time.time)
                {
                    EnemyMachine.ChangeState<EnemyIdleState>();
                }
            }


            if (EnemyMachine.isAttack)
            {
                if (Vector3.Distance(m_Transform.position, player.position) < 2f)
                {
                    Message m = Message.Allocate();
                    m.Type = "PlayerTakeDamage";
                    m.Data = EnemyMachine.damage;
                    MessageDispatcher.SendMessage(m);
                    Message.Release(m);
                }
                EnemyMachine.isAttack = false;
            }

        }

    }

    public class EnemyAttackState03 : State
    {
        private EnemyManager EnemyMachine { get { return (EnemyManager)machine; } }

        private Animator anim;

        private float atktimer = 0f;
        private Transform m_Transform;
        private Transform player;
        private TMoonAI.SteeringBehavior m_SteeringBehavior;

        public override void Initialize()
        {
            AddTransition<EnemyIdleState>();
            AddTransition<EnemyDieState>();

            anim = EnemyMachine.anim;
            m_Transform = EnemyMachine.transform;
            m_SteeringBehavior = EnemyMachine.m_SteeringBehavior;
            player = m_SteeringBehavior.player;
        }

        public override void Enter(Type lastState)
        {
            anim.SetTrigger("IsAttack_03");
            atktimer = Time.time + 1f;
        }

        public override void Execute()
        {
            m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.LookRotation(player.position - m_Transform.position), Time.deltaTime * m_SteeringBehavior.rotateSpeed);
            if (atktimer <= Time.time)
            {
                EnemyMachine.ChangeState<EnemyIdleState>();
            }

            if (EnemyMachine.isAttack)
            {
                if (Vector3.Distance(m_Transform.position, player.position) < 2f)
                {
                    Message m = Message.Allocate();
                    m.Type = "PlayerTakeDamage";
                    m.Data = EnemyMachine.damage;
                    MessageDispatcher.SendMessage(m);
                    Message.Release(m);
                }
                EnemyMachine.isAttack = false;
            }

        }

    }

    public class EnemyGetHitState : State
    {
        private EnemyManager EnemyMachine { get { return (EnemyManager)machine; } }

        public override void Initialize()
        {
            AddTransition<EnemyDieState>();

            MessageDispatcher.AddListener("EnemyTakeDamage", TakeDamage, EnemyMachine.enemyId.ToString());
        }

        private void TakeDamage(TMoonEventSystem.IMessage m)
        {
            if (EnemyMachine.enemyViewModel.HP.Value <= 0)
            {
                return;
            }

            EnemyMachine.enemyViewModel.HP.Value -= (int)m.Data;
            EnemyMachine.OnTakeDamageUpdateColorEvent();
            EnemyMachine.m_Slider.value = (float)EnemyMachine.enemyViewModel.HP.Value / EnemyMachine.HP;
            if (EnemyMachine.enemyViewModel.HP.Value <= 0)
            {
                EnemyMachine.ChangeState<EnemyDieState>();
            }
        }
    }
}
