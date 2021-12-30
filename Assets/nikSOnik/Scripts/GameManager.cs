using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Rigidbody Ball;

    public Text ScoreText;
    public Text HighcoreText;
    public GameObject GameOverText;
    public GameObject WinText;
    [SerializeField] Button toMenu;

    private bool m_Started = false;
    private bool m_GameOver = false;

    MainManager manager;

    User user;
    Level level;
    Highscore highscore;
    private void Awake()
    {
        manager = MainManager.Instance;
        manager.OnGameOver += GameOver;
    }
    public void Exit()
    {
        manager.SaveLevel(level);
        SceneManager.LoadScene(0);
    }
    private void OnDestroy()
    {
        manager.OnGameOver -= GameOver;
    }
    void Start()
    {
        user = manager.CurrentUser;
        highscore = manager.Highscore;

        UpdateScoreUI();
        BuildLevel();
    }
    private void BuildLevel()
    {
        level = manager.GetLevel();
        const float offsetX = -1.5f;
        const float offsetY = 2.5f;
        const float height = 0.3f;
        const float width = 0.6f;
        for (int line = 0; line < level.lines; line++)
        {

            for (int row = 0; row < level.rows; row++)
            {
                int index = line * level.rows + row;
                if (level.bricks[index].id != -1)
                {
                    Vector3 position = new Vector3(offsetX + width * row, offsetY + line * height, 0);

                    Brick info = level.bricks[index];

                    BrickData data = manager.GetBrickData(info.id);

                    BrickObject brick = data.CreateInstance();
                    brick.Initialize(data, info.lives, index);
                    brick.SetPosition(position);

                    if (!data.IsIndestructible)
                    {
                        brick.OnDestroyed.AddListener((int points) => { BirckDestroyed(points, index); });
                        brick.OnBrickHitted.AddListener((int id) => { BrickHitted(index); });
                    }
                }
            }
        }
    }
    private void UpdateScoreUI()
    {
        ScoreText.text = $"Score: {user.score}";
        string highscoreText= $"Best score: {highscore.username}:{highscore.score}";
        if (highscore.username == user.name)
            highscoreText = $"You are the best! Max score: {user.highscore}";
        HighcoreText.text = highscoreText;
    }

    private void Update()
    {
        if (!m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Exit();
            if (!m_Started)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    m_Started = true;
                    float randomDirection = Random.Range(-1.0f, 1.0f);
                    Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                    forceDir.Normalize();

                    Ball.transform.SetParent(null);
                    Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            manager.ResetUser();
            Reload();
        }
    }
    void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void BirckDestroyed(int point, int index)
    {
        user.score += point;
        level.bricks[index].id = -1;
        if (user.score > highscore.score)
        {
            highscore = new Highscore() { username = user.name, score = user.score };
            manager.Highscore = highscore;
        }
        if (user.score > user.highscore)
            user.highscore = user.score;
        UpdateScoreUI();
        if (level.IsClear)
            LevelCleared();
    }
    void LevelCleared()
    {
        Destroy(Ball.gameObject);
        m_GameOver = true;
        StartCoroutine(Restart());
    }
    IEnumerator Restart()
    {
        WinText.SetActive(true);
        yield return new WaitForSeconds(2f);
        Reload();
    }
    void BrickHitted(int index)
    {
        level.bricks[index].lives--;
    }
    public void GameOver()
    {
        toMenu.interactable = false;
        m_GameOver = true;
        GameOverText.SetActive(true);
    }
}