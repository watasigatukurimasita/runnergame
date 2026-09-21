using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ジャンプの力
    public float jumpForce = 8f;
    
    // 【追加】ジャンプ音の音声ファイルを割り当てる変数
    public AudioClip jumpSound;

    private Rigidbody2D rb;
    private AudioSource audioSource; // 【追加】音を鳴らすコンポーネント
    private bool isGrounded = true;

    void Start()
    {
        // 自身のRigidbody 2DとAudio Sourceを取得
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // ② スペースキーでジャンプ（地面にいる時のみ）
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;

            // 【追加】ジャンプ音が設定されていれば鳴らす（音の重複再生にも対応）
            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }

    // 地面に着地した判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // ③ 障害物に当たった判定（ゲームオーバー）
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // GameManagerのGameOver関数を呼び出す
            FindObjectOfType<GameManager>().GameOver();
        }
    }
}