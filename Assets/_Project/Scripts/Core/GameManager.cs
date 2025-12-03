using UnityEngine;
using _Project.Systems.Audio;
using _Project.Systems.SaveLoad;


namespace _Project.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCoreSystems();
        }

        private void InitializeCoreSystems()
        {
            Debug.Log("[GameManager] Initializing core systems...");
            ServiceLocator.Register<EventBus>(new EventBus());
            ServiceLocator.Register<AudioSystem>(new AudioSystem());
            ServiceLocator.Register<SaveLoadSystem>(new SaveLoadSystem());
        }
    }
}
