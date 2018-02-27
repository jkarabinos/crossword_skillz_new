using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Facebook.Unity;

public class FacebookLogic : MonoBehaviour {

    private void Awake()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            //Handle FB.Init
            FB.Init(() => {
                FB.ActivateApp();
            });
        }
    }

    public void InviteFriendsButton()
    {

        //invite friends via facebook
        
        FB.Mobile.AppInvite(new Uri("https://fb.me/1588167587927021"), callback: AppInviteCallback);

    }

    public void AppInviteCallback(IResult result)
    {
        if (result.Cancelled)
        {
            Debug.Log("invite cancelled");
        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Error on invite!");
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            Debug.Log("succes on share");
        }

        //Debug.Log ("do something after the app invite");
    }
}
