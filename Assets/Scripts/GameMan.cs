using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameMan : MonoBehaviour
{
    public static GameMan Instance;

    public class PlayerData
    {
        public int PlayerID;
        public PersistentPlayer PersistentPlayer;
        public string PlayerName;
        public int Score;
        public Color PlayerColor;
        public Sprite Skin;
        public HandSet HandSkin;
        public PlayerScoreUI PlayerScoreUI;
    }
    [SerializeField] private PlayerJoinHandler playerJoinHandler;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject playerScoresHolder;
    private Dictionary<int, PlayerData> players = new Dictionary<int, PlayerData>();
    [SerializeField] private bool fillWithBots = false;
    [SerializeField] private int maxPlayers = 8;
    [SerializeField] private Level level;
    [SerializeField] private int pointsNeeded = 10;
    [SerializeField] private RectTransform[] uiPointSpots;


    // Name of the Full Screen Pass Renderer Feature
    public string fullScreenPassName = "Pixelation";

    // Name of the float property in the Shader Graph
    public string floatPropertyName = "Pixel Size";
 

    public float zoneTime = 30f;
    public float zoneGracePeriod = 5f;
    public float gasTickTime = 1f;
    public int gasDamage = 15;
    public UnityEngine.Material pixelation;

    private PlayerData winner;
    private List<Color> playerColors = new List<Color>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Init()
    {
        playerColors = GameAssets.i.playerColors;
        AudioMan.LoadAllSounds();
        pixelation.SetFloat("_Pixel_Size", 1f);
    }
    public void StartGame()
    {
        winner = null;
        ResetPlayerScores();
        if (fillWithBots) {
            PopulateBots();
        }
        SceneManager.LoadScene("Game");
        SceneManager.sceneLoaded += OnGameSceneLoaded;
    }
    public IEnumerator EndGame(PlayerData winner)
    {
        yield return new WaitForSeconds(1f);
        canvas.SetActive(false);
        SceneManager.LoadScene("Menu");
        SceneManager.sceneLoaded += OnGameSceneLoaded;
    }

    public void StartRound()
    {
        GameObject levelWorld = GameAssets.i.levels[Random.Range(0, GameAssets.i.levels.Length)];
        GameObject obj = Instantiate(levelWorld, GameObject.FindGameObjectWithTag("Level Holder").transform);
        Level level = obj.GetComponent<Level>();

        DynamicCamera camera = FindObjectOfType<DynamicCamera>();
        camera.level = level;
        camera.UpdateCameraPositionAndSize(true);
    }
    private void ResetPlayerScores()
    {
        foreach (PlayerData player in GetAllPlayers())
        {
            player.Score = 0;
        }
    }
    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            GameObject levelWorld = GameAssets.i.levels[Random.Range(0, GameAssets.i.levels.Length)];
            Instantiate(levelWorld, GameObject.FindGameObjectWithTag("Level Holder").transform); 
            SceneManager.sceneLoaded -= OnGameSceneLoaded;
        }
        else if (scene.name == "Menu")
        {
            SceneManager.sceneLoaded -= OnGameSceneLoaded;
        }
    }
    private IEnumerator LevelTransitionCoroutine(Player winner)
    {
        yield return new WaitForSeconds(1.5f);
        if (winner != null)
        {
            Instantiate(GameAssets.i.plus1Text, winner.transform.position, Quaternion.identity, winner.transform);
            GetPlayer(winner.playerID).Score++;
        }
        yield return new WaitForSeconds(1f);

        float elapsed = 0f;
        float transitionTime = 1f;
        float targetSize = 32f;
        float minSize = 1f;

        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            pixelation.SetFloat("_Pixel_Size", Mathf.Lerp(minSize, targetSize, elapsed / transitionTime));
            yield return null; // Wait until the next frame.
        }

        // Fully pixelation (middle of transition)
        Destroy(level.gameObject);
        StartRound();

        elapsed = 0f;
        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            pixelation.SetFloat("_Pixel_Size", Mathf.Lerp(targetSize, minSize, elapsed / transitionTime));
            yield return null; // Wait until the next frame.
        }
        pixelation.SetFloat("_Pixel_Size", 1f);
    } 

    public void EndRound(Player winner)
    { 
        StartCoroutine(LevelTransitionCoroutine(winner));
    }    
    public void AddPlayer(PersistentPlayer persistentPlayer, int playerID, string playerName)
    {
        bool canAdd = true;
        if (players.Count >= maxPlayers) {
            canAdd = false;
            /*// Return unless there is a bot to replace
            foreach (PlayerData playerData in players.Values) {
                if (playerData.PersistentPlayer.isBot) {
                    // Replace the bot
                    RemovePlayer(playerData.PlayerID);
                    playerData.PersistentPlayer.isBot = false;
                    canAdd = true;
                    break;
                }
            }*/
        }
        if (!canAdd) {
            return;
        }
        if (players.ContainsKey(playerID))
        {
            return;
        }
        Color color = new Color(0, 0, 0);
        if (playerColors.Count > 0) {
            color = playerColors[Random.Range(0, playerColors.Count)];
            playerColors.Remove(color);
        }

        Sprite skin = GameAssets.i.playerSkins[Random.Range(0, GameAssets.i.playerSkins.Length)];
        HandSet handSkin = GameAssets.i.playerHandSkins[Random.Range(0, GameAssets.i.playerHandSkins.Length)];
        
        GameObject playerScore = Instantiate(GameAssets.i.playerScoreUI, playerScoresHolder.transform);
        players[playerID] = new PlayerData
        {
            PersistentPlayer = persistentPlayer,
            PlayerID = playerID,
            PlayerName = playerName,
            PlayerColor = color,
            Skin = skin,
            HandSkin = handSkin,
            Score = 0,
            PlayerScoreUI = playerScore.GetComponent<PlayerScoreUI>()
        };
        players[playerID].PlayerScoreUI.SetColor(color);

        if (level != null)
        {
            level.AddPlayer(players[playerID]);
        }
    }

    public void RemovePlayer(int playerID)
    {
        if (players.ContainsKey(playerID))
        {
            players.Remove(playerID);
        }
    }

    public void UpdatePlayerScore(int playerID, int scoreDelta)
    {
        if (players.ContainsKey(playerID))
        {
            players[playerID].Score += scoreDelta;
        }
    }

    public List<PlayerData> GetAllPlayers()
    {
        return new List<PlayerData>(players.Values);
    }

    public PlayerData GetPlayer(int playerID)
    {
        return players.ContainsKey(playerID) ? players[playerID] : null;
    }
    public Level GetLevel()
    {
        return level;
    }
    public void SetLevel(Level level)
    {
        this.level = level;
    }

    public void PopulateBots() {
        int bots = maxPlayers - players.Count;
        for (int i = 0; i < bots; i++) {
            GameObject persistentPlayer = Instantiate(GameAssets.i.persistentPlayer);
        }
    }
}
