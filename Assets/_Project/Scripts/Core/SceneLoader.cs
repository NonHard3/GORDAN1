using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

namespace Project.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public static IEnumerator LoadSceneAsync(string sceneName, float delay = 0f)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            while (!asyncLoad.isDone)
                yield return null;
        }
    }
}