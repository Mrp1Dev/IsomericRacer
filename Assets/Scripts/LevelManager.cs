using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject lobbyLevelInstance;
    [SerializeField] private GameObject[] gameLevelPrefabs;

    private GameObject currentGameLevel;
    private bool isLobby = true;
    private void Awake()
    {
        lobbyLevelInstance.SetActive(true);
        isLobby = true;
    }

    public GameObject LoadNewGameplayLevel()
    {
        lobbyLevelInstance.SetActive(false);
        isLobby = false;
        if (currentGameLevel != null) { Destroy(currentGameLevel); }
        currentGameLevel = Instantiate(gameLevelPrefabs[Random.Range(0, gameLevelPrefabs.Length)]);
        return currentGameLevel;
    }

}