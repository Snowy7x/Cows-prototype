using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LobbyData : MonoBehaviour
    {
        //private Lobby lobby;
        public Button joinButton;
        public TMP_Text lobbyName;
         

        public void Init(MenuEvents events) //, Lobby lobby)
        {
            joinButton.onClick.AddListener(() =>
            {
                //events.JoinRoom(lobby); 
            });

            //lobbyName.text = lobby.Owner.Name;//lobby.GetData("Name");
            //this.lobby = lobby;
        }
        
        
    }
}