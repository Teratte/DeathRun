using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    [SerializeField] private DatabaseAccess dataBaseAccess;

    [SerializeField] private Text idText;
    [SerializeField] private Text passwordText;

    [SerializeField] private Text createIdText;
    [SerializeField] private Text createPasswordText;
    [SerializeField] private Text createNicknameText;

    [SerializeField] private GameObject TitlePanel;
    [SerializeField] private GameObject CreateAccountPanel;

    [SerializeField] private Text txtIDNotFoundWarning;
    [SerializeField] private Text txtPasswordNotFoundWarning;
    [SerializeField] private Text txtDuplicateIdWarning;
    [SerializeField] private Text txtDuplicateNicknameWarning;

    public void OnCreateAccountPanel()
    {
        CreateAccountPanel.SetActive(true);
    }

    public void OffCreateAccountPanel()
    {
        CreateAccountPanel.SetActive(false);
    }

    public async void CreateAccount()
    {
        Define.AccountCreationStatus AccountCreationStatus = await dataBaseAccess.SaveNewAccountToDataBase(createIdText.text, createPasswordText.text, createNicknameText.text);

        switch(AccountCreationStatus)
        {
            case Define.AccountCreationStatus.Success:
                CreateAccountPanel.SetActive(false);
                txtDuplicateIdWarning.text = string.Empty;
                txtDuplicateNicknameWarning.text = string.Empty;
                break;
            case Define.AccountCreationStatus.DuplicateId:
                txtDuplicateNicknameWarning.text = string.Empty;
                txtDuplicateIdWarning.text = "중복 아이디입니다.";
                break;
            case Define.AccountCreationStatus.DuplicateNickname:
                txtDuplicateIdWarning.text = string.Empty;
                txtDuplicateNicknameWarning.text = "중복 닉네임입니다.";
                break;
        }
    }

    public async void TryLogin()
    {
        var (loginStatus, nickname) = await dataBaseAccess.CheckUserInformsFromDataBase(idText.text, passwordText.text);

        switch (loginStatus)
        {
            case Define.LoginStatus.Success:
                Debug.Log("로그인 정보를 찾았습니다!");
                PhotonInit.Instance.SetPlayerName(nickname);
                txtIDNotFoundWarning.text = string.Empty;
                txtPasswordNotFoundWarning.text = string.Empty;
                TitlePanel.SetActive(false);
                break;
            case Define.LoginStatus.IDNotFound:
                txtPasswordNotFoundWarning.text = string.Empty;
                txtIDNotFoundWarning.text = "아이디를 찾지 못했습니다.";
                break;
            case Define.LoginStatus.PasswordNotFound:
                txtIDNotFoundWarning.text = string.Empty;
                txtPasswordNotFoundWarning.text = "비밀번호가 틀립니다.";
                break;
        }
    }
}
