
namespace Define
{
    public enum AccountCreationStatus
    {
        Success,        //회원가입 성공
        DuplicateId,   //아이디 중복
        DuplicateNickname,   //닉네임 중복
    }

    public enum LoginStatus
    {
        Success,
        IDNotFound,
        PasswordNotFound,
    }

    public enum Scenes
    {
        Title, 
        InGame,
    }
}