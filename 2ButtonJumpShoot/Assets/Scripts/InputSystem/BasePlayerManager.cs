using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccessibilityInputSystem
{
    public abstract class BasePlayerManager : MonoBehaviour
    {
        [Serializable]
        public struct KeyEventSpecifier
        {
            string _specifier;
            KeyCode _key;

            public string Specifier { get => _specifier; private set => _specifier = value; }
            public KeyCode Key { get => _key; private set => _key = value; }

            public KeyEventSpecifier(string specifier, KeyCode key)
            {
                _specifier = specifier;
                _key = key;
            }

            
        }

        public static event Action<BasePlayer> NewPlayerAdded;
        /**
         * <summary>
         * <c string>A text to broadcast (i.e "Waiting for input for action x")</c>
         * <c KeyEventSpecifier[]>The keys that have already been used with a short specifier</c>
         * </summary>
         * */
        public static event Action<string, KeyEventSpecifier[]> NewPlayerBeingAdded;
        public static event Action<string, KeyEventSpecifier> NewPlayerKeyInUse;
        public static event Action<int> PlayerRemoved;

        public static BasePlayerManager Instance { get; private set; }
        public int PlayerCount => players.Count;

        [Header("Configuration")]
        [SerializeField] public GameObject playerPrefab;
        [SerializeField] public Transform playerParent;
        [SerializeField] protected bool shouldCheckForNewPlayer;
        [SerializeField] protected int maxPlayers = 1;

        [Header("Players")]
        [SerializeField, ReadOnly] protected Dictionary<KeyCode, BasePlayer> playerKeyBindings;
        [SerializeField, ReadOnly] protected List<BasePlayer> players;

        public BasePlayer GetPlayer()
        {
            if (players.Count >= 1) {
                return GetPlayer(players[0].Keys[0]);
            }
            return null;
        }
        public BasePlayer GetPlayer(KeyCode keyCode) => playerKeyBindings?[keyCode];

        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);

                playerKeyBindings = new Dictionary<KeyCode, BasePlayer>();
                players = new List<BasePlayer>();
            }
            else
            {
                Instance.shouldCheckForNewPlayer = shouldCheckForNewPlayer;
                DestroyImmediate(gameObject);
            }
        }

        protected void OnDestroy()
        {
            if (Instance == this) { Instance = null; }
            StopAllCoroutines();
        }

        protected void Update()
        {
            if (players?.Count >= maxPlayers)
            {
                shouldCheckForNewPlayer = false;
            }
            else if (players?.Count == 0)
            {
                shouldCheckForNewPlayer = true;
            }

            if (shouldCheckForNewPlayer)
            {
                CheckForNewPlayer();
            }
        }

        public KeyCode GetKeyInput()
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    return kcode;
                }
            }
            return KeyCode.None;
        }

        protected void NewPlayerWasAdded(BasePlayer player) => NewPlayerAdded?.Invoke(player);
        protected void NewPlayerIsBeingAdded(string message, params KeyEventSpecifier[] keyEventSpecifiers) => NewPlayerBeingAdded?.Invoke(message, keyEventSpecifiers);
        protected void NewPlayerKeyIsInUse(string message, KeyEventSpecifier keyEventSpecifier) => NewPlayerKeyInUse?.Invoke(message, keyEventSpecifier);
        protected void PlayerWasRemoved() => PlayerRemoved?.Invoke(players.Count);

        public virtual void RemovePlayer() { if (PlayerCount > 0) RemovePlayer(players[players.Count - 1].Keys[0]); }
        public virtual void RemovePlayer(KeyCode keyCode)
        {
            var player = playerKeyBindings?[keyCode];
            if (player == null) return;

            // Remove from key mapping lookup
            foreach (var key in player.Keys)
            {
                playerKeyBindings.Remove(key);
            }

            // Remove from players
            players.Remove(player);

            // Cleanup
            player.Destroy();

            PlayerWasRemoved();
        }

        protected abstract void CheckForNewPlayer();
        protected abstract void AddPlayer(params KeyCode[] keyCodes);
    }
}