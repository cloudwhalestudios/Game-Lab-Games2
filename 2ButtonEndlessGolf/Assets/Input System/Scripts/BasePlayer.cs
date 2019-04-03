using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccessibilityInputSystem
{
    [Serializable]
    public abstract class BasePlayer : MonoBehaviour
    {
        private static int NEXT_PLAYER_ID = 0;

        [SerializeField] private int _ID;
        [SerializeField] private string _name;
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private BaseInputController _inputController;

        public int ID { get => _ID; set => _ID = value; }
        public string Name { get => _name; set => _name = value; }
        public GameObject PGameObject { get => _gameObject; set => _gameObject = value; }
        public BaseInputController InputController { get => _inputController; set => _inputController = value; }

        public BasePlayer(string name, Transform parent = null)
        {
            // Set the ID
            ID = NEXT_PLAYER_ID++;

            if (BasePlayerManager.Instance?.playerPrefab != null)
            {
                PGameObject = Instantiate(GameManager.Instance.playerPrefab);
            }
            else
            {
                PGameObject = new GameObject();
            }
            PGameObject.name = name;
            if (parent != null)
            {
                PGameObject.transform.SetParent(parent);
            }

            // Check components; add if necessary
            CheckInputController();
        }

        protected abstract void CheckInputController();
    }
}