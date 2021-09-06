using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using TMPro;
using UnityEngine.UI;
using FullSerializer;

public class AccountManager : MonoBehaviour
{
    public static AccountManager instance;
    public static AccountManager Instance { get { return instance; } }

    public static fsSerializer serializer = new fsSerializer();

    [Header("Firebase Keys")]
    public string uri = "https://dialogue-rpg-default-rtdb.europe-west1.firebasedatabase.app";
    public string authKey = "AIzaSyDCYqnjRPNwMkgKckBsfFOSIMCiXfnKG34";

    [Header("User Data")]
    public string idToken;
    public string localId;
    public User user = new User();

    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;
    public Button loginButton;
    public Button registerButton;

    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

        if (PlayerPrefs.HasKey("UserEmail") && PlayerPrefs.HasKey("UserPassword"))
        {
            Debug.Log("auto login");
            emailLoginField.text = PlayerPrefs.GetString("UserEmail");
            passwordLoginField.text = PlayerPrefs.GetString("UserPassword");
            SignIn(PlayerPrefs.GetString("UserEmail"), PlayerPrefs.GetString("UserPassword"));


        }
        else
        {
            Debug.Log("No Login Data Saved");
        }
    }
    void SignUp(string email, string username, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + authKey, userData)
        .Then(response =>
        {
            localId = response.localId;
            idToken = response.idToken;

            user.localId = localId;

            Post();
            PlayerData.Instance.user = user;
            UIManager.instance.LoadScreen(2);

            PlayerPrefs.SetString("UserEmail", email);
            PlayerPrefs.SetString("UserPassword", password);
        })
        .Catch(error =>
        {
            Debug.Log("Could not register user: " + error);
        });
    }

    void SignIn(string email, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + authKey, userData)
        .Then(response =>
        {
            localId = response.localId;
            idToken = response.idToken;

            RestClient.Get<User>(uri + "/users/" + localId + "/.json")
            .Then(response2 =>
            {
                PlayerData.Instance.user = response2;

                Debug.Log("Logging in: " + response2.username);

                Load();

                UIManager.Instance.LoadScreen(2);

                PlayerPrefs.SetString("UserEmail", email);
                PlayerPrefs.SetString("UserPassword", password);
            })
            .Catch(error2 =>
            {
                Debug.Log("Problem getting player data: " + error2);
            });

        })
        .Catch(error =>
        {
            Debug.Log("Could not login user: " + error);
        });
    }

    public void Load()
    {

        RestClient.Get(AccountManager.Instance.uri + "/users/" + PlayerData.Instance.user.localId + "/campaigns.json?auth=" + AccountManager.Instance.idToken)
        .Then(response =>
        {

            fsData campaignData = fsJsonParser.Parse(response.Text);
            Dictionary<string, string> campaignKeys = null;
            serializer.TryDeserialize(campaignData, ref campaignKeys);

            foreach (var c in campaignKeys.Values)
            {
                PlayerData.Instance.user.campaigns.Add(c);
            }

            GameObject.Find("Campaigns").GetComponent<Campaigns>().Refresh();
        });
    }

    void Post()
    {
        RestClient.Put(uri + "users/" + localId + ".json?auth=" + idToken, user);
    }

    public void RegisterButton()
    {
        if (usernameRegisterField.text != null && emailRegisterField.text != null && passwordRegisterField.text != null && passwordRegisterVerifyField.text != null)
        {
            if (passwordRegisterField.text == passwordRegisterVerifyField.text)
            {
                user.username = usernameRegisterField.text;
                user.email = emailRegisterField.text;

                SignUp(emailRegisterField.text, usernameRegisterField.text, passwordRegisterField.text);
            }
            else
            {
                StartCoroutine(Warning(warningRegisterText, "Passwords do not match"));
            }
        }
        else
        {
            StartCoroutine(Warning(warningRegisterText, "Please fill in all fields"));
        }
    }

    public void LoginButton()
    {
        SignIn(emailLoginField.text, passwordLoginField.text);
    }

    public void SignOutButton()
    {
        PlayerData.Instance.user = null;
        UIManager.instance.LoadScreen(0);

        emailLoginField.text = "";
        passwordLoginField.text = "";

        emailRegisterField.text = "";
        usernameRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";

        PlayerPrefs.DeleteKey("UserEmail");
        PlayerPrefs.DeleteKey("UserPassword");
    }

    IEnumerator Warning(TMP_Text obj, string message)
    {
        obj.text = message;
        obj.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        obj.gameObject.SetActive(false);
    }

}
