using System;
using Mirror;
using Steamworks;
using UI;
using UnityEngine;

namespace Multiplayer
{
    public class PlayerClient : NetworkBehaviour
    {
         // Player Data
         [SyncVar] public int ConnectionID;
         [SyncVar] public int PlayerIdNumber;
         [SyncVar] public ulong PlayerSteamID;
         [SyncVar (hook = nameof(PlayerNameUpdate))] public string PlayerName;
         [SyncVar (hook = nameof(PlayerReadyUpdate))] public bool PlayerReady;
         
         private GameNetworkManager networkManager;
         
         private GameNetworkManager NetworkManager
         {
             get
             {
                 if (networkManager != null) return networkManager;
                 return networkManager = GameNetworkManager.singleton as GameNetworkManager;
             }
         }

         private void Start()
         {
             DontDestroyOnLoad(gameObject);
         }

         public override void OnStartAuthority()
         {
             CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
             gameObject.name = "LocalPlayer";
             LobbyUIController.Instance.FindLocalPlayer(this);
             LobbyUIController.Instance.UpdateLobbyName();
         }

         public override void OnStartClient()
         {
             NetworkManager.GamePlayers.Add(this);
             LobbyUIController.Instance.UpdateLobbyName();
             LobbyUIController.Instance.UpdateLobbyPlayers();
         }

         public override void OnStopClient()
         {
             NetworkManager.GamePlayers.Remove(this);
             LobbyUIController.Instance.UpdateLobbyPlayers();
         }
         
         [Command]
         private void CmdSetPlayerName(string name)
         {
             this.PlayerNameUpdate(PlayerName, name);
         }
         
         [Command]
         private void CmdSetPlayerReady()
         {
             this.PlayerReadyUpdate(PlayerReady, !PlayerReady);
         }
         
         public void TogglePlayerReady()
         {
             if (isOwned) CmdSetPlayerReady();
         }

         public void PlayerNameUpdate(string oldName, string newName)
         {
             if (isServer)
             {
                 PlayerName = newName;
             }

             if (isClient)
             {
                 LobbyUIController.Instance.UpdateLobbyPlayers();
             }
         }
         
         private void PlayerReadyUpdate(bool oldReady, bool newReady)
         {
             if (isServer)
             {
                 this.PlayerReady = newReady;
             }
             if (isClient)
             {
                 LobbyUIController.Instance.UpdateLobbyPlayers();
             }
         }
         
         public void CanStartGame(string sceneName)
         {
             if (isOwned && isServer)
             {
                    CmdStartGame(sceneName);
             }
         }
         
         [Command]
         public void CmdStartGame(string sceneName)
         {
             NetworkManager.StartGame(sceneName);
         }
    }
}