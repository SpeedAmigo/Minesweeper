using TMPro;
using UnityEngine;

public class PasswordWindowScript : MonoBehaviour
{
    private const string Password = "Kamikadze";
    
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject streakSetWindow;

    public void CheckPassword()
    {
        if (inputField.text == Password)
        {
            gameObject.SetActive(false);
            streakSetWindow.SetActive(true);
        }

        inputField.text = "";
    }
    
}
