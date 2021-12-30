using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct Highscore
{
    public string username;
    public int score;
}
[System.Serializable]
public class GameData
{
    public List<User> users;
    public Highscore highscore;
    string dataPath;

    public GameData(string path)
    {
        dataPath = path;
        Load();
    }
    void Load()
    {
        if (!File.Exists(dataPath))
        {
            users = new List<User>();
            highscore = new Highscore();
            string json = JsonUtility.ToJson(this);
            File.WriteAllText(dataPath, json);
            return;
        }
        string file = File.ReadAllText(dataPath);
        GameData loaded = JsonUtility.FromJson<GameData>(file);
        users = loaded.users;
        highscore = loaded.highscore;
    }
    public void Save()
    {
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(dataPath, json);
    }
}

[System.Serializable]
public class User
{
    public string name;
    public int score;
    public int highscore;
}

[System.Serializable]
public struct Level
{
    public int lines;
    public int rows;
    public Brick[] bricks;
    public bool IsClear => !IsHaveBricks();

    bool IsHaveBricks()
    {
        foreach (Brick item in bricks)
        {
            if (!MainManager.Instance.GetBrickData(item.id).IsIndestructible)
                return true;
        }
        return false;
    }
}

[System.Serializable]
public struct Brick
{
    public int id;
    public int lives;
}

public class MainManager : MonoBehaviour
{
    #region Singleton
    public static MainManager Instance { get; private set; }
    void CreateInstace()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [SerializeField] BrickData[] bricks;
    [SerializeField] LevelData defaultLevel;
    [SerializeField] string dataPath;

    const string levelsPath = "/levels";

    GameData data;
    public List<User> Users => data.users;
    public User CurrentUser { get; private set; }
    public Highscore Highscore
    {
        get { return data.highscore; }
        set { data.highscore = value; }
    }

    public event System.Action OnUserAdded;
    public event System.Action OnGameOver;

    private void Awake()
    {
        //a few minutes, in order to realize that defaultLevel gives an NullReferenceException when the MainManager enters into the DontDestroyOnLoad scene !!!!!!!!!!!!!!!
        if (Instance == null)
        {
            CreateInstace();
            Initialize();
            Load();
            Instance.defaultLevel.Initialize();
        }
    }
    private void Initialize()
    {
        dataPath = Application.persistentDataPath;

        if (!Directory.Exists(dataPath + levelsPath))
            Directory.CreateDirectory(dataPath + levelsPath);
    }

    public BrickData GetBrickData(int id)
    {
        if (id < 0)
            id = 0;
        return bricks[id];
    }
    public Level GetLevel()
    {
        string path = GetLevelPath(CurrentUser.name);
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<Level>(json);
    }

    #region Helpers
    private string GetLevelPath(string username)
    {
        return $"{dataPath + levelsPath}/{username}.lvl";
    }

    public bool UserNameExists(string name)
    {
        for (int i = 0; i < data.users.Count; i++)
        {
            if (data.users[i].name == name)
                return true;
        }
        return false;
    }
    #endregion

    public void StartGame(int id)
    {
        CurrentUser = data.users[id];
        SceneManager.LoadScene(1);
    }
    public void GameOver()
    {
        OnGameOver?.Invoke();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    #region Data Manipulations
    public void AddUser(string name)
    {
        User user = new User();
        user.name = name;
        user.score = 0;
        user.highscore = 0;
        data.users.Add(user);

        string path = GetLevelPath(name);
        string json = JsonUtility.ToJson(defaultLevel.Data);
        File.WriteAllText(path, json);
        Save();
        OnUserAdded?.Invoke();
    }
    public void ResetUser()
    {
        if (CurrentUser.score > CurrentUser.highscore)
            CurrentUser.highscore = CurrentUser.score;
        CurrentUser.score = 0;
        SaveLevel(defaultLevel.Data);
        Save();
    }
    public void SaveLevel(Level level)
    {
        string path = GetLevelPath(CurrentUser.name);
        string json = JsonUtility.ToJson(level);
        File.WriteAllText(path, json);
    }
    private void Save()
    {
        data.Save();
    }
    public void Load()
    {
        string path = dataPath + "/data.json";
        data = new GameData(path);
    }
    #endregion

}