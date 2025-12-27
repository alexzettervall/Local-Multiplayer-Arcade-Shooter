using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Material", fileName = "New Material")]
public class Material : ScriptableObject
{
    public int priority;
    public Sound bulletHit;
    public Sound punchHit;
    public Sound explosionHit;
    public Sound fireHit;
    public Sound gasHit;
    public Sound footstep;
}
