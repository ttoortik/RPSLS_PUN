using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using ExitGames.Client.Photon;

public class GameConnect : PunBehaviour
{
    public InputField InputField;
    public string UserId;

	string previousRoomPlayerPrefKey = "PUN:Demo:RPS:PreviousRoom";
	public string previousRoom;

    private const string MainSceneName = "DemoRPS-Scene";

	const string NickNamePlayerPrefsKey = "NickName";


	void Start()
	{
		InputField.text = PlayerPrefs.HasKey(NickNamePlayerPrefsKey)?PlayerPrefs.GetString(NickNamePlayerPrefsKey):"";
	}

    public void ApplyUserIdAndConnect()
    {
		string nickName = "DemoNick";
        if (this.InputField != null && !string.IsNullOrEmpty(this.InputField.text))
        {
            nickName = this.InputField.text;
			PlayerPrefs.SetString(NickNamePlayerPrefsKey,nickName);
        }
        
    
        if (PhotonNetwork.AuthValues == null)
        {
            PhotonNetwork.AuthValues = new AuthenticationValues();
        }
        


		PhotonNetwork.AuthValues.UserId = nickName;

		Debug.Log("Nickname: " + nickName + " userID: " + this.UserId,this);
		


        PhotonNetwork.playerName = nickName;
        PhotonNetwork.ConnectUsingSettings("0.5");
        
    
        PhotonHandler.StopFallbackSendAckThread();
    }


    public override void OnConnectedToMaster()
    {
        this.UserId = PhotonNetwork.player.UserId;

		if (PlayerPrefs.HasKey(previousRoomPlayerPrefKey))
		{
			Debug.Log("getting previous room from prefs: ");
			this.previousRoom = PlayerPrefs.GetString(previousRoomPlayerPrefKey);
			PlayerPrefs.DeleteKey(previousRoomPlayerPrefKey); 
		}


        if (!string.IsNullOrEmpty(this.previousRoom))
        {
            Debug.Log("ReJoining previous room: " + this.previousRoom);
            PhotonNetwork.ReJoinRoom(this.previousRoom);
            this.previousRoom = null;
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinedLobby()
    {
        OnConnectedToMaster(); 
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
		Debug.Log("OnPhotonRandomJoinFailed");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2, PlayerTtl = 20000 }, null);
    }

    public override void OnJoinedRoom()
    {
		Debug.Log("Joined room: " + PhotonNetwork.room.Name);
        this.previousRoom = PhotonNetwork.room.Name;
		PlayerPrefs.SetString(previousRoomPlayerPrefKey,this.previousRoom);

    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
		Debug.Log("OnPhotonJoinRoomFailed");
        this.previousRoom = null;
		PlayerPrefs.DeleteKey(previousRoomPlayerPrefKey);
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
        Debug.Log("Disconnected due to: " + cause + ". this.previousRoom: " + this.previousRoom);
    }
	
	public override void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer)
	{
		Debug.Log("OnPhotonPlayerActivityChanged() for "+otherPlayer.NickName+" IsInactive: "+otherPlayer.IsInactive);
	}

}
