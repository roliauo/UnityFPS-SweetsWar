﻿using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class RoomListItem : MonoBehaviour
    {
        public Text text_RoomName;
        public Text text_GameMode;
        public Image image_GameMode;
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

        public void SetInfo(RoomInfo info)
        {
            Debug.Log("info.ToStringFull(): " + info.ToStringFull());
            roomName = info.Name;
            text_RoomName.text = info.Name;

            object gameMode;
            info.CustomProperties.TryGetValue(GameConstants.GAME_MODE, out gameMode);
            text_GameMode.text = (string)gameMode;

            image_GameMode.color = GameConstants.GetModeColor((string)info.CustomProperties[GameConstants.GAME_MODE]);
            text_Players.text = info.PlayerCount + " / " + info.MaxPlayers;
        }
    }
}

