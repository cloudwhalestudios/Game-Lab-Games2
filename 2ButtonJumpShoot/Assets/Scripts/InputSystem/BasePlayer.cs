using System;
using UnityEngine;

namespace AccessibilityInputSystem
{
    [Serializable]
    public abstract class BasePlayer
    {
        private static int NEXT_PLAYER_ID = 0;

        [SerializeField] private int _ID;
        [SerializeField] private string _name;
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private BaseInputController _inputController;
        [SerializeField] private KeyCode[] _keys;

        public int ID { get => _ID; set => _ID = value; }
        public string Name { get => _name; set => _name = value; }
        public GameObject PGameObject { get => _gameObject; set => _gameObject = value; }
        public BaseInputController InputController { get => _inputController; set => _inputController = value; }
        public KeyCode[] Keys { get => _keys; set => _keys = value; }

        public BasePlayer(string name, Transform parent = null)
        {
            // Set the ID
            ID = NEXT_PLAYER_ID++;
            Name = name;

            if (BasePlayerManager.Instance?.playerPrefab != null)
            {
                PGameObject = UnityEngine.Object.Instantiate(BasePlayerManager.Instance.playerPrefab);
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
            else
            {
                PGameObject.transform.SetParent(BasePlayerManager.Instance.transform);
            }
        }

        public virtual void Destroy()
        {
            UnityEngine.Object.DestroyImmediate(PGameObject);
        }
    }
}