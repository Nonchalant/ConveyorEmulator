using UnityEngine.SceneManagement;

namespace Application
{
    public interface IRouter
    {
        public void Title();
        public void Game();
        public void Exit();
    }
    
    public class Router : IRouter
    {
        public void Title()
        {
            SceneManager.LoadScene("TitleScene");
        }
        
        public void Game()
        {
            SceneManager.LoadScene("GameScene");
        }
    
        public void Exit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}