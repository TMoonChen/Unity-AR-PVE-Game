using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMoonEventSystem;

/// <summary>
/// 状态应该能知道哪些状态能进入自己的状态
/// 要是在添加转换的状态时候附加上转换条件那就更完美了
/// </summary>
namespace PlayerState
{
    public class PlayerIdleState : State
    {
        private PlayerManager playerMachine { get { return (PlayerManager)machine; } }

        private Animator anim;

        public override void Initialize()
        {
            AddTransition<PlayerWalkState>();
            AddTransition<PlayerAttackState>();
            AddTransition<PlayerDieState>();

            anim = playerMachine.anim;
        }

        public override void Enter(Type lastState)
        {
            anim.SetBool("IsIdle", true);
        }

        public override void Execute()
        {
            if (playerMachine.direction != Vector3.zero)
            {
                playerMachine.ChangeState<PlayerWalkState>();
                return;
            }

            if (playerMachine.isAttackClick)
            {
                playerMachine.ChangeState<PlayerAttackState>();
                return;
            }
        }

        public override void Exit()
        {
            anim.SetBool("IsIdle", false);
        }

    }

    public class PlayerWalkState : State
    {
        private PlayerManager playerMachine { get { return (PlayerManager)machine; } }

        private CharacterController cc;
        private Transform m_Transform;
        private Animator anim;

        public override void Initialize()
        {
            AddTransition<PlayerIdleState>();
            AddTransition<PlayerAttackState>();
            AddTransition<PlayerDieState>();

            cc = playerMachine.cc;
            m_Transform = playerMachine.transform;
            anim = playerMachine.anim;

        }

        public override void Enter(Type lastState)
        {
            anim.SetBool("IsRun", true);
        }

        public override void Execute()
        {
            if (playerMachine.direction == Vector3.zero)
            {
                playerMachine.ChangeState<PlayerIdleState>();
                return;
            }

            if (playerMachine.isAttackClick)
            {
                playerMachine.ChangeState<PlayerAttackState>();
                return;
            }

            #region 失败的代码  本来是想做面向屏幕的移动
            //Vector3 tempDir = Vector3.zero;
            //if (playerMachine.direction.z != 0)
            //{
            //    tempDir = (m_Transform.position - Camera.main.transform.position) * playerMachine.direction.z;
            //}
            //else
            //{
            //    tempDir = (m_Transform.position - Camera.main.transform.position);
            //}
            ////tempDir = (m_Transform.position - Camera.main.transform.position) * playerMachine.direction.z;
            //tempDir.y = m_Transform.position.y;
            //Quaternion rotation = Quaternion.Euler(0f, 90f * playerMachine.direction.x, 0f);
            //tempDir = rotation * tempDir;
            //tempDir.y = m_Transform.position.y;
            //Debug.DrawLine(m_Transform.position, tempDir, Color.red, 0.5f);

            //m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.LookRotation(tempDir), Time.deltaTime * playerMachine.rotateSpeed);
            #endregion

            #region V2 

            //Vector3 sceneDir = m_Transform.position - Camera.main.transform.position;
            //Quaternion targetRotation = Quaternion.identity;

            //if (playerMachine.direction.z > 0.1f)
            //{  
            //    sceneDir = sceneDir * playerMachine.direction.z;
            //    targetRotation = Quaternion.LookRotation(sceneDir);
            //}
            //if (playerMachine.direction.z < -0.1f)
            //{
            //    sceneDir = sceneDir * playerMachine.direction.z;
            //    targetRotation = Quaternion.LookRotation(sceneDir);
            //}
            //if (playerMachine.direction.x > 0.1f)
            //{
            //    Quaternion rotation = Quaternion.Euler(0f, 90f * playerMachine.direction.x, 0f);
            //    sceneDir = rotation * sceneDir;
            //}
            //if (playerMachine.direction.x < -0.1f)
            //{
            //    Quaternion rotation = Quaternion.Euler(0f, 90f * playerMachine.direction.x, 0f);
            //    sceneDir = rotation * sceneDir;
            //}
            //if (playerMachine.direction.z > 0.1f && playerMachine.direction.x > 0.1f)
            //{

            //}
            //if (playerMachine.direction.z > 0.1f && playerMachine.direction.x < -0.1f)
            //{

            //}
            //if (playerMachine.direction.z < -0.1f && playerMachine.direction.x > 0.1f)
            //{

            //}
            //if (playerMachine.direction.z < -0.1f && playerMachine.direction.x < -0.1f)
            //{

            //}

            //sceneDir.y = m_Transform.position.y;

            //Debug.DrawLine(m_Transform.position,sceneDir,Color.red,0.5f);

            //m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.LookRotation(sceneDir), Time.deltaTime * playerMachine.rotateSpeed);

            #endregion

            m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, Quaternion.LookRotation(playerMachine.direction), Time.deltaTime * playerMachine.rotateSpeed);
            cc.SimpleMove(m_Transform.forward * playerMachine.moveSpeed); 
        }

