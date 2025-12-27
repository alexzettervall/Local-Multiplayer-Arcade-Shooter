using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public static class AudioMan
{
    private static Dictionary<Sound, List<AudioClip>> soundCache = new Dictionary<Sound, List<AudioClip>>();

    private static Dictionary<Sound, float> lastPlayedTimes = new Dictionary<Sound, float>();
    private static Dictionary<Sound, float> soundPitches = new Dictionary<Sound, float>();
    private static float soundDelay = 0.04f;
    private static float soundDelayVarience = 0.01f;
 
    public static void LoadAllSounds()
    {
        foreach (Sound sound in System.Enum.GetValues(typeof(Sound)))
        {
            LoadSounds(sound);
        }
    }
    // Dynamically load all sounds for a SoundType using Addressable labels
    public static void LoadSounds(Sound sound)
    {
        string label = sound.ToString();
        if (label == "None")
        {
            soundCache[sound] = new List<AudioClip>();
            lastPlayedTimes[sound] = 0f;
            return;
        }
        Addressables.LoadAssetsAsync<AudioClip>(label, null).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                List<AudioClip> clips = new List<AudioClip>(handle.Result);
                soundCache[sound] = clips;
                lastPlayedTimes[sound] = 0f;
                soundPitches[sound] = 1f;

                // Hardcoded Pitches
                if (sound == Sound.ShootRPG) {
                    soundPitches[sound] = 4f;
                }

                Debug.Log($"Loaded {clips.Count} sounds for {sound}");
            }
            else
            {
                Debug.LogError($"Failed to load sounds for label: {label}");
            }
        };
    }

    // Play a random sound for a given SoundType
    public static void PlaySound(Sound sound)
    {
        float delay = soundDelay;
        if (sound == Sound.FireBurn)
        {
            delay += 0.5f;
        }
        if (Time.time - lastPlayedTimes[sound] < delay)
        {
            return;
        }
        lastPlayedTimes[sound] = Time.time + Random.Range(-soundDelayVarience, soundDelayVarience);
        if (soundCache.TryGetValue(sound, out List<AudioClip> clips) && clips.Count > 0)
        {
            int randomIndex = Random.Range(0, clips.Count);
            GameObject obj = new GameObject("Sound: " + sound.ToString());
            AudioSource source = obj.AddComponent<AudioSource>();
            AudioClip clip = clips[randomIndex];
            source.clip = clip;
            source.pitch = soundPitches[sound];
            source.Play();
            GameObject.Destroy(obj, clip.length);
        }
        else
        {
            Debug.LogWarning($"No sounds loaded for {sound}");
        }
    }
}
// 31
public enum Sound
{
    None = 0,
    ShootPistol = 11,
    ShootSub = 1,
    ShootShotgun = 8,
    ShootFlamethrower = 25,
    ShootSniper = 28,
    ShootAR = 29,
    ShootLaser = 30,
    ShootRPG = 32,
    PickupGun = 2,
    PickupGrenade = 3,
    GrenadePull = 4,
    GrenadeThrow = 5,
    Explosion = 6,
    Punch = 7,
    GunClick = 10,
    PlayerBulletHit = 9,
    PlayerPunchHit = 18,
    WoodBulletHit = 13,
    WoodPunchHit = 16,
    StoneBulletHit = 14,
    StonePunchHit = 17,
    MetalBulletHit = 12,
    MetalPunchHit = 15,
    C4Remote = 19,
    PickupC4 = 20,
    FootStepGrass = 21,
    FootStepMetal = 22,
    FootStepWood = 23,
    FootStepStone = 24,
    FireBurn = 26,
    GasStart = 27,
    OpenPresent = 31,
}
