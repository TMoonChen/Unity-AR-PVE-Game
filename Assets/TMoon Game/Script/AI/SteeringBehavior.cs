using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMoonAI
{

    public class SteeringBehavior : MonoBehaviour
    {

        /// <summary>
        /// 当前智能体的位置信息
        /// </summary>
        [HideInInspector]
        public Transform selfTransform;

        /// <summary>
        /// 当前智能体所达到的最大速率
        /// </summary>
        public float m_fMaxSpeed = 1f;
        [HideInInspector]
        public Transform player;

        public float rotateSpeed = 2f;

        public Vector3 offset;

        private void Awake()
        {
            selfTransform = this.transform;
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Start()
        {
            offset = GameManager.Instance.EnemyAttackPositions[GameManager.Instance.Index];
        }

        /// <summary>
        /// 根据智能体与目标位置的距离，返回到达目标距离的速度
        /// 若到达了则速速为Vector3.zero
        /// </summary>
        /// <param name="targetPos">目标位置</param>
        /// <param name="stopDistance">据目标多少米停止移动</param>
        /// <param name="tweaker">用于调整即将到达目标位置时速率大小，tweaker越大则即将到达时运动速率越小，但有最大速度限制</param>
        /// <returns></returns>
        public bool Arrive(Vector3 targetPos)
        {

            // 智能体到目标位置的预期速度
            Vector3 toTarget = targetPos - selfTransform.position;

            // 判读是否到达停止距离
            if (toTarget.magnitude > 0.5f)
            {
                if (Vector3.Distance(selfTransform.position,player.position) <= 0.5f )
                {
                    return  true;
                }

                selfTransform.rotation = Quaternion.Slerp(selfTransform.rotation, Quaternion.LookRotation(toTarget), Time.deltaTime * rotateSpeed);
                selfTransform.position = Vector3.MoveTowards(selfTransform.position, targetPos, Time.deltaTime * m_fMaxSpeed );
            }
            else
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 偏移追逐
        /// </summary>
        /// <param name="leader">追逐的领头者</param>
        /// <param name="offset">与领头者之间的偏移(间隔)</param>
        /// <returns></returns>
        public Vector3 GetTargetPosition(Transform leader)
        {
            Vector3 worldOffsetPos = leader.TransformPoint(offset);

            return worldOffsetPos;
        }
    }

}

