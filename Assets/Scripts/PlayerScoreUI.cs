using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScoreUI : MonoBehaviour
{
    [SerializeField] private RectTransform player;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Image playerColor;
    private RectTransform rect;
    private float targetX;
    private float deltaX;
    private void Start()
    {
        targetX = player.localPosition.x;
    }
    private void Update()
    {
        float speed = deltaX / moveSpeed;
        float newX = player.anchoredPosition.x + speed * Time.deltaTime;
        if (newX > targetX)
        {
            newX = targetX;
        }
        player.anchoredPosition = new Vector2(newX, player.anchoredPosition.y);
    }
    public void MovePos(float newX)
    {
        deltaX = (newX - 25f) / 0.7f - targetX;
        targetX = (newX-25f)/0.7f; // This is bad I know
    }
    public void SetColor(Color color)
    {
        playerColor.color = new Color(color.r, color.g, color.b, playerColor.color.a);
    } 
}
