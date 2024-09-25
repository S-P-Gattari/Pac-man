using UnityEngine;

public class PacStudentCollisionHandler : MonoBehaviour
{
    public GameObject prefab0;  // 引用预制体0

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 检查碰撞对象的标签是否是 RP 或 PP
        if (collision.gameObject.tag == "RP" || collision.gameObject.tag == "PP")
        {
            // 获取碰撞对象的位置
            Vector3 objectPosition = collision.gameObject.transform.position;

            // 删除原来的物体
            Destroy(collision.gameObject);

            // 在原位置实例化预制体0
            Instantiate(prefab0, objectPosition, Quaternion.identity);

            Debug.Log("替换物体为预制体0");
        }
    }
}
