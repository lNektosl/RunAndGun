using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarUI : MonoBehaviour {
    [SerializeField] UnityEngine.UI.Image healthbar;
    private Player player;

    public void Awake () {
        player = FindObjectOfType<Player>();
    }

    public void UpdateHealthBar () {
        float hpPercentage = player.GetHPAsPercentage();
        healthbar.fillAmount = hpPercentage;
    }

    public void Update () {
        UpdateHealthBar();
    }
}
