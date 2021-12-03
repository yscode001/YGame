using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField]
    private InputField UsernameIpt;

    [SerializeField]
    private InputField PasswordIpt;

    [SerializeField]
    private Button LoginBtn;

    public void Start()
    {
        LoginBtn.onClick.AddListener(OnLoginBtnClick);
    }

    private void OnLoginBtnClick()
    {
        string username = UsernameIpt.text;
        string password = PasswordIpt.text;
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            WndRoot.Instance.ShowTips("请输入用户名或密码");
            return;
        }
    }
}