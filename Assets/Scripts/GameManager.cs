using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using QFramework;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    public GameObject enemyPrefab;
    public GameObject endLinePrefab;
    public GameObject scorePrefab;
    public GameObject haloPrefab;
    public GameObject snowBallPrefab;
    private Queue<GameObject> haloPool = new Queue<GameObject>();

    private Queue<Score> scorePool = new Queue<Score>();
    private Queue<Enemy> enemyPool = new Queue<Enemy>();

    private List<Enemy> allEnemy = new List<Enemy>();
    public Player player;
    public Flag flag;
    public int enemyCount = 50;
    public int totalCount = 0;
    public float bornPosX = 0f;
    public float bornPosY = 0f;
    public float CamerMoveSpeed = 2;

    public float originEnemyBornHeight = 0f;
    private float curHeight = 0f;
    private int themeColorIndex = -1;
    private int enemyColorIndex = -1;
    public Color CurEnemyColor = Color.cyan;
    public Color CurThemeColor = Color.blue;
    private Color[] themeColorArr = { Color.cyan, Color.blue, Color.green, Color.gray, Color.yellow };
    private Color[] enemyColorArr = { Color.cyan, Color.blue, Color.green, Color.gray, Color.yellow };
    public enum GameState
    {
        BeforeStart,
        Running,
        PlayerDie,
        GameEnd,
    }
    public int CurLevel = 1;
    public int HistoryProgress = 0;
    public bool NeedPlayMusic = false;
    public bool NeedShowTips = false;
    public float finishHeight = -30f;
    private float playerStartY = 0f;
    private float totalMoveLength = 0f;


    private bool stopGenerateEnemy = false;
    public int CurRebornTimes = 0;
    public GameState CurState = GameState.BeforeStart;
    public static GameManager Instance;

    public int MaxScore = 0;
    private int currentSortOrder = 1;
    public void Awake()
    {
        Instance = this;
        Camera.main.gameObject.AddComponent<CameraMove>();
        SetLevelConfig();
        InitEnemyPool();

        playerStartY = player.gameObject.transform.position.y;
        totalMoveLength = Math.Abs(finishHeight - playerStartY) - 0.6144f / 2 - 0.166f;//脚下到终点线上边的距离 , 0.6144f 终点线1/2宽，0.166f角色radius * scale
    }

    private void SetLevelConfig(bool resetColor = false)
    {
        curHeight = originEnemyBornHeight;
        CurLevel = PlayerPrefs.GetInt("CurLevel", 1);
        if (CurLevel == 1)
        {
            CurThemeColor = Color.cyan;
            CurEnemyColor = Color.gray;
        }
        else
        {
            themeColorIndex = PlayerPrefs.GetInt("ThemeColorIndex", -1);
            if (themeColorIndex == -1)
            {
                themeColorIndex = Random.Range(1, themeColorArr.Length);
            }
            else
            {
                if (resetColor)
                {
                    int nextIndex = themeColorIndex;
                    Debug.LogError("当前nextIndex：" + nextIndex);
                    do
                    {
                        themeColorIndex = Random.Range(0, themeColorArr.Length);
                    } while (nextIndex == themeColorIndex);
                }

            }
            Debug.LogError("当前themeColorIndex：" + themeColorIndex);
            PlayerPrefs.SetInt("ThemeColorIndex", themeColorIndex);
            CurThemeColor = themeColorArr[themeColorIndex];

            enemyColorIndex = PlayerPrefs.GetInt("EnemyColorIndex", -1);
            if (enemyColorIndex == -1)
            {
                enemyColorIndex = Random.Range(1, enemyColorArr.Length);
            }
            else
            {
                if (resetColor)
                {
                    int nextIndex = enemyColorIndex;
                    Debug.LogError("当前nextIndex：" + nextIndex);
                    do
                    {
                        enemyColorIndex = Random.Range(0, enemyColorArr.Length);
                    } while (nextIndex == enemyColorIndex);
                }

            }
            Debug.LogError("当前themeColorIndex：" + themeColorIndex);
            PlayerPrefs.SetInt("enemyColorIndex", enemyColorIndex);
            CurEnemyColor = enemyColorArr[enemyColorIndex];
        }
        Debug.LogError("当前怪物颜色：" + CurEnemyColor + "   当前主题颜色： " + CurThemeColor);
        MaxScore = PlayerPrefs.GetInt("MaxScore", 0);
        HistoryProgress = PlayerPrefs.GetInt("HistoryProgress", 0);
        NeedPlayMusic = PlayerPrefs.GetInt("NeedPlayMusic", 1) == 1;
        NeedShowTips = PlayerPrefs.GetInt("NeedShowTips", 1) == 1;

    }

    private void Start()
    {
        player.SetThemeColor();
    }

    private void InitEnemyPool()
    {
        Vector3 pos;
        for (int i = 0; i < enemyCount; i++)
        {
            // yi hang 5 ge
            pos = new Vector3(Random.Range(-2.5f, 2.5f), curHeight, i * 0.01f);
            GenerateEnemy(pos);
        }
    }

    private void ClearEnemyPool()
    {
        // foreach (Enemy e in enemyPool)
        // {
        //     e.gameObject.SetActive(false);
        // }
        foreach (Enemy e in allEnemy)
        {
            e.gameObject.SetActive(false);
        }
    }

    private int lineMaxCount = 5;
    private int curLineCount = 0;
    private int curLineMaxCount = 0;
    private float yRange = 0.3f;
    public void GenerateEnemy(Vector3 pos = default)
    {
        if (stopGenerateEnemy) return;
        if (curHeight - 3 < finishHeight)
        {
            stopGenerateEnemy = true;
            GameObject endline = endLinePrefab.gameObject.Instantiate();
            var h = endline.GetComponent<SpriteRenderer>().size.x;
            Debug.Log("endline: " + h);
            endline.gameObject.Position(new Vector3(0, finishHeight, 0));
            return;
        }
        if (pos == default)
        {
            pos = new Vector3(Random.Range(-2.5f, 2.5f), curHeight, totalCount * 0.01f);
        }
        curHeight = originEnemyBornHeight - totalCount / 5 * 1.5f;
        totalCount++;
        Enemy enemy;
        if (enemyPool.Count > 0)
        {
            enemy = enemyPool.Dequeue();
        }
        else
        {
            enemy = enemyPrefab.gameObject.Instantiate().GetComponent<Enemy>();
            allEnemy.Add(enemy);
        }
        enemy.gameObject.Position(pos);
        enemy.gameObject.SetActive(true);
        enemy.transform.localScale = Vector3.one * (0.15f + Random.Range(-0.02f, 0.02f));
        enemy.SetColor(CurEnemyColor , currentSortOrder);
        currentSortOrder = currentSortOrder + 2;
    }

    public void GameStart()
    {

    }

    private void Update()
    {
        if (CurState != GameState.Running) return;
        curInterval += Time.deltaTime;
        if (curInterval > intervalTime)
        {
            CurScore += Config.EachUnitScore;
            EventManager.Instance.TriggerEvent(EventKey.UpdateScore, null);
            curInterval = 0f;
            //判断生成雪球
            bool showSnowBall = Random.Range(0, 100) < Config.SnowBallAppearProbability;
            if (showSnowBall)
            {
                GameObject snowBall = snowBallPrefab.Instantiate();
                snowBall.GetComponent<SnowBall>().ShowSnowBall();
            }
        }

        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     GameObject snowBall = snowBallPrefab.Instantiate();
        //     snowBall.GetComponent<SnowBall>().ShowSnowBall();
        // }
    }

    public void GameFinish()
    {
        CurState = GameState.GameEnd;
        player.PlayerFinish();
        EventManager.Instance.TriggerEvent(EventKey.LevelFinish, null);
        PlayerPrefs.SetInt("MaxScore", 0);

        //延迟3s后跳转到下一关
        StartCoroutine(NextLevel());
    }

    private IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(2);
        EventManager.Instance.TriggerEvent(EventKey.ResetLevel, null);
    }


    public void PlayerDie()
    {
        CurState = GameState.PlayerDie;
        player.PlayerDie();
        if (CurRebornTimes == 0)
        {
            CurRebornTimes++;
            EventManager.Instance.TriggerEvent(EventKey.PlayerReborn, null);
        }
        else
        {
            CurState = GameState.GameEnd;
            EventManager.Instance.TriggerEvent(EventKey.PlayerDie, null);
        }
    }

    public void GameReborn()
    {
        CurState = GameState.BeforeStart;
        player.ResetState(false);
    }

    public void ResetGame(bool isFinish)
    {
        Debug.LogError("CurProgress: " + CurProgress + "    hei: " + HistoryProgress);
        if (CurProgress > HistoryProgress)
        {
            PlayerPrefs.SetInt("HistoryProgress", CurProgress);
        }
        if (CurScore > MaxScore)
        {
            PlayerPrefs.SetInt("MaxScore", CurScore);
            MaxScore = CurScore;
        }
        if (isFinish)
        {
            Debug.LogError("通关设置");
            CurLevel++;
            PlayerPrefs.SetInt("HistoryProgress", 0);
            PlayerPrefs.SetInt("CurLevel", CurLevel);
            flag.UpdateLevelFlag();
        }
        SetLevelConfig(isFinish);
        player.ResetState();
        ClearEnemyPool();
        GameReborn();
        stopGenerateEnemy = false;
        curHeight = originEnemyBornHeight;
        totalCount = 0;
        CurRebornTimes = 0;
        CurScore = 0;
        curInterval = 0f;
        currentSortOrder = 1;
        Camera.main.transform.position = new Vector3(0, 0, -10);
        InitEnemyPool();
        CurState = GameState.BeforeStart;
    }

    private int curTipsIndex = 0;
    private string[] tips = { "干得好！", "厉害！", "漂亮！", "帅气！" };
    public void ShowTips()
    {
        if (!NeedShowTips) return;
        curTipsIndex = curTipsIndex++ % 4;
        EventManager.Instance.TriggerEvent(EventKey.ShowTips, tips[curTipsIndex]);

    }

    public void ShowScore(Vector3 pos, int score)
    {
        Score s;
        if (scorePool == null) scorePool = new Queue<Score>();
        if (scorePool.Count == 0)
        {
            s = scorePrefab.gameObject.Instantiate().Position(pos).GetComponent<Score>();
        }
        else
        {
            s = scorePool.Dequeue();
        }
        CurScore += score;
        s.ShowScore(pos, score);
        EventManager.Instance.TriggerEvent(EventKey.UpdateScore, null);

    }

    public void RecycleScorePre(Score s)
    {
        scorePool.Enqueue(s);
    }

    public void RecycleEnemyPre(Enemy e)
    {
        e.gameObject.SetActive(false);
        enemyPool.Enqueue(e);
    }

    public int CurProgress = 0;
    public int MaxProgress = 0;

    public int CurScore = 0;
    public void UpdateProgress(float curH)
    {
        CurProgress = (int)Math.Round((curH - playerStartY) * -100 / totalMoveLength);
        // Debug.Log("Curprogress:" + CurProgress + "   curH: " + curH + "  startH: " + playerStartY + "  totalmovel: " + totalMoveLength);
        EventManager.Instance.TriggerEvent(EventKey.UpdateProgress, null);

        //随机生成雪球
    }

    private float intervalTime = 0.2f;
    private float curInterval = 0f;

}