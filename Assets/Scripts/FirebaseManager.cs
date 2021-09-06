using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.UI;
using TMPro;
using Proyecto26;
using UnityEngine.Networking;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;
    public static FirebaseManager Instance { get { return instance; } }

    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference db;

    public string root = "https://dialogue-rpg-default-rtdb.europe-west1.firebasedatabase.app/";

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

    [Header("UserData")]
    public TMP_Text usernameField;

    //Script references
    public PlayerData playerData;
    public Campaigns campaigns;
    public Invites invites;
    public ActiveCampaign activeCampaign;

    void Awake()
    {
        instance = this;


    }

    void Start()
    {

        // if (PlayerPrefs.HasKey("UserEmail") && PlayerPrefs.HasKey("UserPassword"))
        // {
        //     Debug.Log("auto login");
        //     StartCoroutine(AutoLogin());


        // }
        // else
        // {
        //     Debug.Log("No Login Data Saved");
        // }
    }

    public void InitializeFirebase()
    {
        Debug.Log("Setting up firebase auth");

        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseDatabase.DefaultInstance.RootReference;

        FirebaseDatabase.DefaultInstance
        .GetReference("users")
        .ValueChanged += HandleValueChanged;
    }

    //Data change detection
    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        Debug.Log("data had changed");

        // DataSnapshot snapshot = args.Snapshot;

        if (PlayerPrefs.HasKey("UserEmail"))
        {
            try
            {
                //Load();
            }
            catch
            {
                Debug.Log("Don't like that");
            }
        }
    }

    //Login Stuff

    public void ClearLoginFields()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    public void ClearRegisterFields()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    public void SignOutButton()
    {
        auth.SignOut();
        UIManager.instance.LoadScreen(0);
        ClearLoginFields();
        ClearRegisterFields();

        loginButton.gameObject.SetActive(true);
        registerButton.gameObject.SetActive(true);
        warningLoginText.gameObject.SetActive(false);

        PlayerPrefs.DeleteKey("UserEmail");
        PlayerPrefs.DeleteKey("UserPassword");
    }

    //Invite Stuff
    public void Invite(string email)
    {
        StartCoroutine(InvitePlayer(email));
    }

    IEnumerator InvitePlayer(string email)
    {
        var DBTask = db.Child("users").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            Debug.Log("This user has no data");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            bool playerFound = false;
            bool messageSent = false;

            foreach (var child in snapshot.Children)
            {
                //Debug.Log(child.Child("email").Value.ToString());

                if (child.Child("email").Value.ToString() == email) //finds user matching the email entered
                {
                    playerFound = true;
                    Debug.Log("Email found");

                    if (child.Child("invites").ChildrenCount < 4) //checks if found user has less than 4 invites already
                    {
                        string inviteKey = db.Child("users").Child(child.Key).Child("invites").Push().Key;

                        db.Child("users").Child(child.Key).Child("invites").Child(inviteKey).Child("key").SetValueAsync(playerData.key);
                        db.Child("users").Child(child.Key).Child("invites").Child(inviteKey).Child("title").SetValueAsync(playerData.campaignName);
                        db.Child("users").Child(child.Key).Child("invites").Child(inviteKey).Child("gamemaster").SetValueAsync(User.DisplayName);

                        activeCampaign.InviteInfo(0);
                        messageSent = true;
                    }
                    else
                    {
                        activeCampaign.InviteInfo(2);
                        messageSent = true;
                    }
                }
            }

            if (!playerFound && !messageSent)
            {
                activeCampaign.InviteInfo(1);
            }
            else if (playerFound && !messageSent)
            {
                activeCampaign.InviteInfo(6);
            }
        }
    }

    IEnumerator GetInviteKeys()
    {
        var DBTask = db.Child("users").Child(User.UserId).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            Debug.Log("This user has no data");
        }
        else
        {
            //data retrieved
            DataSnapshot snapshot = DBTask.Result;

            Debug.Log("User Invites:" + snapshot.Child("invites").ChildrenCount.ToString());

            playerData.inviteEntryKeys.Clear();
            playerData.inviteKeys.Clear(); //Clearing invites from the array so that they can be readded in order when refreshing
            playerData.inviteTitles.Clear();
            playerData.inviteGms.Clear();

            foreach (var child in snapshot.Child("invites").Children)
            {
                string iKey = child.Key.ToString();
                playerData.inviteEntryKeys.Add(iKey);

                foreach (var iChild in snapshot.Child("invites").Child(iKey).Children)
                {
                    switch (iChild.Key)
                    {
                        case "key":
                            playerData.inviteKeys.Add(iChild.Value.ToString());
                            break;
                        case "title":
                            playerData.inviteTitles.Add(iChild.Value.ToString());
                            break;
                        case "gamemaster":
                            playerData.inviteGms.Add(iChild.Value.ToString());
                            break;
                        default:
                            Debug.Log("No save location for " + iChild.Key);
                            break;
                    }
                }

                Debug.Log("All invite data loaded");
            }

            yield return new WaitForSeconds(1f);

            invites.Refresh();
        }
    }

    public void Accept(string key, string eKey)
    {
        StartCoroutine(AcceptInvite(key, eKey));
    }

    IEnumerator AcceptInvite(string key, string eKey)
    {
        string userKey = db.Child("users").Child(User.UserId).Child("campaigns").Push().Key;
        var DBTask = db.Child("users").Child(User.UserId).Child("campaigns").Child(userKey).SetValueAsync(key);

        string pKey = db.Child("campaigns").Child(key).Child("players").Push().Key;
        db.Child("campaigns").Child(key).Child("players").Child(User.UserId).SetValueAsync(User.DisplayName);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log("Joined Campaign");
            StartCoroutine(DeleteInvite(eKey));
        }
    }

    public void Decline(string eKey)
    {
        StartCoroutine(DeleteInvite(eKey));
    }

    IEnumerator DeleteInvite(string key)
    {
        var DBTask = db.Child("users").Child(User.UserId).Child("invites").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            Debug.Log("This user has no data");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            foreach (var child in snapshot.Children)
            {
                if (child.Key.ToString() == key)
                {
                    Debug.Log("Deleted invite");
                    db.Child("users").Child(User.UserId).Child("invites").Child(key).RemoveValueAsync();

                    playerData.inviteEntryKeys.Remove(key);
                    playerData.inviteKeys.RemoveAt(playerData.inviteEntryKeys.IndexOf(key));
                    playerData.inviteTitles.RemoveAt(playerData.inviteEntryKeys.IndexOf(key));
                    playerData.inviteGms.RemoveAt(playerData.inviteEntryKeys.IndexOf(key));
                }
            }

            invites.Refresh();
        }
    }

    //Character Stuff
    public void LoadCharacters(string key)
    {
        GetCharacterKeys(key);
    }

    IEnumerator GetCharacterKeys(string key)
    {
        Debug.Log("Load Characters is being called");
        var DBTask = db.Child("campaigns").Child(key).Child("characters").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            Debug.Log("NO Characters could be found");
        }
        else
        {
            //data retrieved
            DataSnapshot snapshot = DBTask.Result;

            foreach (var child in snapshot.Children)
            {
                //activeCampaign.playerKeys.Add(child.Value.ToString());

                Character c = new Character();

                c.id = child.Value.ToString();
                c.campaign = key;

                foreach (var cChild in child.Children)
                {
                    switch (cChild.Key)
                    {
                        case "name":

                            break;
                        case "word":

                            break;
                        case "wit":

                            break;
                        case "will":

                            break;
                        case "want":

                            break;
                        case "weapon":

                            break;
                        case "clothing":

                            break;
                        case "relic":

                            break;
                        case "background":

                            break;
                        case "features":

                            break;
                        default:
                            Debug.Log("No save point for " + cChild.Key.ToString());
                            break;
                    }
                }

                playerData.characters.Add(c);


            }

            Debug.Log("All Characters Loaded");

        }
    }
}
