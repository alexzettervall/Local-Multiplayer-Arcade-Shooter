public class PlayerBotAI : BotAI
{
    public PlayerBotAI() : base(GameAssets.i.playerAISettings) {

    }

    public override Perception CreatePerception()
    {
        return new GeneralPerception(blackboard);
    }

    public override UtilityScorer CreateUtilityScorer()
    {
        return new PlayerUtilityScorer(blackboard);
    }

    public override BehaviorTree CreateBehaviorTree()
    {
        return new PlayerBehaviorTree(blackboard);
    }



    /*float directionRandomization = 0.25f;
    float updatePeriod = 2f;
    float updateTimer = 0f;
    PersistentPlayer persistentPlayer;
    bool updatingUse = false;

    public BotAI(PersistentPlayer persistentPlayer) {
        this.persistentPlayer = persistentPlayer;
    }

    public void Update() {
        updateTimer -= Time.deltaTime;
        if (updateTimer <= 0) {
            updateTimer = updatePeriod;
            UpdateDirection();
        }
        if (!updatingUse) {
            persistentPlayer.StartCoroutine(UpdateUse());
        }
        // do pickup logic
        persistentPlayer.player.OnInteract(true);
        persistentPlayer.player.OnUse(true, false);
    }

    public void UpdateDirection() {
        Player[] players = GameObject.FindObjectsOfType<Player>();
        Player closestPlayer = null;
        float closestDist = float.MaxValue;
        foreach (Player player in players) {
            if (player == persistentPlayer.player) {
                continue;
            }
            float distance = Vector2.Distance(persistentPlayer.player.transform.position, player.transform.position);
            if (distance < closestDist) {
                closestDist = distance;
                closestPlayer = player;
            }
        }
        if (closestPlayer == null) {
            return;
        }
        Vector2 target = closestPlayer.transform.position;
        Vector2 direction = target - (Vector2)persistentPlayer.player.transform.position;
        // Randomize direction slightly
        float rad = Random.Range(0, 6.282f);
        float dis = Random.Range(0f, directionRandomization);
        direction += new Vector2(dis * Mathf.Cos(rad), dis * Mathf.Sin(rad));
        direction.Normalize();

        persistentPlayer.player.OnMove(direction);
        persistentPlayer.player.OnRotate(direction, null);
        // make ai go towards other objects
    }

    public IEnumerator UpdateUse() {
        if (updatingUse) {
            yield break;
        }
        updatingUse = true;

        Item item = persistentPlayer.player.GetItem();
        if (item == null){
            updatingUse = false;
            yield break;
        }
        if (item.HasTag("deadly weapon") && item.HasTag("throwable")) {
            persistentPlayer.player.OnUse(true, false);
            yield return new WaitForSeconds(0.1f);
            persistentPlayer.player.OnUse(false, true);
            yield return new WaitForSeconds(0.5f);
            persistentPlayer.player.OnUse(false, false);
        }
        else if (item.HasTag("deadly weapon")) {
            persistentPlayer.player.OnUse(true, false);
            yield return new WaitForSeconds(1f);
            persistentPlayer.player.OnUse(false, false);
            yield return new WaitForSeconds(0.5f);
        }
        else {
            // Drop item
            persistentPlayer.player.OnDrop(true);
            yield return new WaitForSeconds(0.1f);
            persistentPlayer.player.OnDrop(false);
            yield return new WaitForSeconds(1f);
        }

        updatingUse = false;
    }*/
}
