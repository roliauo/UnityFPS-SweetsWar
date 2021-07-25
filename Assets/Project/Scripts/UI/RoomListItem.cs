using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class RoomListItem : MonoBehaviour
    {
        public Text text_RoomName;
        public Text text_GameMode;
        public Image Image_GameMode;
        public Text text_Players;
        public Button btn_JoinRoom;

        private string roomName;

        public void Start()
        {
            btn_JoinRoom.onClick.AddListener(() =>
            {
                if (PhotonNetwork.InLobby)
                {
                    PhotonNetwork.LeaveLobby();
                }

                PhotonNetwork.JoinRoom(roomName);
            });
        }

        public void Initialize(string name, string mode, byte currentPlayers, byte maxPlayers)
        {
            roomName = name;
            text_RoomName.text = name;
            text_GameMode.text = mode;
            text_Players.text = currentPlayers + " / " + maxPlayers;           
        }

        public void Initialize(RoomInfo info)
        {
            roomName = info.Name;
            text_RoomName.text = info.Name;
            text_GameMode.text = (string)info.CustomProperties[GameConstants.GAME_MODE];
            Image_GameMode.color = GameConstants.GetModeColor((string)info.CustomProperties[GameConstants.GAME_MODE]);
            text_Players.text = info.PlayerCount + " / " + info.MaxPlayers;
        }
    }
}

