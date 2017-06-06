using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    private float distance = 5f;    //跟随距离
    [SerializeField]
    private float speed = 2f;   //跟随速率
    [SerializeField]
    private float mouseSpeed = 2f;  //鼠标移动敏感度
    [SerializeField]
    private float mouseScroll = 2f; //鼠标缩放速率

    //跟随最大与最小距离
    private float distanceMin = 2f;
    private float distanceMax = 15f;
    private Transform target;   //摄像机跟随的目标
    private Vector3 targetPos;  //目标位置
    private Vector3 direction = Vector3.zero;   //摄像机移动的方向
    private float mouseX = 0f;  //鼠标x轴移动值
    private float mouseY = 15f; //鼠标y轴移动值
    private float mouseYMin = -5f;   //鼠标y轴移动值最小值
    private float mouseYMax = 89.5f;    //鼠标y轴移动值最大值


    private void Awake()
    {
        target = GameObject.FindWithTag("Player").transform;
        direction = target.position - transform.position;
    }

    private void LateUpdate()
    {
        //TouchZoom();

        //GetInput();

        CameraRotate();

        CalculateTargetPos();

        FollowTarget();
    }

    private void CalculateTargetPos()
    {
        //文章入口的代码图解
        targetPos = target.position - direction.normalized * distance;
    }

    private void FollowTarget()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
        transform.LookAt(target.position);
    }

    private void GetInput()
    {
        if (Input.GetMouseButton(0))
        {
            Cursor.visible = false;

            mouseX += Input.GetAxis("Mouse X") * mouseSpeed;
            //摄像机鼠标上下移动与mouseY是相反的，可自己通过观察可得，这里就不明说了
            mouseY -= Input.GetAxis("Mouse Y") * mouseSpeed;

            mouseY = Mathf.Clamp(mouseY, mouseYMin, mouseYMax);

        }
        else Cursor.visible = true;

        distance = distance - Input.GetAxis("Mouse ScrollWheel") * mouseScroll;
        //distance = distance - isZoom * mouseScroll;

        distance = Mathf.Clamp(distance, distanceMin, distanceMax);

    }

    private void CameraRotate()
    {
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        //transform.rotation = rotation;    //不围绕目标旋转
        //Quaternion作用于Vector3的右乘操作（*）返回一个将向量做旋转操作后的向量.
        //这里算是一个难点，这里意思是将根据鼠标XY轴旋转“加到”Vector3.forward向前向量，所以direction是经过了旋转的向前向量
        //就好像一条直线绕着一点旋转了一样
        direction = rotation * Vector3.forward;
    }

    //记录上一次手机触摸位置判断用户是在左放大还是缩小手势    
    private Vector2 oldPosition1;
    private Vector2 oldPosition2;
    private int isZoom = 0;
    private void TouchZoom()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                //触摸位置  
                Vector3 tempPosition1 = Input.GetTouch(0).position;
                Vector3 tempPosition2 = Input.GetTouch(1).position;

                if (isEnlarge(oldPosition1, oldPosition2, tempPosition1, tempPosition2))
                {
                    //放大
                    isZoom -= 1;
                }
                else
                {
                    //缩小
                    isZoom += -1;
                }

                //备份上一次的触摸位置  
                oldPosition1 = tempPosition1;
                oldPosition2 = tempPosition2;
            }
        }
        else
        {
            isZoom = 0;
        }
    }

    //记录手指位置与初始位置是缩小或放大  
    bool isEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
    {
        float leng1 = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
        float leng2 = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));
        if (leng1 < leng2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}