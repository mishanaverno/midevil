using UnityEngine.SceneManagement;

namespace Game
{
    public class SceneManager : Singletone<SceneManager>
    {
        public void LoadBattleScene(string location)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(location, LoadSceneMode.Single);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Battle", LoadSceneMode.Additive);

        }
    }
}
