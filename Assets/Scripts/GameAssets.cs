using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;
    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _i;
        }
    }
    public Gradient bulletGradient;
    public GameObject iceCubeIconPrefab;
    public GameObject fireIconPrefab;
    public Outfit[] playerOutfits;
    public Sprite[] playerSkins;
    public HandSet[] playerHandSkins;
    public LootPools lootPools;
    public GameObject lootPrefab;
    public AISettings AISettings;
    public float spawnPositionRandomization = 0f;
    public GameObject persistentPlayer;
    public GameObject plus1Text;
    public GameObject bloodSplatter;
    public GameObject bloodResidue;
    public GameObject poisonGas;
    public GameObject eternalFire;
    public GameObject shrapnel;
    public GameObject cratorPrefab;
    public GameObject explosionGradient;
    public Sprite circle;
    public Color itemCircleNormal;
    public Color itemCircleDanger;
    public Color itemCircleUseless;
    public LayerMask structuresOnly;
    public LayerMask playersOnly;
    public GameObject playerScoreUI;
    public GameObject[] levels;
    public List<Color> playerColors;
    public GameObject playerPrefab;
    public GameObject ammoPrefab;
    public GameObject gunPrefab;
    public GameObject itemBackgroundPrefab;
    public GunData[] gunDatas;
    public GameModeData[] gameModeDatas;
    public AmmoSprite[] ammoSprites;
    public AmmoLimit[] ammoLimits;
    public SoundClip[] soundClips;

    public Sprite GetGunSprite(GunType gunType)
    {
        foreach (GunData gunData in gunDatas)
        {
            if (gunData.gunType == gunType)
            {
                return gunData.sprite;
            }
        }
        return null;
    }
    public GunData GetGunData(GunType gunType)
    {
        foreach (GunData gunData in gunDatas)
        {
            if (gunData.gunType == gunType)
            {
                return gunData;
            }
        }
        return null;
    }
    public GameModeData GetGameModeData(GameMode gameMode)
    {
        foreach (GameModeData gameModeData in gameModeDatas)
        {
            if (gameModeData.gameMode == gameMode)
            {
                return gameModeData;
            }
        }
        return null;
    }
    public Sprite GetAmmoSprite(AmmoType ammoType)
    {
        foreach (AmmoSprite ammoSprite in ammoSprites)
        {
            if (ammoSprite.ammoType == ammoType)
            {
                return ammoSprite.sprite;
            }
        }
        return null;
    }
    public int GetAmmoLimit(AmmoType ammoType)
    {
        foreach(AmmoLimit ammoLimit in ammoLimits)
        {
            if (ammoLimit.ammoType == ammoType)
            {
                return ammoLimit.limit;
            }
        }
        return 0;
    }
    public AudioClip GetSoundClip(Sound sound)
    {
        foreach (SoundClip soundClip in soundClips)
        {
            if (soundClip.sound == sound)
            {
                return soundClip.clips[Random.Range(0, soundClip.clips.Length)];
            }
        }
        return null;
    }

    [System.Serializable]
    public class AmmoSprite
    {
        public AmmoType ammoType;
        public Sprite sprite;
    }
    [System.Serializable]
    public class AmmoLimit
    {
        public AmmoType ammoType;
        public int limit;
    }
    [System.Serializable]
    public class GunData
    {
        public GunType gunType;
        public Sprite sprite;
        public Sprite worldSprite;
        public Color color;
        public GameObject bulletPrefab;
        public bool auto;
        public float spread;
        public float minVelocity;
        public float maxVelocity;
        public int damage;
        public int amount;
        public float fireRate;
    }
    [System.Serializable]
    public class GameModeData
    {
        public GameMode gameMode;
        public bool respawn;
        public float respawnTime;
        public float gameTime;
        public bool timed;
        public bool killPoints;
    }
    [System.Serializable]
    public class SoundClip
    {
        public Sound sound;
        public AudioClip[] clips;
    }
}

