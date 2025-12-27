using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMan : MonoBehaviour
{
    public void StartRound()
    {
        GameMan.Instance.StartGame();
    }
}
