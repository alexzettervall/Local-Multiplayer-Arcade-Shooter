using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI
{
    private Player player;
    public PlayerUI(Player player)
    {
        this.player = player;
        UpdateUI();
    }

    public void UpdateUI()
    {
        UpdateHealthCircle();
        UpdateAmmoUI();
    }
    private void UpdateHealthCircle()
    {
        Image healthCircle = player.GetHealthCircle();
        Color color = player.GetColor();
        healthCircle.color = new Color(color.r, color.g, color.b, healthCircle.color.a);
        healthCircle.fillAmount = (float)player.GetHealth() / (float)player.GetMaxHealth();
    }
    private void UpdateAmmoUI()
    {
        RadialAmmoDisplay ammoDisplay = player.GetAmmoDisplay();
        Item item = player.GetItem();
        if (!(item is Gun))
        {
            ammoDisplay.gameObject.SetActive(false);
            return;
        }
        ammoDisplay.gameObject.SetActive(true);
        Gun gun = (Gun)item;
        ammoDisplay.UpdateDisplay(gun.GetAmmo(), gun.GetMaxAmmo());
    }
}
