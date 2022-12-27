using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public List<Sprite> sprites;

    public int playerID;

    public bool atHome;
    public bool canMove;

    public bool completeHome;

    public bool isMoving;

    public bool powerUsed;

    public enum ColorClass {
        Red,
        Yellow,
        Green,
        Blue
    }

    public ColorClass colorClass;

    public int boardPos;

   // int spacesPerPlayer;

    public Action onMoveOver;

    float smoothTime = 0.05f;

    private Vector2 velocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetX(float x) {
        transform.position = new Vector2(x, transform.position.y);
    }

    public void SetY(float y) {
        transform.position = new Vector2(transform.position.x, y);
    }

    public void SetZ(float z) {
        transform.position = new Vector3(transform.position.x, transform.position.y,z);  
    }

    public void OnStartGame() {
        switch (colorClass) {
            case ColorClass.Red:
                gameObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;

            case ColorClass.Yellow:
                gameObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case ColorClass.Green:
                gameObject.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case ColorClass.Blue:
                gameObject.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
        }

        completeHome = false;
        atHome = true;
        canMove = true;
        isMoving = false;
        powerUsed = false;
        boardPos = GameVars.spacesPerPlayer * playerID;

        transform.position = new Vector2(GameVars.unitsPerSpace * boardPos, 0);

    }

    public IEnumerator MoveUp() {
        Vector2 targetHeight = Vector2.up*3;
        Vector2 currentHeight = transform.position;

        isMoving = true;
        while(!GameVars.eq(currentHeight, targetHeight)) {
            currentHeight = Vector2.SmoothDamp(currentHeight,targetHeight,ref velocity,smoothTime);
            transform.position = currentHeight;
            yield return null;
        }

        isMoving = false;
    }

    public IEnumerator MoveDown() {
        isMoving = true;
        Vector2 targetHeight = Vector2.up*0;
        Vector2 currentHeight = transform.position;

        while(!GameVars.eq(currentHeight,targetHeight)) {
            currentHeight = Vector2.SmoothDamp(currentHeight,targetHeight,ref velocity,smoothTime);
            transform.position = currentHeight;
            yield return null;
        }

        isMoving = false;
        onMoveOver();
    }
    public IEnumerator MoveWithBoard(int centerPos) {
        isMoving = true;

        int displacement = GameVars.mod(boardPos - centerPos + GameVars.spacesPerPlayer * GameVars.numPlayers/2, GameVars.spacesPerPlayer * GameVars.numPlayers) - GameVars.spacesPerPlayer * GameVars.numPlayers/2; 

        Vector2 currentPos = transform.position;
        Vector2 targetPos = Vector2.right * GameVars.unitsPerSpace * displacement;

        while(!GameVars.eq(currentPos, targetPos)) {
            
            currentPos = Vector2.SmoothDamp(currentPos, targetPos,ref velocity,smoothTime);
            transform.position = currentPos;
            yield return null;
        }

        isMoving = false;

    }

    public int UseDicePower(int rollNumber,  List<Piece> pieces) {
        if (colorClass == ColorClass.Yellow) {
            List<Piece> blockingPieces = pieces.FindAll(p => p.boardPos == boardPos + rollNumber * 2 && !p.atHome);

            if (!powerUsed && blockingPieces.Count == 0 && boardPos + rollNumber * 2 <= playerID * GameVars.spacesPerPlayer + GameVars.numPlayers * GameVars.spacesPerPlayer + GameVars.piecePerPlayer) {
                rollNumber *= 2;
                powerUsed = true;

                return rollNumber * 2;
            }

        } else if (colorClass == ColorClass.Blue) {
            print("WIP");
        }

        return rollNumber;
    }

    public void UseSendHomePower(Piece piece) {
        if (colorClass == ColorClass.Red) {
            piece.boardPos = piece.playerID * GameVars.spacesPerPlayer;
            piece.atHome = true;
            piece.SetZ(0);
            piece.powerUsed = false;
        }
    }

    public bool UseCanMovePower(Piece piece) {
        if (colorClass == ColorClass.Green || piece.colorClass == ColorClass.Green) {
            return true;
        }

        return false;
    }
}
