using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _content;

    [Header("INPUT")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private InputAction _backInputAction;

    [Header("MENU")]
    [SerializeField] private GameObject _menu;
    [SerializeField] private Text _title;
    [SerializeField] private Text _saveText;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Text _loadText;
    [SerializeField] private Button _loadButton;
    [SerializeField] private Text _controlsText;
    [SerializeField] private Button _controlsButton;
    [SerializeField] private Text _soundText;
    [SerializeField] private Button _soundButton;
    [SerializeField] private Text _exitText;
    [SerializeField] private Button _exitButton;

    [Header("SAVE")]
    [SerializeField] private GameObject _save;

    [Header("LOAD")]
    [SerializeField] private GameObject _load;

    [Header("CONTROLS")]
    [SerializeField] private GameObject _controls;

    [Header("SOUND")]
    [SerializeField] private GameObject _sound;

    [Header("EXIT")]
    [SerializeField] private GameObject _exit;
    [SerializeField] private Text _exitConfirmationText;
    [SerializeField] private Text _yesText;
    [SerializeField] private Text _noText;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;

    [Header("BACK")]
    [SerializeField] private Button _backButton;

    [Header("ANIMATIONS")]
    [SerializeField] private Animation _animation;
    //[SerializeField] private AnimationClip _introAnimation;

    private bool isAnimating = false;
    private float currentTime;
    private float previousTime;
    private float deltaTime;
    private float animationTime = 0.0f;

    private GameObject _currentPanel = null;
    private bool isPaused = false;

    void OnEnable()
    {
        _content.SetActive(false);
        _backInputAction.Enable();

        ToggleCursor(false);

        _currentPanel = _menu;
        SetTextsAndButtons();
    }

    void OnDisable() => _backInputAction.Disable();

    private void Start() => _backInputAction.performed += _ => Back();

    public void Back()
    {
        if (_currentPanel == _menu)
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0.0f : 1.0f;
            _playerInput.SwitchCurrentActionMap(isPaused ? "UI" : "Player");
            _content.SetActive(isPaused);
            ToggleCursor(isPaused);

            if (isPaused)
            {
                _menu.SetActive(true);
                _save.SetActive(false);
                _load.SetActive(false);
                _controls.SetActive(false);
                _sound.SetActive(false);
                _exit.SetActive(false);
            }

            PlayAnimation();
            return;
        }

        TogglePanels(_menu);
    }

    void Update()
    {
        // Animation update
        if (_animation.clip && isAnimating == true)
        {
            currentTime = Time.realtimeSinceStartup;
            deltaTime = currentTime - previousTime;
            previousTime = currentTime;
            _animation[_animation.clip.name].time = animationTime;
            _animation.Sample();
            animationTime += deltaTime;

            if (animationTime >= _animation.clip.length)
            {
                _animation[_animation.clip.name].time = _animation.clip.length;
                _animation.Sample();
                isAnimating = false;
            }
        }
    }

    void PlayAnimation()
    {
        animationTime = 0.0f;
        previousTime = currentTime = Time.realtimeSinceStartup;
        isAnimating = true;
        _animation.Play();
    }

    void SetTextsAndButtons()
    {
        // TODO: localization
        _title.text = "Game paused";

        _saveText.text = "Save";
        _saveButton.onClick.RemoveAllListeners();
        _saveButton.onClick.AddListener(() => TogglePanels(_save));

        _loadText.text = "Load";
        _loadButton.onClick.RemoveAllListeners();
        _loadButton.onClick.AddListener(() => TogglePanels(_load));

        _controlsText.text = "Controls";
        _controlsButton.onClick.RemoveAllListeners();
        _controlsButton.onClick.AddListener(() => TogglePanels(_controls));

        _soundText.text = "Sound";
        _soundButton.onClick.RemoveAllListeners();
        _soundButton.onClick.AddListener(() => TogglePanels(_sound));

        _exitText.text = "Exit";
        _exitButton.onClick.RemoveAllListeners();
        _exitButton.onClick.AddListener(() => TogglePanels(_exit));

        _backButton.onClick.RemoveAllListeners();
        _backButton.onClick.AddListener(Back);

        _exitConfirmationText.text = "Are you sure?";
        _yesText.text = "Yes";
        _yesButton.onClick.RemoveAllListeners();
        _yesButton.onClick.AddListener(ExitGame);
        _noText.text = "No";
        _noButton.onClick.RemoveAllListeners();
        _noButton.onClick.AddListener(() => TogglePanels(_menu));
    }

    void TogglePanels(GameObject panel)
    {
        _currentPanel.SetActive(false);
        panel.SetActive(true);
        _currentPanel = panel;
    }

    void ToggleCursor(bool isOn)
    {
        Cursor.lockState = isOn ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = isOn;
    }

    void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif // UNITY_EDITOR
    }
}
