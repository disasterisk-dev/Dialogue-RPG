using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
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

    //Reset all fields when needed

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
    }

    public void SaveDataButton()
    {
        StartCoroutine(UpdateUsernameAuth(usernameField.text));
        StartCoroutine(UpdateUsernameDatabase(usernameField.text));
    }

    //Automatically recovers login data from player prefs and logs user in

    IEnumerator AutoLogin()
    {
        emailLoginField.text = PlayerPrefs.GetString("UserEmail");
        passwordLoginField.text = PlayerPrefs.GetString("UserPassword");

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    //login

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

    //registration

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
                        //username is set
                        UIManager.instance.LoadScreen(3);
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

    // loading data

    public void Load()
    {
        StartCoroutine(GetCampaignKeys());

    }

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

            playerData.campaignKeys.Clear(); //Clearing campaigns from the array so that they can be readded in order when refreshing
            playerData.campaignTitles.Clear();
            playerData.campaignGenres.Clear();
            playerData.campaignGms.Clear();

            foreach (var child in snapshot.Child("campaigns").Children)
            {
                playerData.campaignKeys.Add(child.Value.ToString());
                StartCoroutine(GetCampaignData(child.Value.ToString()));
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

            foreach (var child in snapshot.Children)
            {
                switch (child.Key)
                {
                    case "title":
                        playerData.campaignTitles.Add(child.Value.ToString());
                        break;
                    case "genre":
                        playerData.campaignGenres.Add(child.Value.ToString());
                        break;
                    case "gamemaster":
                        playerData.campaignGms.Add(child.Value.ToString());
                        break;
                    default:
                        Debug.Log("No save location for " + child.Key);
                        break;
                }

            }

            Debug.Log("All campaign data loaded");

        }
    }

    //Campaign stuff

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
        db.Child("campaigns").Child(key).Child("players").Child("GM").SetValueAsync(User.UserId);

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

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        Debug.Log("data had changed");

        // DataSnapshot snapshot = args.Snapshot;

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
