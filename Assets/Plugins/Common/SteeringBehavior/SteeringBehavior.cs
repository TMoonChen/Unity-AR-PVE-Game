using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehavior
{
    /// <summary>
    /// 操控行为类的操控对象
    /// </summary>
    private MovingEntity mover;

    /// <summary>
    /// SteeringBehavior构造函数，初始化操控行为类
    /// </summary>
    /// <param name="mover"></param>
    public SteeringBehavior(MovingEntity mover)
    {
        this.mover = mover;
    }

    /// <summary>
    /// 返回靠近目标位置的速度
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public Vector3 Seek(Vector3 targetPos)
    {
        // 对靠近目标位置的理想速度
        Vector3 desiredVel = mover.m_fMaxSpeed * Vector3.Normalize(targetPos - mover.selfTransform.position);
        // 理想速度-当前速度=结果速度，这样行动方向是收受到当前的速度影响，比较合理
        return desiredVel - mover.m_vVelocity;
    }

    /// <summary>
    /// 返回远离目标位置的速度
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public Vector3 Flee(Vector3 targetPos)
    {
        // 对远离目标位置的理想速度
        Vector3 desiredVel = mover.m_fMaxSpeed * Vector3.Normalize(mover.selfTransform.position - targetPos);
        // 理想速度-当前速度=结果速度，这样行动方向是收受到当前的速度影响，比较合理
        return desiredVel - mover.m_vVelocity;
    }

    /// <summary>
    /// 根据智能体与目标位置的距离，返回到达目标距离的速度
    /// 若到达了则速速为Vector3.zero
    /// </summary>
    /// <param name="targetPos">目标位置</param>
    /// <param name="stopDistance">据目标多少米停止移动</param>
    /// <param name="tweaker">用于调整即将到达目标位置时速率大小，tweaker越大则即将到达时运动速率越小，但有最大速度限制</param>
    /// <returns></returns>
    public Vector3 Arrive(Vector3 targetPos, float stopDistance, float tweaker)
    {
        // 智能体到目标位置的预期速度
        Vector3 toTarget = targetPos - mover.selfTransform.position;

        // 智能体与目标位置的距离
        float dist = toTarget.magnitude;

        // 判读是否到达停止距离
        if (dist > stopDistance)
        {
            // 与目标点距离越近则运动速率越小，tweaker越大则运动速率越小
            float speed = dist / tweaker;
            // 确保不超过最大速率
            speed = Mathf.Min(speed, mover.m_fMaxSpeed);
            // 类似Seek
            Vector3 desiredVel = toTarget / dist * speed;
            return desiredVel - mover.m_vVelocity;

        }

        return Vector3.zero;
    }

    /// <summary>
    /// 返回智能体追逐目标的速度
    /// </summary>
    /// <param name="evader">被智能体追逐的逃避者</param>
    /// <returns></returns>
    public Vector3 Pursuit(MovingEntity evader)
    {
        // 智能体到逃避者的预期速度
        Vector3 toEvader = evader.selfTransform.position - mover.selfTransform.position;

        // 智能体与逃避者前方向的点积运算
        float relativeHeading = Vector3.Dot(evader.selfTransform.forward, mover.selfTransform.forward);

        //这里视角判定逃避者是否在智能体前面视角范围36度左右的
        if (relativeHeading < -0.95 //acos(0.95) = 18degs 
           && Vector3.Dot(toEvader, mover.selfTransform.forward) > 0) //智能体朝向与预期速度之间的夹角  大于0为锐角  所以逃避者肯定不会在智能体朝向的后面
        {
            //确定逃避者在智能体前面36°视角范围，直接Seek
            return Seek(evader.selfTransform.position);
        }

        //预估相遇时间  相遇时间是和两者距离成正比，和追逐者速率和逃避者速率成反比
        float LookAheadTime = toEvader.magnitude / (mover.m_fMaxSpeed + evader.m_fMaxSpeed);

        //靠近相遇地点（逃避者的被预测位置）
        return Seek(evader.selfTransform.position + evader.m_vVelocity * LookAheadTime);
    }

    /// <summary>
    /// 返回智能体逃避追逐者的速度
    /// </summary>
    /// <param name="pursuer">追逐智能体的追逐者</param>
    /// <returns></returns>
    public Vector3 Evade(MovingEntity pursuer)
    {
        /*不需要检查是否面向*/

        Vector3 toPursuer = pursuer.selfTransform.position - mover.selfTransform.position;

        //预估相遇时间
        float LookAheadTime = toPursuer.magnitude / (mover.m_fMaxSpeed + pursuer.m_fMaxSpeed);
        //逃离相遇地点
        return Flee(pursuer.selfTransform.position + pursuer.m_vVelocity * LookAheadTime);
    }

    /// <summary>
    /// 返回在徘徊圈位置的速度
    /// </summary>
    /// <returns></returns>
    public void Wander()
    {
        // 给目标点增加随机位移
        mover.m_vWanderTarger += Random.insideUnitSphere * mover.m_fWanderJitter;

        // 目标点回归到圆上
        mover.m_vWanderTarger.Normalize();
        mover.m_vWanderTarger *= mover.m_fWanderRadius;
    }

    /// <summary>
    /// 返回插入两个智能体之间的到达速度
    /// </summary>
    /// <param name="agentA"></param>
    /// <param name="agentB"></param>
    /// <returns></returns>
    public Vector3 Interpose(MovingEntity agentA, MovingEntity agentB)
    {
        // 插入的两个智能体之间的位置的中点
        Vector3 MidPoint = (agentA.selfTransform.position + agentB.selfTransform.position) / 2.0f;

        // 计算当前智能体到达MidPoint的时间
        float TimeToReachMinPoint = Vector3.Distance(mover.selfTransform.position, MidPoint) / mover.m_fMaxSpeed;

        // 假设AB两个智能体在时间T继续前行
        Vector3 APos = agentA.selfTransform.position + agentA.m_vVelocity * TimeToReachMinPoint;
        Vector3 BPos = agentB.selfTransform.position + agentB.m_vVelocity * TimeToReachMinPoint;

        // 重新计算中点
        MidPoint = (APos + BPos) / 2.0f;

        return Arrive(MidPoint, 0f, 1f);

    }

    /// <summary>
    /// 得到目标位置到障碍物之间的隐藏点
    /// </summary>
    /// <param name="posOb"></param>
    /// <param name="radiusOb"></param>
    /// <param name="posTarget"></param>
    /// <returns></returns>
    public Vector3 GetHidingPosition(Vector3 posOb, float radiusOb, Vector3 posTarget)
    {
        // 计算从目标到物体的朝向
        Vector3 toOb = Vector3.Normalize(posOb - posTarget);

        // 确定大小并加到障碍物的位置，得到隐藏点
        return (toOb * radiusOb) + posOb;
    }

    /// <summary>
    /// 偏移追逐
    /// </summary>
    /// <param name="leader">追逐的领头者</param>
    /// <param name="offset">与领头者之间的偏移(间隔)</param>
    /// <returns></returns>
    public Vector3 OffsetPursuit(MovingEntity leader, Vector3 offset)
    {
        //在世界空间中计算偏移的位置
        Vector3 worldOffsetPos = leader.selfTransform.TransformPoint(offset);
        Vector3 toOffset = worldOffsetPos - mover.selfTransform.position;

        //预期时间 与领头者和追逐者的距离成正比，与速度之和成反比
        float lookAheadTime = toOffset.magnitude / (mover.m_fMaxSpeed + leader.m_vVelocity.magnitude);

        //偏移的预测位置
        return Arrive(worldOffsetPos + leader.m_vVelocity * lookAheadTime, 0f, 1f);
    }
}