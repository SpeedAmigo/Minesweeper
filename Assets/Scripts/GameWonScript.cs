using UnityEngine;

public class GameWonScript : MonoBehaviour
{
    [SerializeField] private GameObject gameWonPanel;

    private void EnableGameWonPanel()
    {
        gameWonPanel.SetActive(true);
    }
    
    private void OnEnable()
    {
        MinesweeperManager.OnGameWonEvent += EnableGameWonPanel;
    }

    private void OnDisable()
    {
        MinesweeperManager.OnGameWonEvent -= EnableGameWonPanel;
    }
}
