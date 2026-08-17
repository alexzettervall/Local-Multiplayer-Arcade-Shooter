using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Level : MonoBehaviour
{
    public LevelCategory category;
    public bool levelEnabled = false;

    [SerializeField] private Vector2 levelSize;
    [SerializeField] private GameMode gameMode;
    [SerializeField] private NavGraph navGraph;
    [SerializeField] private List<PlayerRespawn> playerRespawns = new List<PlayerRespawn>();
    [SerializeField] private List<GameMan.PlayerData> players = new List<GameMan.PlayerData>();
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject readyText;
    [SerializeField] private GameObject goText;
    private Dictionary<GameMan.PlayerData, int> playerScores = new Dictionary<GameMan.PlayerData, int>();
    private GameAssets.GameModeData gameModeData;
    private bool started = false;
    private bool ended = false;
    private bool zoneSpawningActive = false;

    private Vector2 currentSize;

    private int playerSpawnIndex = 0;

    private float timer;

    private List<Waypoint> waypoints = new List<Waypoint>();
    private void Awake()
    {
        navGraph.BuildCache();
        currentSize = new Vector2(levelSize.x, levelSize.y);
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null) return;
        if (navGraph == null) return;

        Color nodeColor = new Color(0, 0, 1f);
        Color connectionColor = new Color(0, 1f, 0f);

        Gizmos.color = nodeColor;

        // Draw nodes
        foreach (var wp in navGraph.nodes)
        {
            Gizmos.DrawSphere(wp.position, 0.05f);
        }

        // Draw connections
        Gizmos.color = connectionColor;
        foreach (WaypointConnection connection in navGraph.connections)
        {
            Gizmos.DrawLine(navGraph.GetWaypoint(connection.a).position, navGraph.GetWaypoint(connection.b).position);
        }
    }

    private void Start()
    {
        Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Level Canvas.prefab").Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject obj = Instantiate(handle.Result, transform);
                LevelCanvas levelCanvas = obj.GetComponent<LevelCanvas>();
                readyText = levelCanvas.GetReadyText();
                goText = levelCanvas.GetGoText();
                SetPlayers(GameMan.Instance.GetAllPlayers());
                GameMan.Instance.SetLevel(this);
                StartCoroutine(StartLevel());
            }
        };
    }
    private void Update()
    {
        if (started)
        {
            timer += Time.deltaTime;
        }
        

        List<PlayerRespawn> toRemove = new List<PlayerRespawn>();
        foreach (PlayerRespawn playerRespawn in playerRespawns)
        {
            playerRespawn.time -= Time.deltaTime;
            if (playerRespawn.time <= 0)
            {
                SpawnPlayer(playerRespawn.playerID);
                toRemove.Add(playerRespawn);
            }
        }
        foreach (PlayerRespawn playerRespawn in toRemove)
        {
            playerRespawns.Remove(playerRespawn);
        }
    }
    IEnumerator SpawnZone()
    {
        
        // Wait for the grace period.
        zoneSpawningActive = true;
        yield return new WaitForSeconds(GameMan.Instance.zoneGracePeriod);
        int rings = Mathf.CeilToInt(Mathf.Max(levelSize.x, levelSize.y) / 2f) + 1;
        GameObject prefab = GameAssets.i.poisonGas;
        AudioMan.PlaySound(Sound.GasStart);
        int minSize = 4;
        float interval = (GameMan.Instance.zoneTime - GameMan.Instance.zoneGracePeriod) / (rings);
        int width = Mathf.CeilToInt(levelSize.x / 2f);
        int height = Mathf.CeilToInt(levelSize.y / 2f);
        for (int x = 0; x < width; x++)
        {
            SpawnMirrored(prefab, new Vector3(x, levelSize.y / 2f, 0));
        } 
        for (int y = 0; y < height+1; y++)
        {
            SpawnMirrored(prefab, new Vector3(levelSize.x / 2f, y, 0));
        }
        yield return new WaitForSeconds(interval);
        for (int ring = rings-2; ring >= minSize; ring--)
        {
            
            

            if (ring < height)
            {
                for (int x = 0; x < ring; x++)
                {
                    SpawnMirrored(prefab, new Vector3(x, ring, 0));
                }
            }
            for (int y = 0; y < Mathf.Min(height, ring+1); y++)
            {
                SpawnMirrored(prefab, new Vector3(ring, y, 0));
            }
            
            
            // Wait
            yield return new WaitForSeconds(interval);
        }

        /*int maxRings = Mathf.CeilToInt(Mathf.Min(levelSize.x, levelSize.y) / 2f) + 5;
        int squareSize = 10;
        float interval = (GameMan.Instance.zoneTime - GameMan.Instance.zoneGracePeriod) / (maxRings); // Time between each spawn.
        Vector2 center = transform.position;
        GameObject prefab = GameAssets.i.poisonGas;
        AudioMan.PlaySound(Sound.GasStart);

        int dif = Mathf.RoundToInt(levelSize.x - levelSize.y);

        Vector2 targetSize = currentSize; // Start with the initial size.
        for (int ring = maxRings; ring >= 0; ring--)
        {
            targetSize = new Vector2((ring) * 2f + dif, ring * 2f); // Update the target size.

            // Smoothly interpolate currentSize to targetSize over the interval duration.
            float elapsed = 0f;
            Vector2 initialSize = currentSize;

            while (ring < maxRings-4 && ring != maxRings && elapsed < interval)
            {
                elapsed += Time.deltaTime;
                currentSize = Vector2.Lerp(initialSize, targetSize, elapsed / interval);
                yield return null; // Wait until the next frame.
            }
            // Spawn fire in the current ring.
            for (int x = -ring - dif/2; x <= ring + dif/2; x++)
            {
                for (int y = -ring; y <= ring; y++)
                {
                    if (Mathf.Abs(x) == ring + dif/2 || Mathf.Abs(y) == ring) // Ensure it's the boundary of the ring.
                    {
                        Vector2 spawnPos = new Vector2(center.x + x, center.y + y);

                        // Check if within bounds of the level.
                        if (spawnPos.x >= 0 && spawnPos.x < levelSize.x && spawnPos.y >= 0 && spawnPos.y < levelSize.y)
                        {
                            Instantiate(prefab, new Vector3(spawnPos.x, spawnPos.y, 0), Quaternion.identity, transform);
                            Instantiate(prefab, new Vector3(-spawnPos.x, spawnPos.y, 0), Quaternion.identity, transform);
                            Instantiate(prefab, new Vector3(-spawnPos.x, -spawnPos.y, 0), Quaternion.identity, transform);
                            if (ring != 0)
                            {
                                Instantiate(prefab, new Vector3(spawnPos.x, -spawnPos.y, 0), Quaternion.identity, transform);
                            }
                        }
                    }
                }
            }
        }*/
    }
    public void SpawnMirrored(GameObject prefab, Vector3 position)
    {
        Instantiate(prefab, position, Quaternion.identity, transform);
        Instantiate(prefab, -position, Quaternion.identity, transform);
        Vector3 topLeft = position;
        topLeft.x *= -1;
        Instantiate(prefab, topLeft, Quaternion.identity, transform);
        Vector3 topRight = position;
        topRight.y *= -1;
        Instantiate(prefab, topRight, Quaternion.identity, transform);
    }
    
    public Vector2 GetCurrentSize()
    {
        return currentSize;
    }
    public Vector2 GetSize()
    {
        return levelSize;
    }
    public void SetPlayers(List<GameMan.PlayerData> players)
    {
        this.players = players;
    }
    public void AddPlayer(GameMan.PlayerData playerData)
    {
        playerScores[playerData] = 0;
        SpawnPlayer(playerData.PlayerID);
    }
    public IEnumerator StartLevel()
    {
        ShuffleSpawnPoints();
        gameModeData = GameAssets.i.GetGameModeData(gameMode);
        foreach (GameMan.PlayerData playerData in players)
        {
            playerScores[playerData] = 0;
        } 
        foreach (GameMan.PlayerData playerData in players)
        {
            SpawnPlayer(playerData.PlayerID);
        }

        yield return new WaitForSeconds(1f);
        readyText.SetActive(true);
        yield return new WaitForSeconds(0.33f);
        readyText.SetActive(false);
        goText.SetActive(true);
        yield return new WaitForSeconds(0.33f);
        goText.SetActive(false);

        started = true;
        if (GameMan.Instance.doGasZone)
        {
            StartCoroutine(SpawnZone());
        }
    }
    public IEnumerator EndLevel()
    {
        ended = true;
        yield return new WaitForSeconds(0f);
        Player player = FindObjectOfType<Player>();
        GameMan.Instance.EndRound(player);
    }
    public void OnPlayerDeath(Player player)
    {
        if (ended)
        {
            return;
        }
        GameAssets.GameModeData gameModeData = GameAssets.i.GetGameModeData(gameMode);
        if (gameModeData.respawn)
        {
            playerRespawns.Add(new PlayerRespawn(player.playerID, gameModeData.respawnTime));
        }
        if (gameModeData.killPoints)
        {
            Entity lastDamager = player.GetLastDamager();
            if (lastDamager is Player && player != lastDamager)
            {
                Player playerDamager = (Player)lastDamager;
                playerScores[GameMan.Instance.GetPlayer(playerDamager.playerID)] += 1;
            }
        }
        if (gameMode == GameMode.Deathmatch)
        {
            playerScores[GameMan.Instance.GetPlayer(player.playerID)] -= FindObjectsOfType<Player>().Length;
            if (GetAlivePlayers() <= 1)
            {
                StartCoroutine(EndLevel());
            }
        } 
    }
    public void OnStructureDestroyed()
    {
        navGraph.BuildCache();
    }
    public void SpawnPlayer(int playerID)
    {
        if (playerSpawnIndex >= spawnPoints.Length)
        {
            playerSpawnIndex = 0;
        }
        Transform spawnPoint = spawnPoints[playerSpawnIndex];
        playerSpawnIndex++;
        
        GameMan.PlayerData playerData = GameMan.Instance.GetPlayer(playerID);
        float spawnPositionRandomization = GameAssets.i.spawnPositionRandomization;
        Vector2 spawnPosition = (Vector2)spawnPoint.position + new Vector2(Random.Range(-spawnPositionRandomization, spawnPositionRandomization), Random.Range(-spawnPositionRandomization, spawnPositionRandomization));
        GameObject newPlayer = Instantiate(GameAssets.i.playerPrefab, spawnPosition, Quaternion.identity, transform);
        Player player = newPlayer.GetComponent<Player>();
        player.InitializePlayer(playerID);
        playerData.PersistentPlayer.SetEntity(player);
    }
    public void ShuffleSpawnPoints()
    {
        for (int i = spawnPoints.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Transform temp = spawnPoints[i];
            spawnPoints[i] = spawnPoints[randomIndex];
            spawnPoints[randomIndex] = temp;
        }
    }
    public void CreateExplosion(Vector2 position, float damage, float radius, Entity causer, int shrapnel)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, radius);

        Dictionary<Entity, int> entityDamages = new Dictionary<Entity, int>();
        foreach (Collider2D hitCollider in hitColliders)
        {
            Entity entity = hitCollider.GetComponent<Entity>();
            if (entity == null)
            {
                continue;
            }
            if (entity is Bullet)
            {
                continue;
            }
            float distance = Vector2.Distance(position, hitCollider.transform.position);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(position, (Vector2)hitCollider.transform.position - position, distance, GameAssets.i.structuresOnly);
            if (raycastHit2D.collider != null && raycastHit2D.collider != hitCollider)
            {
                continue;
            }
            float damageFactor = Mathf.Clamp01(1 - (distance / radius));
            int finalDamage = Mathf.RoundToInt(damage * damageFactor);
            entityDamages[entity] = finalDamage;
            entity.GetRigidbody().AddForce(((Vector2)entity.transform.position - position) * damageFactor * 32f, ForceMode2D.Impulse);
        }
        foreach (Entity entity in entityDamages.Keys)
        {
            entity.Damage(entityDamages[entity], causer, DamageSource.Explosion);
        }
        Instantiate(GameAssets.i.cratorPrefab, position, Quaternion.identity, transform);
        GameObject explosionGradient = Instantiate(GameAssets.i.explosionGradient, position, Quaternion.identity);
        explosionGradient.transform.localScale = new Vector3(radius*2, radius*2, 1f);
        Destroy(explosionGradient, 3f);
        AudioMan.PlaySound(Sound.Explosion);
        BulletSpawner.ShootBullet(shrapnel, GameAssets.i.shrapnel, position, 10, 12, 180, 0, 10, causer);
    }
    public bool IsStarted()
    {
        return started;
    }
    public int GetAlivePlayers()
    {
        int count = 0;
        Player[] players = FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if (!player.IsDead())
            {
                count++;
            }
        }
        return count;
    }
    public AStar.Path FindPath(Vector2 start, Vector2 goal, float moveSpeed, float dps) {
        return navGraph.FindPath(start, goal, moveSpeed, dps);
    }
    public float FindDistance(Vector2 start, Vector2 goal, out List<Vector2> path) {
        // this should pathfind but computation
        path = null;
        return Vector2.Distance(start, goal);
        //return navGraph.FindDistance(start, goal, out path);
    }
    public class PlayerRespawn
    {
        public int playerID;
        public float time;
        public PlayerRespawn(int playerID, float time)
        {
            this.playerID = playerID;
            this.time = time;
        }
    }
}
