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

    public Vector2 CurrentMovementInput { get; private set; }

    private void Update()
    {       
        float playerMoveSpeed = playerStats.playerBaseMovespeed + playerStats.playerBaseMovespeed * (playerStats.playerMovespeed / 100);     
        MoveCharacterByInput(playerMoveSpeed);      
    }   

    private void MoveCharacterByInput(float playerMoveSpeed)
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 currentScale = playerCharacter.transform.localScale;
        SetSpriteDirection(inputVector, currentScale);
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
    private void SetSpriteDirection(Vector2 inputVector, Vector3 currentScale)
    {
        if (inputVector.x > 0 && currentScale.x > 0)
        {
            currentScale.x *= -1;
            playerCharacter.transform.localScale = currentScale;
        }
        else if (inputVector.x < 0 && currentScale.x < 0)
        {
            currentScale.x *= -1;
            playerCharacter.transform.localScale = currentScale;
        }
    }
}
