using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Player player;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float playerRadius;
    [SerializeField] private GameObject playerCharacter;

    private GameObject _characterVisuals;
    

    public Vector2 CurrentMovementInput { get; private set; }

    private void Start()
    {
        gameManager.OnCharacterSet += SetCharacterOnGameStart;
    }

    private void OnDestroy()
    {
        gameManager.OnCharacterSet -= SetCharacterOnGameStart;
    }

    private void SetCharacterOnGameStart()
    {
        GameObject chosenCharacter = playerCharacter.transform.GetChild(0).gameObject;
        _characterVisuals = chosenCharacter.transform.Find("CharacterVisuals").gameObject;
    }

    private void Update()
    {
        if (_characterVisuals != null)
        {
            float playerMoveSpeed = playerStats.playerBaseMovespeed + playerStats.playerBaseMovespeed * (playerStats.playerMovespeed / 100);     
            MoveCharacterByInput(playerMoveSpeed); 
        }
    }   

    private void MoveCharacterByInput(float playerMoveSpeed)
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        
        SetSpriteDirection(inputVector);
        player.transform.position += (Vector3)inputVector * playerMoveSpeed * Time.deltaTime;

        CurrentMovementInput = inputVector;       
    }

    private void LateUpdate()
    {
        ClampToMapBounds(gameManager.mapSize);
    }

    private void ClampToMapBounds(Bounds mapBounds)
    {
        Vector3 currentPos = player.transform.position;

        float minX = mapBounds.min.x + playerRadius;
        float minY = (mapBounds.min.y + playerRadius) * 0.97f;
        float maxX = mapBounds.max.x - playerRadius;
        float maxY = (mapBounds.max.y - playerRadius) * 1.03f;

        currentPos.x = Mathf.Clamp(currentPos.x, minX, maxX);
        currentPos.y = Mathf.Clamp(currentPos.y, minY, maxY);
        player.transform.position = currentPos;
    }
    private void SetSpriteDirection(Vector2 inputVector)
    {
        Vector3 currentScale = _characterVisuals.transform.localScale;
        if (inputVector.x > 0 && currentScale.x > 0)
        {
            currentScale.x *= -1;
            _characterVisuals.transform.localScale = currentScale;
        }
        else if (inputVector.x < 0 && currentScale.x < 0)
        {
            currentScale.x *= -1;
            _characterVisuals.transform.localScale = currentScale;
        }
    }
}
