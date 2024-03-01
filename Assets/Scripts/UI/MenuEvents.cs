using System.Collections.Generic;
using System.Threading.Tasks;
using Multiplayer;
using Steamworks;
using UI;
using UnityEngine;

public class MenuEvents : MonoBehaviour
{
    public static MenuEvents Instance;
    [Header("Lobbies List")]
    [SerializeField] private Transform lobbiesParent;
    [SerializeField] private LobbyData lobbyUIPrefab;
    [SerializeField] private float LobbyUpdateRate = 1f;
    
    private bool createdPlayerItem;
    private List<LobbyData> lobbiesPrefabs;
    private List<PlayerListItem> playerPrefabs;
    private PlayerClient localPlayer;
    

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    private void Start()
    {

        /*if (!SteamManager.Initialized || !SteamUser.BLoggedOn())
        {
            SceneManager.LoadScene(0);
            return;
        }*/
        
        InvokeRepeating(nameof(UpdateLobbies), 0f, LobbyUpdateRate);
    }

    private void OnEnable()
    {
        SteamLobbyManager.Instance.OnLobbyCreatedEvent += OnLobbyCreated;
        SteamLobbyManager.Instance.OnLobbyJoinedEvent += OnLobbyJoined;
    }
    
    private void OnDisable()
    {
        SteamLobbyManager.Instance.OnLobbyCreatedEvent -= OnLobbyCreated;
        SteamLobbyManager.Instance.OnLobbyJoinedEvent -= OnLobbyJoined;
    }


    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        MenuManager.Instance.OpenMenu(callback.m_eResult != EResult.k_EResultOK ? "MainMenu" : "Loading");
    }
    
    private void OnLobbyJoined(LobbyEnter_t callback)
    {
        MenuManager.Instance.OpenMenu(callback.m_EChatRoomEnterResponse != 1 ? "MainMenu" : "Loading");
    }

    public async Task UpdateLobbies()
    {
       
    }

    public void JoinRoom(ulong lobbyId)
    {
        MenuManager.Instance.OpenMenu("Loading");
        SteamLobbyManager.Instance.JoinLobby(lobbyId);
    }
    
    public async void CreateLobby()
    {
        MenuManager.Instance.OpenMenu("Loading");
        SteamLobbyManager.Instance.CreateLobby();
    }
}
