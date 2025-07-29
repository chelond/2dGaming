using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Tooltip("Имя или индекс сцены для перехода")]
    public string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Убедись, что у игрока стоит тег Player
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
