using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject background;
    private LevelGrid levelGrid;
    private Snake snake;

    private bool isPaused;

    private void Awake()
    {
        // Singleton
        if (Instance != null)
        {
            Debug.LogError("There is more than one Instance");
        }

        Instance = this;
    }

    private void Start()
    {
        SoundManager.CreateSoundManagerGameObject();

        // Configuración de la cabeza de serpiente
        GameObject snakeHeadGameObject = new GameObject("Snake Head");
        SpriteRenderer snakeSpriteRenderer = snakeHeadGameObject.AddComponent<SpriteRenderer>();
        snakeSpriteRenderer.sprite = GameAssets.Instance.snakeHeadSprite;
        snake = snakeHeadGameObject.AddComponent<Snake>();

        // Configurar el LevelGrid
        levelGrid = new LevelGrid(10, 10);
        snake.Setup(levelGrid);
        levelGrid.Setup(snake);

        // Inicializo tema score
        Score.InitializeStaticScore();

        isPaused = false;
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     Loader.Load(Loader.Scene.Game);
        // }

        // Lógica de Pause con tecla Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void ChangeBackgroundSize(float newScale)
    {
        if (background != null)
        {
            newScale++;
            background.transform.localScale = new Vector3(newScale, newScale, 1f);
        }
        else
        {
            Debug.LogError("Background no está asignado en el GameManager.");
        }
    }

    public void ChangeMapSize(int newWidth, int newHeight)
    {
        if (levelGrid != null)
        {
            levelGrid.SetMapSize(newWidth, newHeight);
        }
        else
        {
            Debug.LogError("LevelGrid no está asignado en el GameManager.");
        }
    }

    public void SnakeDied()
    {
        GameOverUI.Instance.Show(Score.TrySetNewHighScore());
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        PauseUI.Instance.Show();
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        PauseUI.Instance.Hide();
        isPaused = false;
    }
}
