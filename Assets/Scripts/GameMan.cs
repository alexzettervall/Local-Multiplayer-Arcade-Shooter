using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Linq;

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
        public Outfit Outfit;
    }
    [SerializeField] private PlayerJoinHandler playerJoinHandler;
    private Dictionary<int, PlayerData> players = new Dictionary<int, PlayerData>();
    [SerializeField] private bool fillWithBots = false;
    [SerializeField] private int maxPlayers = 8;
    [SerializeField] private Level level;
    [SerializeField] private int pointsNeeded = 10;
    [SerializeField] private RectTransform[] uiPointSpots;
    [Header("Score UI")]
    [SerializeField] private GameObject scoreUIHolder;
    [SerializeField] private ScoreUI scoreUI;
    [SerializeField] private bool gameOver = false;
    [SerializeField] private int gameWinner;
    [SerializeField] private ParticleEmitter confettiParticles;


    // Name of the Full Screen Pass Renderer Feature
    public string fullScreenPassName = "Pixelation";

    // Name of the float property in the Shader Graph
    public string floatPropertyName = "Pixel Size";
 

    public float zoneTime = 30f;
    public float zoneGracePeriod = 5f;
    public float gasTickTime = 1f;
    public int gasDamage = 15;
    public UnityEngine.Material pixelation;

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
        gameWinner = -1;
        gameOver = false;
        ResetPlayerScores();
        if (fillWithBots) {
            PopulateBots();
        }
        SceneManager.LoadScene("Game");
        SceneManager.sceneLoaded += OnGameSceneLoaded;
    }
    public void EndGame(PlayerData winner)
    {
        gameWinner = winner.PlayerID;
        gameOver = true;
    }

    public void StartRound()
    {
        GameObject levelWorld = GetLevelPrefab(LevelCategory.Forest);
        GameObject obj = Instantiate(levelWorld, GameObject.FindGameObjectWithTag("Level Holder").transform);
        Level level = obj.GetComponent<Level>();

        DynamicCamera camera = FindObjectOfType<DynamicCamera>();
        camera.level = level;
        camera.UpdateCameraPositionAndSize(true);
    }
    private GameObject GetLevelPrefab(LevelCategory? category = null)
    {
        /*
        Get a random level prefab of a specific category.
        */
        var available = GameAssets.i.levels.Where(level =>
        {
            Level levelData = level.GetComponent<Level>();
            return levelData.levelEnabled && (category == null || levelData.category == category);
        }).ToArray();

        return available[Random.Range(0, available.Length)];
    }
    private GameObject GetSpecificLevel(string levelName)
    {
        /*
        Get a level prefab of a specific level.
        */
        return GameAssets.i.levels.First(level => level.name == levelName);
    }
    private void ResetPlayerScores()
    {
        foreach (PlayerData player in GetAllPlayers())
        {
            player.Score = 0;
        }
        scoreUI.MoveScores(pointsNeeded);
    }
    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            StartRound();
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

        // MAGIC NUMBERS YAAAY
        float transitionTime = 0.25f;
        float targetSize = 32f;
        float minSize = 1f;

        // Pixilate
        StartCoroutine(PixelateTransition(minSize, targetSize, transitionTime));
        yield return new WaitForSeconds(transitionTime);

        // Fully pixelation (middle of transition)
        Destroy(level.gameObject);

        bool showScores = true;
        if (showScores)
        {
            // Update and show scores
            scoreUI.UpdateScoreUI(players, pointsNeeded);
            scoreUIHolder.SetActive(true);

            // Depixilate
            StartCoroutine(PixelateTransition(targetSize, minSize, transitionTime));
            yield return new WaitForSeconds(transitionTime);

            // Play animation
            yield return new WaitForSeconds(1f);
            float timeToWalk = scoreUI.MoveScores(pointsNeeded);
            yield return new WaitForSeconds(timeToWalk + 1f);

            // Check for winners
            foreach (PlayerData player in players.Values)
            {
                if (player.Score >= pointsNeeded)
                {
                    confettiParticles.transform.position = scoreUI.playerScoreUIs[player.PlayerID].player.transform.position;
                    StartCoroutine(confettiParticles.EmitBursts());
                    EndGame(player);
                    yield return new WaitForSeconds(5);
                }
            }

            // Pixilate
            StartCoroutine(PixelateTransition(minSize, targetSize, transitionTime));
            yield return new WaitForSeconds(transitionTime);
            scoreUIHolder.SetActive(false);
        }

        if (!gameOver)
        {
            StartRound();
        }
        else
        {
            SceneManager.LoadScene("Menu");
            SceneManager.sceneLoaded += OnGameSceneLoaded;
        }

        // Depixilate
        StartCoroutine(PixelateTransition(targetSize, minSize, transitionTime));
        yield return new WaitForSeconds(transitionTime);
    } 

    public IEnumerator PixelateTransition(float start, float end, float transitionTime)
    {
        float elapsed = 0f;
        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            pixelation.SetFloat("_Pixel_Size", Mathf.Lerp(start, end, elapsed / transitionTime));
            yield return null; // Wait until the next frame.
        }
        pixelation.SetFloat("_Pixel_Size", end);
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

        Outfit outfit = GameAssets.i.playerOutfits[Random.Range(0, GameAssets.i.playerOutfits.Length)];
    
        players[playerID] = new PlayerData
        {
            PersistentPlayer = persistentPlayer,
            PlayerID = playerID,
            PlayerName = playerName,
            PlayerColor = color,
            Outfit = outfit,
            Score = 0
        };

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

    public Item GetClosestItemInRange(Vector2 position, float radius, Item ignore)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius, 1 << 6);

        Collider2D closestCollider = null;
        float closestDist = float.MaxValue;

        foreach (Collider2D collider in colliders)
        {
            Item item = collider.GetComponent<Item>();
            if (item == null) continue;
            if (item == ignore) continue;
            
            float dist = Vector2.Distance(collider.transform.position, position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestCollider = collider;
            }
        }

        if (closestCollider != null)
        {
            return closestCollider.GetComponent<Item>();
        }

        return null;
    }
}
