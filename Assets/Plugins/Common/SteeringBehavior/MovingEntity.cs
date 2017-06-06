using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntity : MonoBehaviour
{
    /// <summary>
    /// 当前智能体的速度
    /// </summary>
    [HideInInspector]
    public Vector3 m_vVelocity;

    /// <summary>
    /// 当前智能体的位置信息
    /// </summary>
    [HideInInspector]
    public Transform selfTransform;

    /// <summary>
    /// 当前智能体所达到的最大速率
    /// </summary>
    public float m_fMaxSpeed = 1f;

    /// <summary>
    /// 徘徊圈半径
    /// </summary>
    public float m_fWanderRadius = 10f;
    /// <summary>
    /// 徘徊圈上的目标位置跳动阈值
    /// </summary>
    public float m_fWanderJitter = 50f;
    /// <summary>
    /// 徘徊目标
    /// </summary>
    public Vector3 m_vWanderTarger = Vector3.zero;

    /// <summary>
    /// 当前智能体的操控行为类
    /// </summary>
    public SteeringBehavior m_pSteering;

    private void Awake()
    {
        m_pSteering = new SteeringBehavior(this);
        selfTransform = this.transform;
    }

    #region Test Code
    //switch (state)
    //{
    //    case MoverState.Null:
    //        break;
    //    case MoverState.Seek:
    //        Vector3 seekVel = m_pSteering.Seek(target.selfTransform.position);
    //        selfTransform.Translate(seekVel * Time.deltaTime);
    //        // 图形调试：画出智能体移动的方向
    //        Debug.DrawRay(selfTransform.position, seekVel * 10f, Color.green);
    //        break;
    //    case MoverState.Flee:
    //        Vector3 fleeVel = m_pSteering.Flee(target.selfTransform.position);
    //        selfTransform.Translate(fleeVel * Time.deltaTime);
    //        // 图形调试：画出智能体移动的方向
    //        Debug.DrawRay(selfTransform.position, fleeVel * 10f, Color.green);
    //        break;
    //    case MoverState.Arrive:
    //        Vector3 arriveVel = m_pSteering.Arrive(target.selfTransform.position, 1f, 1f);
    //        selfTransform.Translate(arriveVel * Time.deltaTime);
    //        // 图形调试：画出智能体移动的方向
    //        Debug.DrawRay(selfTransform.position, arriveVel * 10f, Color.green);
    //        break;
    //    case MoverState.Pursuit:
    //        Vector3 pursuiteVel = m_pSteering.Pursuit(target);
    //        selfTransform.Translate(pursuiteVel * Time.deltaTime);
    //        // 图形调试：画出智能体移动的方向
    //        Debug.DrawRay(selfTransform.position, pursuiteVel * 10f, Color.green);
    //        break;
    //    case MoverState.Evade:
    //        Vector3 evadeVel = m_pSteering.Evade(target);
    //        selfTransform.Translate(evadeVel * Time.deltaTime);
    //        // 图形调试：画出智能体移动的方向
    //        Debug.DrawRay(selfTransform.position, evadeVel * 10f, Color.green);
    //        break;
    //    case MoverState.Wander:
    //        timer += Time.deltaTime;
    //        if (timer >= wanderTime)
    //        {
    //            m_pSteering.Wander();
    //            timer = 0f;
    //        }
    //        m_vWanderTarger.y = selfTransform.position.y;
    //        Vector3 wanderVel = m_pSteering.Seek(m_vWanderTarger);
    //        selfTransform.Translate(wanderVel * Time.deltaTime);
    //        // 图形调试：画出智能体移动的方向
    //        Debug.DrawRay(selfTransform.position, wanderVel * 10f, Color.green);
    //        // 图形调试：画出智能体徘徊范围
    //        Debug.DrawLine(selfTransform.position, m_vWanderTarger, Color.red, 1000f);
    //        break;
    //    case MoverState.Interpose:
    //        Vector3 interposeVel = m_pSteering.Interpose(target, targetB);
    //        selfTransform.Translate(interposeVel * Time.deltaTime);
    //        // 图形调试：画出智能体移动的方向
    //        Debug.DrawRay(selfTransform.position, interposeVel * 10f, Color.green);
    //        break;
    //    case MoverState.Hiding:
    //        Vector3 hidingPos = m_pSteering.GetHidingPosition(target.selfTransform.position, 3f, posTarget);
    //        Vector3 arriveVel1 = m_pSteering.Arrive(hidingPos, 1f, 1f);
    //        selfTransform.Translate(arriveVel1 * Time.deltaTime);
    //        // 图形调试：画出智能体移动的方向
    //        Debug.DrawRay(selfTransform.position, arriveVel1 * 10f, Color.green);
    //        break;
    //    case MoverState.OffsetPursuit:
    //        Vector3 offsetPursuitVel = m_pSteering.OffsetPursuit(leader, offsetPos);
    //        selfTransform.Translate(offsetPursuitVel * Time.deltaTime);
    //        // 图形调试：画出智能体移动的方向
    //        Debug.DrawRay(selfTransform.position, offsetPursuitVel * 10f, Color.green);
    //        break;
    //}
    #endregion
}