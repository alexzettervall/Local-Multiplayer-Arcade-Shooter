using UnityEngine;

public static class ParticleManager
{
    public static void EmitParticles(ParticleType particleType, int amount, Vector2 position)
    {
        GameAssets.ParticleData particleData = GameAssets.i.GetParticleData(particleType);
        GameObject particleObj = GameObject.Instantiate(particleData.prefab, position, Quaternion.identity);
        ParticleSystem particleSystem = particleObj.GetComponent<ParticleSystem>();
        ParticleSystem.EmissionModule emission = particleSystem.emission;

        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, amount)
        });

        GameObject.Destroy(particleObj, 10f);
    }
}

public enum ParticleType
{
    Blood,
    Feather
}