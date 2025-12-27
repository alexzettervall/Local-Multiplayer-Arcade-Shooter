using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private bool randomizeRot;
    [SerializeField] private Vector2 maxOffset;
    [SerializeField] private Vector2 size = new Vector2(1f, 1f);

    private void Start()
    {
        if (randomizeRot)
        {
            transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
        }
        if (sprites.Length != 0)
        {
            sr.sprite = sprites[Random.Range(0, sprites.Length)];
        }


        RandomizePosition();
        
        float sizeM = Random.Range(size.x, size.y);
        transform.localScale = new Vector3(transform.localScale.x * sizeM, transform.localScale.y * sizeM, 1f);
    }

    private void RandomizePosition() {
        float rad = Random.Range(0, 6.283f);
        float dist = Random.Range(0f, maxOffset.x);
        transform.position = (Vector2)transform.position + dist * new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    public void SetMaxOffSet(Vector2 maxOffset) {
        this.maxOffset = maxOffset;
    }

}
