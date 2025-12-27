using UnityEngine;
using UnityEngine.UI;

public class RadialAmmoDisplay : MonoBehaviour
{
    [SerializeField] private Sprite icon;
    [SerializeField] private int bullets;
    [SerializeField] private int maxBullets = 6;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float range = 180f;
    private GameObject[] bulletIcons = new GameObject[0];

    private void Start()
    {
        InitializeDisplay();
    }
    private void InitializeDisplay()
    { 
        foreach (var icon in bulletIcons)
        {
            if (icon != null)
            {
                Destroy(icon);
            }
        }
        bulletIcons = new GameObject[maxBullets];
        for (int i = 0; i < maxBullets; i++)
        {
            float angle = bulletIcons.Length > 1 ? Mathf.Lerp(-range / 2f + 90f, range / 2f + 90f, (float)i /(bulletIcons.Length - 1)) : 90;
            float radian = angle * Mathf.Deg2Rad;

            Vector3 position = transform.position + new Vector3(
                Mathf.Cos(radian) * radius,
                Mathf.Sin(radian) * radius,
                0f
            );

            Vector2 dir = position - transform.position;
            float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

            bulletIcons[i] = new GameObject("Icon");
            SpriteRenderer sr = bulletIcons[i].AddComponent<SpriteRenderer>();
            sr.sprite = icon;
            bulletIcons[i].transform.parent = transform;
            bulletIcons[i].transform.position = position;
            bulletIcons[i].transform.eulerAngles = new Vector3(0, 0, rot);
        }
    }
    public void UpdateDisplay(int currentBullets, int maxBullets)
    {
        if (bulletIcons.Length != maxBullets)
        {
            this.maxBullets = maxBullets;
            InitializeDisplay();
        }
        for (int i = 0; i < maxBullets; i++)
        {
            bulletIcons[i].SetActive(i >= bulletIcons.Length - currentBullets);
        }
    }
}
