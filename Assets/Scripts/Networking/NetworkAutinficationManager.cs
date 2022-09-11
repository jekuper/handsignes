using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkAutinficationManager : NetworkAuthenticator
{
    struct AuthenticationRequest : NetworkMessage {
        public string nickname;
    }
    struct AuthenticationResponce : NetworkMessage {
        public int Code;
        public string Message;
        public ProfileData newData;
    }

    #region Client
    public override void OnStartClient () {
        NetworkClient.RegisterHandler<AuthenticationResponce> (ResponceChecker, false);
        base.OnStartClient ();
    }
    public override void OnStopClient () {
        NetworkClient.UnregisterHandler<AuthenticationResponce> ();
        base.OnStopClient ();
    }

    public override void OnClientAuthenticate () {
        AuthenticationRequest request = new AuthenticationRequest {
            nickname = NetworkDataBase.LocalUserData.nickname
        };

        NetworkClient.connection.Send (request);

        base.OnClientAuthenticate ();
    }

    private void ResponceChecker (AuthenticationResponce responce) {
        if (responce.Code == 0) {
            NetworkDataBase.LocalUserData = responce.newData;
            ClientAccept ();
        } else if (responce.Code == 1) {
//            Debug.LogError (responce.Message);
            NetworkManager.singleton.StopClient (); 
            if (LobbyGUI.singleton != null) {
                LobbyGUI.singleton.StopLoading (responce.Message);
            }
        }
    }
    #endregion

    #region Server
    public override void OnStartServer () {
        NetworkServer.RegisterHandler<AuthenticationRequest> (RequestChecker, false);
        base.OnStartServer ();
    }
    public override void OnStopServer () {
        NetworkServer.UnregisterHandler<AuthenticationRequest> ();
        base.OnStartServer ();
    }
    public override void OnServerAuthenticate (NetworkConnectionToClient conn) {
        base.OnServerAuthenticate (conn);
    }


    private void RequestChecker (NetworkConnectionToClient conn, AuthenticationRequest request) {
        Debug.Log ("authenfication request");
        if (LobbyGUI.singleton == null) {
            AuthenticationResponce responce = new AuthenticationResponce {
                Code = 1,
                Message = "game have already started",
            };
            conn.Send (responce);

            conn.isAuthenticated = false;

            StartCoroutine (DelayedDisconnection (conn, 1));
            return;
        }
        if (NetworkDataBase.NicknameExistChecker (request.nickname)) {
            AuthenticationResponce responce = new AuthenticationResponce {
                Code = 1,
                Message = "nickname has already been taken",
            };
            conn.Send (responce);

            conn.isAuthenticated = false;

            StartCoroutine (DelayedDisconnection (conn, 1));
        } else {
            ProfileData newData = new ProfileData ();
            newData.nickname = request.nickname;
            NetworkDataBase.lastTeamIndex++;
            newData.teamIndex = NetworkDataBase.lastTeamIndex;

            NetworkDataBase.data.Add (conn, newData);

            AuthenticationResponce responce = new AuthenticationResponce {
                Code = 0,
                Message = "OK",
                newData = newData,
            };
            conn.Send (responce);
            ServerAccept (conn);
        }
    }
    #endregion

    public IEnumerator DelayedDisconnection (NetworkConnectionToClient conn, float time) {
        yield return new WaitForSeconds (time);

        ServerReject (conn);
    }
}
