using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // 障害物の移動速度
    public float speed = 5f;

    void Update()
    {
        // 左方向へ移動する
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // 画面外（左側）に出たら自動削除してメモリを解放
        if (transform.position.x < -12f)
        {
            Destroy(gameObject);
        }
    }
}