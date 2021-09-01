using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.UI;
using TMPro;

public class FirebaseManager : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference db;

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
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.Log("Could not resolve Firebase Dependencies: " + dependencyStatus);
            }
        });

        //playerData = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();

    }

    void Start()
    {
        if (PlayerPrefs.HasKey("UserEmail") && PlayerPrefs.HasKey("UserPassword"))
        {
            Debug.Log("auto login");
            StartCoroutine(AutoLogin());
        }
        else
        {
            Debug.Log("No Login Data Saved");
        }
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
                Load();
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

    //Functions allowing access via button

    public void LoginButton()
    {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
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

    public void SaveDataButton()
    {
        StartCoroutine(UpdateUsernameAuth(usernameField.text));
        StartCoroutine(UpdateUsernameDatabase(usernameField.text));
    }

    IEnumerator AutoLogin()
    {
        emailLoginField.text = PlayerPrefs.GetString("UserEmail");
        passwordLoginField.text = PlayerPrefs.GetString("UserPassword");

        loginButton.gameObject.SetActive(false);
        registerButton.gameObject.SetActive(false);
        warningLoginText.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }


    IEnumerator Login(string _email, string _password)
    {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //wait until the above is done
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //handle ay errors
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            //display appropriate error message
            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Incorrect Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Email is Invalid";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }

            warningLoginText.text = message;
        }
        else
        {
            //using is now logged in
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged in";
            Load();

            yield return new WaitForSeconds(1.5f);

            usernameField.text = User.DisplayName + "'s Campaigns";
            UIManager.instance.LoadScreen(2);
            confirmLoginText.text = "";
            ClearLoginFields();
            ClearRegisterFields();

            PlayerPrefs.SetString("UserEmail", _email);
            PlayerPrefs.SetString("UserPassword", _password);

            playerData.email = _email;
            playerData.username = User.DisplayName;
            playerData.UID = User.UserId;


        }
    }


    IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Passwords do not match";
        }
        else
        {
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //wait until above is done
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Password too weak";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email already in use";
                        break;
                }

                warningRegisterText.text = message;
            }
            else
            {
                //new user created
                User = RegisterTask.Result;

                if (User != null)
                {
                    //create profile and add display name
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //wait until above is done
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //handle errors
                        Debug.LogWarning(message: $"Failed to Register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        db.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);
                        db.Child("users").Child(User.UserId).Child("email").SetValueAsync(_email);
                        //username is set
                        UIManager.instance.LoadScreen(2);
                        warningRegisterText.text = "";
                        ClearLoginFields();
                        ClearRegisterFields();

                        PlayerPrefs.SetString("UserEmail", _email);
                        PlayerPrefs.SetString("UserPassword", _password);
                    }
                }
            }
        }
    }

    IEnumerator UpdateUsernameAuth(string _username)
    {
        UserProfile profile = new UserProfile { DisplayName = _username };

        var ProfileTask = User.UpdateUserProfileAsync(profile);
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //username updated
        }
    }

    IEnumerator UpdateUsernameDatabase(string _username)
    {
        var DBTask = db.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //username updated in database
        }
    }


    public void Load()
    {
        StartCoroutine(GetCampaignKeys());
        StartCoroutine(GetInviteKeys());
    }

    //Campaign stuff

    IEnumerator GetCampaignKeys()
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

            Debug.Log("User campaigns:" + snapshot.Child("campaigns").ChildrenCount.ToString());

            playerData.campaigns.Clear();

            foreach (var child in snapshot.Child("campaigns").Children)
            {
                StartCoroutine(GetCampaignData(child.Value.ToString()));
                StartCoroutine(GetCharacterKeys(child.Value.ToString()));
            }

            yield return new WaitForSeconds(1f);

            campaigns.Refresh();
        }
    }

    IEnumerator GetCampaignData(string key)
    {
        var DBTask = db.Child("campaigns").Child(key).GetValueAsync();
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

            Campaign campaign = new Campaign();
            campaign.key = key;

            List<string> playerIDs = new List<string>();
            List<string> playerNames = new List<string>();

            foreach (var child in snapshot.Children)
            {
                switch (child.Key)
                {
                    case "title":
                        campaign.name = child.Value.ToString();
                        break;
                    case "genre":
                        campaign.genre = child.Value.ToString();
                        break;
                    case "gamemaster":
                        campaign.gameMaster = child.Value.ToString();
                        break;
                    case "gmid":
                        campaign.gmID = child.Value.ToString();
                        break;
                    case "players":
                        foreach (var pChild in child.Children)
                        {
                            playerIDs.Add(pChild.Key.ToString());
                            playerNames.Add(pChild.Value.ToString());
                        }
                        break;
                    default:
                        Debug.Log("No save location for " + child.Key);
                        break;
                }
            }

            campaign.playerIDs = playerIDs;
            campaign.playerNames = playerNames;

            playerData.campaigns.Add(campaign);

        }
    }

    public void CreateCampaign(string name, string genre)
    {
        StartCoroutine(NewCampaign(name, genre, User.DisplayName));
    }

    IEnumerator NewCampaign(string name, string genre, string gameMaster)
    {
        //generate new key in this location as a unique id for this campaign
        string key = db.Child("campaigns").Push().Key;
        string userKey = db.Child("users").Child(User.UserId).Child("campaigns").Push().Key;

        var DBTask = db.Child("users").Child(User.UserId).Child("campaigns").Child(userKey).SetValueAsync(key);

        db.Child("campaigns").Child(key).Child("title").SetValueAsync(name);
        db.Child("campaigns").Child(key).Child("genre").SetValueAsync(genre);
        db.Child("campaigns").Child(key).Child("gamemaster").SetValueAsync(gameMaster);
        db.Child("campaigns").Child(key).Child("gmid").SetValueAsync(User.UserId);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log("New Campaign created");
            campaigns.Refresh();
        }
    }

    public void DeleteCampaign(string key)
    {
        StartCoroutine(DeleteCampaignIenum(key));
    }

    IEnumerator DeleteCampaignIenum(string key)
    {
        var DBTask = db.Child("campaigns").Child(key).RemoveValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            foreach (Campaign c in playerData.campaigns)
            {
                if (c.key == key)
                {
                    Debug.Log("For loop is triggering properly: " + c.key);
                    StartCoroutine(DeleteCampaignEntry(c.key));
                    Load();

                    //campaigns.Refresh();
                }
            }
            Debug.Log("Campaign Deleted");
            //campaigns.Refresh();
        }
    }

    IEnumerator DeleteCampaignEntry(string cKey)
    {
        var DBTask = db.Child("users").Child(User.UserId).Child("campaigns").GetValueAsync();
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
                if (child.Value.ToString() == cKey)
                {
                    db.Child("users").Child(User.UserId).Child("campaigns").Child(child.Key).RemoveValueAsync();
                }
            }
        }
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
