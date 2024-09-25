using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentDirection : MonoBehaviour
{
    public float moveSpeed = 5f;  // 移动速度
    public AudioSource moveAudio;  // 移动音频
    private Animator animator;     // Animator组件
    private Vector2[] points;      // 移动路径点
    private int currentPointIndex = 0; // 当前目标点索引
    private bool isMoving = false;   // 是否在移动中
    private Vector2 currentDirection; // 当前移动方向

    void Start()
    {
        animator = GetComponent<Animator>(); // 获取Animator组件

        // 初始化路径点
        points = new Vector2[]
        {
            new Vector2(1, -1),   // 左上角
            new Vector2(6, -1),   // 右上角
            new Vector2(6, -5),   // 右下角
            new Vector2(1, -5)    // 左下角
        };

        // 设置PacStudent的初始位置
        transform.position = points[0];
        currentPointIndex = 0;

        // 确保音频可以循环播放
        if (moveAudio != null)
        {
            moveAudio.loop = true;  // 设置音频为循环播放
        }
    }

    void Update()
    {
        // 如果不在移动中，开始移动到下一个点
        if (!isMoving)
        {
            MoveToNextPoint();
        }
        else
        {
            // 如果正在移动，继续线性移动到目标位置
            MoveToTarget();
        }
    }

    // 开始移动到下一个点
    void MoveToNextPoint()
    {
        currentPointIndex = (currentPointIndex + 1) % points.Length; // 循环移动到下一个点
        Vector2 targetPosition = points[currentPointIndex];          // 设置目标位置
        currentDirection = (targetPosition - (Vector2)transform.position).normalized; // 计算方向

        // 设置动画参数
        animator.SetFloat("DirX", currentDirection.x);
        animator.SetFloat("DirY", currentDirection.y);

        // 播放移动音频
        if (moveAudio != null && !moveAudio.isPlaying)
        {
            moveAudio.Play();  // 播放音频
        }

        // 设置移动状态
        isMoving = true;
    }

    // 使用线性插值移动到目标位置
    void MoveToTarget()
    {
        // 获取当前目标位置
        Vector2 targetPosition = points[currentPointIndex];

        // 使用插值进行平滑移动
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // 检查是否到达目标位置
        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            // 停止移动，重置状态
            transform.position = targetPosition;
            isMoving = false;

            // 停止音频
            if (moveAudio != null)
            {
                moveAudio.Stop();  // 停止音频播放
            }
        }
    }
}
