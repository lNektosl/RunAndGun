using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    private Player player;

    [SerializeField] private float timeBeforeExit;
    public void Awake () {
        player = FindFirstObjectByType<Player>();
    }

    public void OnEnable () {
        player.OnPlayerDie += OnPlayerDied;
    }

    public void OnPlayerDied () {
        Invoke("EndGame",timeBeforeExit);
    }

    public void EndGame() {
        SceneManager.LoadScene("Main Menu");
    }
}
