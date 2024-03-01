using Multiplayer;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerListItem : MonoBehaviour
    {
        [HideInInspector]public string playerName;
        [HideInInspector]public int connectionID;
        [HideInInspector]public ulong playerSteamID;
        private bool AvatarReceived;
        
        [SerializeField] TMP_Text playerNameText;
        [SerializeField] TMP_Text readyText;
        [SerializeField] RawImage playerAvatar;
        public bool isReady;
        
        protected Callback<AvatarImageLoaded_t> avatarImageLoaded;
        
        public void ChangeReadyStatus()
        {
            readyText.text = isReady ? "Ready" : "Not Ready";
            readyText.color = isReady ? Color.green : Color.red;
        }
        
        private void Awake()
        {
            avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
        }

        void GetPlayerIcon()
        {
            int imageID = SteamFriends.GetLargeFriendAvatar(new CSteamID(playerSteamID));
            if (imageID == -1)
            {
                // No avatar
                return;
            }
            else
            {
                playerAvatar.texture = GetSteamImageAsTexture2D(imageID);
            }
        }

        public void Init(PlayerClient client)
        {
            playerName = client.PlayerName;
            connectionID = client.ConnectionID;
            playerSteamID = client.PlayerSteamID;
            ChangeReadyStatus();
            playerNameText.text = playerName;
            if (!AvatarReceived)
            {
                GetPlayerIcon();
            }
            isReady = client.PlayerReady;
        }
        
        private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
        {
            if (callback.m_steamID.m_SteamID == playerSteamID)
            {
                playerAvatar.texture = GetSteamImageAsTexture2D(callback.m_iImage);
            }
            else
            {
                // Another player
                return;
            }
        }
        
        // WARNING: DO NOT READ THIS, THIS IS STUPID SHIT TO GET THE AVATAR.
        // THIS IS NOT BY ME, IT IS BY GOOGLE NERDS.
        private Texture2D GetSteamImageAsTexture2D(int iImage)
        {
            Texture2D texture = null;
            bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
            if (isValid)
            {
                byte[] image = new byte[width * height * 4];
                isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));
                if (isValid)
                {
                    texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                    texture.LoadRawTextureData(image);
                    texture.Apply();
                }
            }
            AvatarReceived = true;
            return texture;
        }
    }
}