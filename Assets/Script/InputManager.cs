using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public PlayerInputActions PlayerInputActions { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
        PlayerInputActions = new PlayerInputActions();

        Debug.Log("InputManager : Awake - PlayerInputActions initialized");
    }

    void OnEnable()
    {
        PlayerInputActions.Enable();
    }

    void OnDisable()
    {
        PlayerInputActions.Disable();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
