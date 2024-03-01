using System.Collections.Generic;
using System.Linq;
using Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LobbyUIController : MonoBehaviour
    {
        public static LobbyUIController Instance;
        [Header("Lobby")] [SerializeField] TMP_Text lobbyName;
        [SerializeField] private Transform playersParent;
        [SerializeField] private PlayerListItem playerUIPrefab;
        [SerializeField] private Button startGameButton;
        [SerializeField] TMP_Text readyButtonText;

        private bool createdPlayerItem;
        private List<PlayerListItem> playerPrefabs;
        private PlayerClient localPlayer;

        private GameNetworkManager networkManager;

        private GameNetworkManager NetworkManager
        {
            get
            {
                if (networkManager != null) return networkManager;
                return networkManager = GameNetworkManager.singleton as GameNetworkManager;
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void ReadyPlayer()
        {
            localPlayer.TogglePlayerReady();
        }

        public void UpdateButton()
        {
            if (localPlayer.PlayerReady)
            {
                readyButtonText.text = "Unready";
            }
            else
            {
                readyButtonText.text = "Ready Up";
            }
        }

        public void CheckIfAllReady()
        {
            bool allReady = true;
            foreach (var player in NetworkManager.GamePlayers)
            {
                if (!player.PlayerReady)
                {
                    allReady = false;
                }
            }
            
            startGameButton.interactable = allReady;
        }

        public void FindLocalPlayer(PlayerClient client)
        {
            localPlayer = client;
            startGameButton.gameObject.SetActive(localPlayer.isServer);
        }

        public void UpdateLobbyName()
        {
            lobbyName.text = SteamLobbyManager.Instance.GetLobbyName();
        }

        public void UpdateLobbyPlayers()
        {
            if (playerPrefabs == null)
            {
                playerPrefabs = new List<PlayerListItem>();
            }

            Debug.Log("UpdateLobbyPlayers");
            Debug.Log($"NetworkManager.GamePlayers.Count: {NetworkManager.GamePlayers.Count}");
            Debug.Log($"playerPrefabs.Count: {playerPrefabs.Count}");
            Debug.Log($"createdPlayerItem: {createdPlayerItem}");
            
            if (!createdPlayerItem)
            {
                CreateHostPlayerItem();
            }

            if (playerPrefabs.Count < NetworkManager.GamePlayers.Count)
            {
                CreateClientPlayerItem();
            }
            if (playerPrefabs.Count > NetworkManager.GamePlayers.Count)
            {
                RemovePlayerItem();
            }
            if (playerPrefabs.Count == NetworkManager.GamePlayers.Count)
            {
                UpdatePlayerItem();
            }
        }

        private void CreateHostPlayerItem()
        {

            foreach (var player in NetworkManager.GamePlayers)
            {
                var playerItem = Instantiate(playerUIPrefab, playersParent);
                playerItem.transform.localScale = Vector3.one;

                playerItem.Init(player);
                playerItem.isReady = player.PlayerReady;
                
                playerPrefabs.Add(playerItem);
            }

            createdPlayerItem = true;
        }

        private void CreateClientPlayerItem()
        {
            foreach (var player in NetworkManager.GamePlayers)
            {
                if (playerPrefabs.Any(b => b.connectionID == player.ConnectionID)) continue;
                var playerItem = Instantiate(playerUIPrefab, playersParent);
                playerItem.transform.localScale = Vector3.one;

                playerItem.Init(player);
                playerItem.isReady = player.PlayerReady;
                playerPrefabs.Add(playerItem);
            }
        }

        private void UpdatePlayerItem()
        {
            Debug.Log("UpdatePlayerItem");

            foreach (var player in NetworkManager.GamePlayers)
            {
                var playerItem = playerPrefabs.FirstOrDefault(b => b.connectionID == player.ConnectionID);
                if (playerItem == null)
                {
                    Debug.Log("playerItem == null");
                    continue;
                }
                playerItem.Init(player);
                if (player == localPlayer) { UpdateButton(); } 
            }
            
            CheckIfAllReady();
        }

        private void RemovePlayerItem()
        {
            List<PlayerListItem> toRemove = new List<PlayerListItem>();
            foreach (var player in playerPrefabs)
            {
                if (!NetworkManager.GamePlayers.Any(b => b.ConnectionID == player.connectionID))
                {
                    toRemove.Add(player);
                }
            }

            if (toRemove.Count > 0)
            {
                foreach (var player in toRemove)
                {
                    GameObject playerObject = player.gameObject;
                    playerPrefabs.Remove(player);
                    Destroy(playerObject);
                    playerObject = null;
                }
            }
        }
        
        public void StartGame(string sceneName)
        {
            localPlayer.CanStartGame(sceneName);
        }
    }
}