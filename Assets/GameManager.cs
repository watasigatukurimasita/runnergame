using UnityEngine;
using System.Collections;
using TMPro; // TextMeshProを使用するため

public class GameManager : MonoBehaviour
{
    // ① 障害物（2種類）のプレハブをインスペクターで登録する配列
    public GameObject[] obstaclePrefabs;
    
    // ④ スコア表示用のUIテキスト
    public TextMeshProUGUI scoreText;

    // ゲームオーバー時に鳴らす音
    public AudioClip gameOverSound;

    private float score = 0f;
    private float spawnTimer = 0f;
    private bool isGameOver = false;
    private AudioSource audioSource;

    void Start()
    {
        // 自身のAudioSourceを取得（なければ自動追加）
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 堅牢性の確保：scoreTextがインスペクターで未設定の場合の自動補正
        if (scoreText == null)
        {
            Debug.LogWarning("GameManager: scoreText がインスペクターで割り当てられていません。自動検索を試みます。");
            scoreText = FindObjectOfType<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        // ゲームオーバー時は更新処理を行わない
        if (isGameOver) return;

        // ④ 時間経過でスコアを増やす
        score += Time.deltaTime * 10f;

        // scoreTextが割り当てられている場合のみテキストを更新（Nullエラー防護）
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
        }

        // 一定時間ごとに障害物をランダム生成（2秒間隔）
        spawnTimer += Time.deltaTime;
        if (spawnTimer > 2f)
        {
            SpawnObstacle();
            spawnTimer = 0f;
        }
    }

    void SpawnObstacle()
    {
        // プレハブ未登録時の保護策
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;

        // 配列の中からランダムで1つ選択（2種類以上の障害物に対応）
        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject selectedPrefab = obstaclePrefabs[randomIndex];

        // 地面（Y: -3.5f）の上にピッタリ乗るよう、オブジェクトの高さを考慮して生成位置を計算
        float spawnY = -3.5f; 

        // プレハブのスプライトまたはスケールから高さを自動計算
        SpriteRenderer sr = selectedPrefab.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // オブジェクトの高さの半分だけ上に移動させることで、底面が地面(Y: -3.5)に揃う
            float height = sr.bounds.size.y;
            spawnY = -3.5f + (height / 2f);
        }

        // 生成位置（X: 10f の画面右外から流れてくる）
        Instantiate(selectedPrefab, new Vector3(10f, spawnY, 0f), Quaternion.identity);
    }

    // ③ ゲームオーバー処理
    public void GameOver()
    {
        // すでにゲームオーバー処理中の場合は重複実行しない
        if (isGameOver) return;
        isGameOver = true;

        // 音を正しく鳴らし切るためにコルーチンを開始
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        // ゲームオーバー音を鳴らす
        if (gameOverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameOverSound);
            // 音がキャンセルされないよう、鳴り始めの時間を確保
            yield return new WaitForSecondsRealtime(0.5f);
        }

        // ゲームの進行（時間経過）を停止
        Time.timeScale = 0f;
        Debug.Log("Game Over!");
    }
}