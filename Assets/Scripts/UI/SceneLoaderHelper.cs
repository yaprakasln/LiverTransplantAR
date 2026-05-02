using UnityEngine;
using UnityEngine.SceneManagement;

namespace LiverTransplantAR.UI
{
    public class SceneLoaderHelper : MonoBehaviour
    {
        public string SceneToLoad;
        public void LoadScene()
        {
            if (!string.IsNullOrEmpty(SceneToLoad))
                SceneManager.LoadScene(SceneToLoad);
        }
    }
}
