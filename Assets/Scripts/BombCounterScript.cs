using TMPro;
using UnityEngine;

public class BombCounterScript : MonoBehaviour
{
    private TMP_Text bombCounter;

    private void Awake()
    {
        bombCounter = GetComponent<TMP_Text>();
    }
    
    private void SetBombsCount(int count)
    {
        bombCounter.text = count.ToString();
    }

    private void OnEnable()
    {
        MinesweeperManager.OnBombCountEvent += SetBombsCount;
    }

    private void OnDisable()
    {
        MinesweeperManager.OnBombCountEvent -= SetBombsCount;
    }
}
