using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Thesis
{
    public class MenuManager : MonoBehaviour
    {
        public void ConfigRoom() { StartCoroutine(LoadSceneAsync("RoomConfigurator")); }

        public void PlayGame()
        {
            if (!Room.Instance.IsReady) ConfigRoom();
            else StartCoroutine(LoadSceneAsync("Game"));
        }

        IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);
            while (!asyncLoad.isDone) yield return null;
        }

        public void Quit() { Application.Quit(); }
    }
}