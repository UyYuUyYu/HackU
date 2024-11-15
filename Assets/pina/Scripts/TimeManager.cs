using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    private float timeLimit = 90f;

    [SerializeField]
    private Text timerText;

    private float currentTime;

    private ScoreManager scoreManager;
    
    [SerializeField] private SendScorePUN sendScorePUN;

    [SerializeField] private GameObject timeUpText;

    private enum Phase
    {
        Phase1,
        Phase2,
        Phase3,
        Phase4,
        Completed
    }

    private Phase currentPhase = Phase.Phase1;


    private void Start()
    {
        scoreManager=this.GetComponent<ScoreManager>();
        currentTime = timeLimit;
        UpdateTimerText();
        Debug.Log("Time Left: " + FormatTime(currentTime));
    }

    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerText();

            if (currentTime <= 0)
            {
                currentTime = 0;
                TimeOut();
            }
            else
            {
                UpdatePhase();
                HandleSpawning();

            }
        }
    }

    private void UpdatePhase()
    {
        if (currentTime > 60f && currentPhase != Phase.Phase1)
        {
            currentPhase = Phase.Phase1;
            SwitchPhase();
        }
        else if (currentTime <= 60f && currentTime > 50f && currentPhase != Phase.Phase2)
        {
            currentPhase = Phase.Phase2;
            SwitchPhase();
        }
        else if (currentTime <= 50f && currentTime > 20f && currentPhase != Phase.Phase3)
        {
            currentPhase = Phase.Phase3;
            SwitchPhase();
        }
        else if (currentTime <= 20f && currentPhase != Phase.Phase4)
        {
            currentPhase = Phase.Phase4;
            SwitchPhase();
        }
    }

    private void SwitchPhase()
    {
        // フェーズが変更されたときの処理
        Debug.Log($"Current Phase: {currentPhase}");
    }

    private void TimeOut()
    {
        currentPhase = Phase.Completed;

        timeUpText.SetActive(true);
        SendScorePUN.myScore=scoreManager.GetScore();
        Debug.Log("Time's up!");

        SceneManager.LoadScene("Result");

    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = FormatTime(currentTime);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("Time: {0:0}:{1:00}", minutes, seconds);
    }



    //Enemy Generater~~~~~~~~~~~~~~~~~
    
    // 通常の敵オブジェクトを格納する配列（3体分）
    [SerializeField] private GameObject[] enemies = new GameObject[3];
    
    // 中ボス用のゲームオブジェクト
    [SerializeField] private GameObject midBoss;

    // ボス用のゲームオブジェクト
    [SerializeField] private GameObject boss;

    public Transform centerPosition;


    // 生成位置を決定する関数
    private Vector3 GetSpawnPosition()
    {
        int randomPosition = Random.Range(0, 2);  // 0, 1 のいずれかをランダムで取得
        float x = 0f;
        float y = 0f;
        float z = 0f;

        switch (randomPosition)
        {
            case 0:
                x = -10f;
                y = Random.Range(-6f, 6f);
                break;
            case 1:
                x = 10f;
                y = Random.Range(-6f, 6f);
                break;
        }

        return new Vector3(x, y, z);
    }

    private float spawnTimer = 5f; // 敵の生成タイミングを5秒に設定
    private float timeSinceLastSpawn = 0f; // 最後に生成してからの経過時間

    //敵の生成のタイミング調整
    private void HandleSpawning()
    {

        //フェーズ1の時
        if (currentPhase == Phase.Phase1)
        {
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= spawnTimer)
            {
                SpawnEnemy();
                timeSinceLastSpawn = 0f;
            }
        }
        //フェーズ2の時 中ボス
        else if(currentPhase == Phase.Phase2)
        {
            //SpawnMidBossEnemy();
        }
        //フェーズ3の時
        else if(currentPhase == Phase.Phase3)
        {
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= spawnTimer)
            {
                SpawnEnemy();
                SpawnEnemy();

                timeSinceLastSpawn = 0f;
            }
        }
        //フェーズ4の時 ボス
        else
        {
            //SpawnBossEnemy();
        }
    }

    public void SpawnEnemy()
    {
        // 敵をランダムに選択し、位置を取得して生成
        GameObject enemyPrefab = enemies[Random.Range(0, enemies.Length)];
        enemyPrefab.gameObject.GetComponent<EnemyMovementScript>().centerPosition = this.centerPosition;
        Vector3 spawnPosition = GetSpawnPosition();
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    public void SpawnMidBossEnemy()
    {
        // 敵をランダムに選択し、位置を取得して生成
        GameObject enemyPrefab = midBoss;
        enemyPrefab.gameObject.GetComponent<EnemyMovementScript>().centerPosition = this.centerPosition;
        Vector3 spawnPosition = GetSpawnPosition();
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
    public void SpawnBossEnemy()
    {
        // 敵をランダムに選択し、位置を取得して生成
        GameObject enemyPrefab = boss;
        enemyPrefab.gameObject.GetComponent<EnemyMovementScript>().centerPosition = this.centerPosition;
        Vector3 spawnPosition = GetSpawnPosition();
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

}

