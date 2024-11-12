using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private DatabaseAccess dataBaseAccess;

    [SerializeField] private Text idText;
    [SerializeField] private Text passwordText;

    [SerializeField] private Text createIdText;
    [SerializeField] private Text createPasswordText;
    [SerializeField] private Text NicknameText;

    public void CreateID()
    {
        dataBaseAccess.SaveUserInformToDataBase(createIdText.text, createPasswordText.text, NicknameText.text);
    }

    public void TryLogin()
    {
        if(dataBaseAccess.CheckUserInformsFromDataBase(idText.text, passwordText.text))
        {
            Debug.Log("회원정보를 찾았습니다.");
        }
        else
        {
            Debug.Log("회원정보를 찾지 못했습니다.");
        }
    }
}
