using UnityEngine;
using UnityEngine.SceneManagement;

namespace LiverTransplantAR.UI
{
    public class MenuSceneLoader : MonoBehaviour
    {
        public string targetScene;

        public void LoadTarget()
        {
            if (!string.IsNullOrEmpty(targetScene))
            {
                Debug.Log($"<color=orange>MENU: Loading Scene -> {targetScene}</color>");
                SceneManager.LoadScene(targetScene);
            }
        }
    }
}
