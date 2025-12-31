using UnityEngine;
using UnityEngine.UI;

public class SmileyChangeScript : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite sprite;

    private void ChangeSprite(bool value)
    {
        if (!value)
        {
            image.sprite = sprite;
        }
    }
    
    private void OnEnable()
    {
        MinesweeperManager.OnTileUncoverEvent += ChangeSprite;
    }

    private void OnDisable()
    {
        MinesweeperManager.OnTileUncoverEvent -= ChangeSprite;
    }
}
