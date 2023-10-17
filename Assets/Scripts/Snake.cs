using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private enum Direction
    {
        Left,
        Right,
        Down,
        Up
    }
    
    private class SnakeBodyPart
    {
        private Vector2Int gridPosition; // Posición 2D de la SnakeBodyPart
        private Transform transform;

        public SnakeBodyPart(int bodyIndex)
        {
            GameObject snakeBodyPartGameObject = new GameObject("Snake Body",
                typeof(SpriteRenderer));
            SpriteRenderer snakeBodyPartSpriteRenderer = snakeBodyPartGameObject.GetComponent<SpriteRenderer>();
            snakeBodyPartSpriteRenderer.sprite = 
                GameAssets.Instance.snakeBodySprite;
            snakeBodyPartSpriteRenderer.sortingOrder = -bodyIndex;
            transform = snakeBodyPartGameObject.transform;
        }
        
        public void SetGridPosition(Vector2Int gridPosition)
        {
            this.gridPosition = gridPosition; // Posición 2D de la SnakeBodyPart
            transform.position = new Vector3(gridPosition.x, gridPosition.y, 0); // Posición 3D del G.O.
        }
    }
    
    private class SnakeMovePosition
    {
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(Vector2Int gridPosition, Direction direction)
        {
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }
    }
    
    private Vector2Int gridPosition; // Posición 2D de la cabeza
    private Vector2Int startGridPosition;
    private Direction gridMoveDirection; // Dirección de la cabeza

    private float horizontalInput, verticalInput;

    private float gridMoveTimer;
    private float gridMoveTimerMax = 1f; // La serpiente se moverá a cada segundo

    private LevelGrid levelGrid;

    private int snakeBodySize; // Cantidad de partes del cuerpo (sin cabeza)
    private List<SnakeMovePosition> snakeMovePositionsList; // Posiciones y direcciones de cada parte (por orden)
    private List<SnakeBodyPart> snakeBodyPartsList;
    
    
    private void Awake()
    {
        startGridPosition = new Vector2Int(0, 0);
        gridPosition = startGridPosition;

        gridMoveDirection = Direction.Up; // Dirección arriba por defecto
        transform.eulerAngles = Vector3.zero; // Rotación arriba por defecto

        snakeBodySize = 0;
        snakeMovePositionsList = new List<SnakeMovePosition>();
        snakeBodyPartsList = new List<SnakeBodyPart>();
    }

    private void Update()
    {
        HandleMoveDirection();
        HandleGridMovement();
    }

    public void Setup(LevelGrid levelGrid)
    {
        // levelGrid de snake = levelGrid que viene por parámetro
        this.levelGrid = levelGrid;
    }

    private void HandleGridMovement() // Relativo al movimiento en 2D
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;

            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(gridPosition, gridMoveDirection);
            snakeMovePositionsList.Insert(0, snakeMovePosition);

            Vector2Int gridMoveDirectionVector;
            switch (gridMoveDirection)
            {
                default:
                case Direction.Left:
                    gridMoveDirectionVector = new Vector2Int(-1, 0);
                    break;
                case Direction.Right:
                    gridMoveDirectionVector = new Vector2Int(1, 0);
                    break;
                case Direction.Down:
                    gridMoveDirectionVector = new Vector2Int(0, -1);
                    break;
                case Direction.Up:
                    gridMoveDirectionVector = new Vector2Int(0, 1);
                    break;
            }
            gridPosition += gridMoveDirectionVector;

            // ¿He comido comida?
            bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);
            if (snakeAteFood)
            {
                // El cuerpo crece
                snakeBodySize++;
                CreateBodyPart();
            }

            if (snakeMovePositionsList.Count > snakeBodySize)
            {
                snakeMovePositionsList.
                    RemoveAt(snakeMovePositionsList.Count - 1);
            }

            transform.position = new Vector3(gridPosition.x, gridPosition.y, 0);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector));
            UpdateBodyParts();
        }
    }

    private void HandleMoveDirection()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        // Cambio dirección hacia arriba
        if (verticalInput > 0) // Si he pulsado hacia arriba (W o Flecha Arriba)
        {
            if (gridMoveDirection != Direction.Down) // Si iba en horizontal
            {
                // Cambio la dirección hacia arriba (0,1)
                gridMoveDirection = Direction.Up;
            }
        }
        
        // Cambio dirección hacia abajo
        // Input es abajo?
        if (verticalInput < 0)
        {
            // Mi dirección hasta ahora era horizontal
            if (gridMoveDirection != Direction.Up)
            {
                gridMoveDirection = Direction.Down;
            }
        }

        // Cambio dirección hacia derecha
        if (horizontalInput > 0)
        {
            if (gridMoveDirection != Direction.Left)
            {
                gridMoveDirection = Direction.Right;
            }
        }
        
        // Cambio dirección hacia izquierda
        if (horizontalInput < 0)
        {
            if (gridMoveDirection != Direction.Right)
            {
                gridMoveDirection = Direction.Left;
            }
        }
    }

    private float GetAngleFromVector(Vector2Int direction)
    {
        float degrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (degrees < 0)
        {
            degrees += 360;
        }

        return degrees - 90;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    public List<Vector2Int> GetFullSnakeBodyGridPosition()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionsList)
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }

    private void CreateBodyPart()
    {
        snakeBodyPartsList.Add(new SnakeBodyPart(snakeBodySize));
    }

    private void UpdateBodyParts()
    {
        for (int i = 0; i < snakeBodyPartsList.Count; i++)
        {
            snakeBodyPartsList[i].SetGridPosition(snakeMovePositionsList[i].GetGridPosition());
        }
    }
}
