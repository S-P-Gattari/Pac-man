using System.Collections;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    private Animator animator;    // Animator组件
    public float stateDuration = 3f; // 每个状态持续时间
    private float stateTimer = 0f;   // 计时器
    private int currentState = 0;    // 当前状态索引
    private string[] states = { "Right", "Up", "Left", "Down", "Scared", "ScaredOver", "Die" }; // 状态数组

    void Start()
    {
        animator = GetComponent<Animator>(); // 获取Animator组件
        animator.Play(states[currentState]);  // 开始播放第一个状态
    }

    void Update()
    {
        // 计时器增加
        stateTimer += Time.deltaTime;

        // 当计时器达到stateDuration时切换到下一个状态
        if (stateTimer >= stateDuration)
        {
            // 重置计时器
            stateTimer = 0f;

            // 切换到下一个状态
            currentState = (currentState + 1) % states.Length;  // 循环播放
            animator.Play(states[currentState]);  // 播放新的动画状态
        }
    }
}
