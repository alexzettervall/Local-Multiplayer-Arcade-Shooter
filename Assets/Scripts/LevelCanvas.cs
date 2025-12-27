using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCanvas : MonoBehaviour
{
    [SerializeField] private GameObject readyText;
    [SerializeField] private GameObject goText;
    public GameObject GetReadyText()
    {
        return readyText;
    }
    public GameObject GetGoText()
    {
        return goText;
    }
}
