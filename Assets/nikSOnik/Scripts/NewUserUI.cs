using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewUserUI : MonoBehaviour
{
    [SerializeField] TMP_InputField username;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button button;
    [SerializeField] int minLenght = 3;
    [SerializeField] string minLenghtMessage;
    [SerializeField] string usernameExistsMessage;
    public void AddUser()
    {
        MainManager.Instance.AddUser(username.text);
        username.text = string.Empty;
        button.interactable = false;
    }
    public void ValidateUserName(string name)
    {
        if (name.Length < minLenght)
        {
            Erorr(minLenghtMessage + minLenght);
        }
        else if (MainManager.Instance.UserNameExists(name))
        {
            Erorr(usernameExistsMessage);
        }
        else
        {
            button.interactable = true;
            messageText.gameObject.SetActive(false);
        }
    }

    void Erorr(string message)
    {
        button.interactable = false;
        messageText.text = message;
        messageText.gameObject.SetActive(true);
    }
}