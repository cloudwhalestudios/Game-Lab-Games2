using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class TwoButtonPlayerManager : BasePlayerManager
        {
            [Header("New Player Input")]
            [SerializeField, ReadOnly] private bool waitingForNextInput;
            [SerializeField, ReadOnly] private KeyCode newPlayer_primaryKey;
            [SerializeField, ReadOnly] private KeyCode newPlayer_secondaryKey;

            protected override void AddPlayer(params KeyCode[] keyCodes)
            {
                if (keyCodes?.Length < 2)
                {
                    throw new System.ArgumentNullException(nameof(keyCodes));
                }

                var newPlayer = new TwoButtonPlayer($"P{players.Count + 1}", keyCodes[0], keyCodes[1], playerParent);
                players.Add(newPlayer);

                // Register Keybinds
                playerKeyBindings.Add(keyCodes[0], newPlayer);
                playerKeyBindings.Add(keyCodes[1], newPlayer);

                NewPlayerWasAdded(newPlayer);
            }

            protected override void CheckForNewPlayer()
            {
                var key = GetKeyInput();

                if (key != KeyCode.None)
                {
                    if (players?.Count >= maxPlayers)
                    {
                        print("Reached player limit " + players.Count);
                        return;
                    }

                    if (!waitingForNextInput)
                    {
                        waitingForNextInput = true;
                        newPlayer_primaryKey = key;
                        newPlayer_secondaryKey = KeyCode.None;
                        // TODO add text
                        NewPlayerIsBeingAdded($"P{players.Count + 1}", new KeyEventSpecifier("Primary", newPlayer_primaryKey));
                    }
                    else
                    {
                        if (key == newPlayer_primaryKey || playerKeyBindings.ContainsKey(key))
                        {
                            // Key already in use
                            NewPlayerKeyIsInUse("", new KeyEventSpecifier("", key));
                            return;
                        }
                        newPlayer_secondaryKey = key;

                        waitingForNextInput = false;
                        AddPlayer(newPlayer_primaryKey, newPlayer_secondaryKey);
                    }
                }
            }
        }
    }
}
