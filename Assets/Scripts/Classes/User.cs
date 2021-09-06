using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User
{
    public string username;
    public string email;
    public string localId;
    public List<string> campaigns;
    public List<string> invites;

    public User()
    {

    }

    public User(string _username, string _email, string _localId)
    {
        username = _username;
        email = _email;
        localId = _localId;
    }
}