        public override void Exit()
        {
            anim.SetBool("IsRun", false);
        }
    }

    public class PlayerAttackState : State
    {
        private PlayerManager playerMachine { get { return (PlayerManager)machine; } }

        private Animator anim;
        private Transform m_Transform;
        private float atktimer = 0f;

        public override void Initialize()
        {
            AddTransition<PlayerIdleState>();
            AddTransition<PlayerDieState>();
            AddTransition<PlayerAttackState>();

            anim = playerMachine.anim;
            m_Transform = playerMachine.transform;
        }

        public override void Enter(Type lastState)
        {
            anim.SetTrigger("IsAttack");
            atktimer = Time.time + 0.7f;
            playerMachine.isAttackClick = false;
        }

        public override void Execute()
        {
            if (playerMachine.isAttack)
            {
                AttackAroundEnemies();
                playerMachine.isAttack = false;
            }

            if (playerMachine.canCombos)
            {
                playerMachine.canCombos = false;
                playerMachine.isCombos = false;
                playerMachine.ChangeState<PlayerAttackState>();
            }

            if (atktimer <= Time.time)
            {
                playerMachine.ChangeState<PlayerIdleState>();
            }

        }

        private void AttackAroundEnemies()
        {
            GameObject[] gos = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject item in gos)
            {
                if (Vector3.Distance(m_Transform.position, item.transform.position) < 2f && Vector3.Angle(item.transform.position - m_Transform.position, m_Transform.forward) < 40f)
                {
                    Message m = Message.Allocate();
                    m.Type = "EnemyTakeDamage";
                    m.Data = playerMachine.damage;
                    m.Filter = item.GetComponent<EnemyManager>().enemyId.ToString();
                    MessageDispatcher.SendMessage(m);
                    Message.Release(m);
                }
            }
        }

        public override void Exit()
        {
            playerMachine.isAttackClick = false;
            playerMachine.canCombos = false;
            playerMachine.isCombos = false;
        }

    }

    public class PlayerDieState : State
    {
        private PlayerManager playerMachine { get { return (PlayerManager)machine; } }

        public override void Initialize()
        {
            AddTransition<PlayerIdleState>();
        }

        public override void Enter(Type lastState)
        {
            playerMachine.anim.SetTrigger("IsDie");
        }

        public override void Execute()
        {
            if (GameManager.Instance.PlayerModel.HP.Value > 0)
            {
                playerMachine.ChangeState<PlayerIdleState>();
            }
        }

    }

    public class PlayerGetHitState : State
    {
        private PlayerManager playerMachine { get { return (PlayerManager)machine; } }

        public override void Initialize()
        {
            AddTransition<PlayerDieState>();
            MessageDispatcher.AddListener("PlayerTakeDamage", TakeDamage,"");
        }

        public void TakeDamage(IMessage m)
        {
            if (GameManager.Instance.PlayerModel.HP.Value <= 0)
            {
                return;
            }

            GameManager.Instance.PlayerModel.HP.Value -= (int)m.Data;

            if (GameManager.Instance.PlayerModel.HP.Value <= 0)
            {
                playerMachine.ChangeState<PlayerDieState>();
                GameManager.Instance.PlayerModel.HP.Value = 0;
            }
        }

    }
}
