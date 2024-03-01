using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer
{
	public class GameNetworkManager : NetworkManager {
		[SerializeField] private PlayerClient gamePlayerPrefab;
		public List<PlayerClient> GamePlayers = new List<PlayerClient>();
		
		public override void OnServerAddPlayer(NetworkConnectionToClient conn)
		{
			if (Utils.IsSceneActive(onlineScene))
			{
				var player = Instantiate(gamePlayerPrefab);
				player.ConnectionID = conn.connectionId;
				player.PlayerIdNumber = GamePlayers.Count;
				player.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobbyManager.Instance.lobbyId, GamePlayers.Count);
				NetworkServer.AddPlayerForConnection(conn, player.gameObject);
			}
		}

		public void StartGame(string sceneName)
		{
			ServerChangeScene(sceneName);
		}
	}
}
