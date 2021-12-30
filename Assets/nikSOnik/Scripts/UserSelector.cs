using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserSelector : MonoBehaviour
{
    [SerializeField] TMP_Dropdown list;
    [SerializeField] Button button;
    private void Awake()
    {
        MainManager.Instance.OnUserAdded += UpdateList;
    }
    private void Start()
    {
        UpdateList();
    }

    void UpdateList()
    {
        List<User> users = MainManager.Instance.Users;
        list.options.Clear();
        for (int i = 0; i < users.Count; i++)
        {
            list.options.Add(new TMP_Dropdown.OptionData($"{users[i].name} : {users[i].highscore}"));
        }
        button.interactable = list.options.Count != 0;
        list.value = 0;
    }

    public void SelectUser()
    {
        MainManager.Instance.StartGame(list.value);
    }
}