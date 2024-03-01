using System;
using Mirror;
using Steamworks;
using UnityEngine;

namespace Multiplayer
{
    public class SteamLobbyManager : MonoBehaviour
    {
        public static SteamLobbyManager Instance;
        
        // Events
        public event Action<LobbyCreated_t> OnLobbyCreatedEvent;
        public event Action<LobbyEnter_t> OnLobbyJoinedEvent;
        
        // Callbacks
        protected Callback<LobbyCreated_t> LobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> LobbyJoinRequested;
        protected Callback<LobbyEnter_t> LobbyEntered;
        
        // Variables
        public ulong lobbyId;
        private const string HostAddressKey = "HostAddress";
        private GameNetworkManager networkManager;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            if (!SteamManager.Initialized) return;
            networkManager = GetComponent<GameNetworkManager>();
            LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            LobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
            LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);   
        }
        
        public void CreateLobby()
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, networkManager.maxConnections);
        }
        
        public void JoinLobby(ulong lobbyId)
        {
            SteamMatchmaking.JoinLobby(new CSteamID(lobbyId));
        }
        
        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogError("Failed to create lobby");
                return;
            }
            
            lobbyId = callback.m_ulSteamIDLobby;
            Debug.Log($"Lobby created successfully: {lobbyId}");
            
            networkManager.StartHost();
            SteamMatchmaking.SetLobbyData(new CSteamID(lobbyId), HostAddressKey, SteamUser.GetSteamID().ToString());
            SteamMatchmaking.SetLobbyData(new CSteamID(lobbyId), "Name", SteamFriends.GetPersonaName() + "'s Lobby");
            OnLobbyCreatedEvent?.Invoke(callback);
        }
        
        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            lobbyId = callback.m_ulSteamIDLobby;
            if (NetworkServer.active) return;
            string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
            networkManager.networkAddress = hostAddress;
            networkManager.StartClient();
            OnLobbyJoinedEvent?.Invoke(callback);
        }
        
        private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        public string GetLobbyName()
        {
            if (lobbyId == 0) return "No Lobby";
            return SteamMatchmaking.GetLobbyData(new CSteamID(lobbyId), "Name");
        }
    }
}