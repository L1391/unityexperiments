using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    public GameObject pieceObject;


    List<List<Piece>> playerPieces;

    List<Piece> allPieces;

    List<Piece> currentPieces;
    List<Piece> movablePieces;

    int currentPiece = 0;

    Dice gameDice;

    IEnumerator currentRoutine;

    int turn;

    public enum GameState {
        OnMenu,
        Paused,
        OnRolling,
        OnMovementSelect,
        OnMoving,
    }

    public GameState currentState;


    // Start is called before the first frame update
    void Start()
    {


        gameDice = FindObjectOfType<Dice>();
        gameDice.onRollDone += RollComplete;

        StartGame();
 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            PrintGame();
        }

        switch (currentState) {
            case GameState.OnMenu:

                break;

            case GameState.Paused:

                break;

            case GameState.OnRolling:
                if(currentRoutine == null) {
                    int playerTurn = turn % GameVars.numPlayers;
                    currentPieces = playerPieces[playerTurn];

                    currentRoutine = NewTurnRollSequence();
                    StartCoroutine(currentRoutine);
                }

                break;

            case GameState.OnMovementSelect:
                foreach(Piece p in allPieces) {
                    if (movablePieces[currentPiece] == p) {
                        p.SetY(1);
                    } else {
                        p.SetY(0);
                    }
                }

                if(Input.GetKeyDown(KeyCode.RightArrow)) {
                    print("right");
                    currentPiece = GameVars.mod((currentPiece + 1), movablePieces.Count);

                    if(currentRoutine != null) {
                        print("stopping routine");
                        StopAllCoroutines();
                        //StopCoroutine(currentRoutine);
                    }

                    
                    currentRoutine = MovePieces(allPieces,movablePieces[currentPiece].boardPos,false);
                    StartCoroutine(currentRoutine);

                } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                    print("left");
                    currentPiece = GameVars.mod((currentPiece - 1) ,movablePieces.Count);

                    if(currentRoutine != null) {
                        print("stopping routine");
                        StopAllCoroutines();
                        //StopCoroutine(currentRoutine);
                    }
                    
                    currentRoutine = MovePieces(allPieces,movablePieces[currentPiece].boardPos,false);
                    StartCoroutine(currentRoutine);

                } else if (Input.GetKeyDown(KeyCode.C)) {
                    print("confirmed");
                    CalculateMove(movablePieces[currentPiece], gameDice.rollNumber);
                } else if (Input.GetKeyDown(KeyCode.X)) {
                    gameDice.rollNumber = movablePieces[currentPiece].UseDicePower(gameDice.rollNumber,currentPieces);
                }

                break;
            case GameState.OnMoving:
                if(currentRoutine == null) {
                    print("moving piece");
                   
                    currentRoutine = MovePieceSequence(movablePieces[currentPiece]);
                    
                    StartCoroutine(currentRoutine);
                }
                break;
        }
        
    }

    void RollComplete() {
        currentRoutine = null;

        print("roll complete");
        

         CalculateMovablePieces(ref currentPieces, gameDice.rollNumber);

         movablePieces = currentPieces.FindAll( p => p.canMove);

        
        if (movablePieces.Count == 0) {
            print("no movable pieces");
            MoveComplete();
        } else {
            StartCoroutine(MovePieces(allPieces,movablePieces[0].boardPos,true));
            currentState = GameState.OnMovementSelect;
        }

        currentPiece = 0;

    }

    void MoveComplete() {
        currentRoutine = null; 

        print("move complete");

        List<Piece> completePieces = currentPieces.FindAll(p => p.completeHome);

        if (completePieces.Count == GameVars.piecePerPlayer) {
            print("Player " + turn % GameVars.numPlayers + " wins!");
            currentState = GameState.OnMenu;
        }

        if (gameDice.rollNumber != 6) {
            print("next turn");
            turn++;
        }

        currentState = GameState.OnRolling;
    }

    void CalculateMovablePieces(ref List<Piece> currentPieces, int rollNumber) {

        foreach (Piece piece in currentPieces) {

                List<Piece> blockingPieces;

                if (piece.atHome) {
                    //blockingPieces = playerPieces[piece.playerID].FindAll(p => ((p.boardPos) % (GameVars.spacesPerPlayer * GameVars.numPlayers) == (piece.boardPos) % (GameVars.spacesPerPlayer * GameVars.numPlayers)) && !p.atHome);
                    blockingPieces = allPieces.FindAll(p => p.boardPos == piece.playerID * GameVars.spacesPerPlayer && !p.atHome && (p.playerID == piece.playerID || piece.UseCanMovePower(p)));
                } else {
                    //blockingPieces = playerPieces[piece.playerID].FindAll(p => (p.boardPos) % (GameVars.spacesPerPlayer * GameVars.numPlayers) == (piece.boardPos + rollNumber) % (GameVars.spacesPerPlayer * GameVars.numPlayers) && !p.atHome);
                    blockingPieces = allPieces.FindAll(p => p.boardPos == piece.boardPos + rollNumber && !p.atHome && (p.playerID == piece.playerID || piece.UseCanMovePower(p)));
                }
        
                blockingPieces.Remove(piece);

                if (blockingPieces.Count > 0 || piece.completeHome || (rollNumber != 6 && piece.atHome) || piece.boardPos + rollNumber >= piece.playerID * GameVars.spacesPerPlayer + GameVars.numPlayers * GameVars.spacesPerPlayer + GameVars.piecePerPlayer) {
                    piece.canMove = false;
                } else  {
                    piece.canMove = true;
                }

        }
    }

    void CalculateMove(Piece piece, int rollNumber) {

        if (piece.boardPos + rollNumber >= (GameVars.spacesPerPlayer * GameVars.numPlayers) + piece.playerID * GameVars.spacesPerPlayer) {
            
            piece.boardPos += rollNumber;
            piece.completeHome = true;
            print("piece in home");

            return;
        }

        if (piece.atHome) {
            for (int i = 0; i < GameVars.numPlayers; i++) {
                if (i != piece.playerID) {
                    Piece blockingPiece = playerPieces[i].Find(p => p.boardPos % (GameVars.spacesPerPlayer * GameVars.numPlayers) == (piece.boardPos) % (GameVars.spacesPerPlayer * GameVars.numPlayers));
                    if (blockingPiece != null) {
                        print("sending piece home");
                        blockingPiece.boardPos = blockingPiece.playerID * GameVars.spacesPerPlayer;
                        blockingPiece.atHome = true;
                        blockingPiece.SetZ(0);
                        blockingPiece.powerUsed = false;
                        blockingPiece.UseSendHomePower(piece);

                        //StartCoroutine(blockingPiece.MoveWithBoard(piece.boardPos + rollNumber)); 
                    }
                }
            }

            piece.atHome = false;
            print("piece out of home");
            piece.SetZ(-1);

        } else {
            for (int i = 0; i < GameVars.numPlayers; i++) {
                if (i != piece.playerID) {
                    Piece blockingPiece = playerPieces[i].Find(p => p.boardPos % (GameVars.spacesPerPlayer * GameVars.numPlayers) == (piece.boardPos + rollNumber) % (GameVars.spacesPerPlayer * GameVars.numPlayers) && !p.atHome && !p.completeHome);
                    if(blockingPiece != null) {
                        print("sending piece home");
                        blockingPiece.boardPos = blockingPiece.playerID * GameVars.spacesPerPlayer;
                        blockingPiece.atHome = true;
                        blockingPiece.SetZ(0);
                        blockingPiece.powerUsed = false;
                        blockingPiece.UseSendHomePower(piece);

                       //StartCoroutine(blockingPiece.MoveWithBoard(piece.boardPos + rollNumber));  
                    }
                }
            }

            print("moving piece");
            piece.boardPos += rollNumber;

        }
        

        currentState = GameState.OnMoving;

    }

    IEnumerator MovePieces(List<Piece> pieces, int center, bool inSequence) {

        foreach (Piece p in pieces) {
            StartCoroutine(p.MoveWithBoard(center));
        }

        List<Piece> moving = pieces.FindAll(p => p.isMoving);
        while (moving.Count > 0) {
            moving = pieces.FindAll(p => p.isMoving);

            yield return null;
        }

        print("done moving all pieces");

        if (!inSequence) currentRoutine = null;
    }

    IEnumerator MovePieceSequence(Piece piece) {
        yield return StartCoroutine(piece.MoveUp());
        print("moved up");

        List<Piece> allButPieces = new List<Piece>();
        playerPieces.ForEach(p => allButPieces.AddRange(p));
        allButPieces.Remove(piece);

        yield return StartCoroutine(MovePieces(allButPieces,piece.boardPos,true));
        print("moved other pieces");

        yield return StartCoroutine(piece.MoveDown());
        print("moved down");
    }

    IEnumerator NewTurnRollSequence() {
        yield return StartCoroutine(MovePieces(allPieces, currentPieces[0].boardPos,true));
        print("centered on player");
        yield return StartCoroutine(gameDice.Roll());
        print("dice rolled");
    }


    void StartGame() {
        
        playerPieces = new List<List<Piece>>(GameVars.numPlayers);

        for (int i = 0; i < GameVars.numPlayers; i ++) {
            
            List<Piece> pieces = new List<Piece>(GameVars.piecePerPlayer);

            
            for (int j=0; j<GameVars.piecePerPlayer; j++) {
                GameObject newPieceObject = Instantiate(pieceObject, Vector3.zero,Quaternion.identity);
                Piece newPiece = newPieceObject.GetComponent<Piece>();
                newPiece.playerID = i;
                newPiece.onMoveOver += MoveComplete;
                newPiece.OnStartGame();


                pieces.Add(newPiece);
            }


            playerPieces.Add(pieces);

        }
        allPieces = new List<Piece>();
        playerPieces.ForEach(p => allPieces.AddRange(p));


        turn = 0;
        currentState = GameState.OnRolling;

    }

    void PrintGame() {
        print(currentRoutine);
        print("Turn " + turn);
        foreach(List<Piece> player in playerPieces) {
            print("Player " + player[0].playerID);
            foreach(Piece p in player) {
                print("Pos : " + p.boardPos + " Home: " + p.atHome + " Can Move: " + p.canMove);
            }
        }
    }
}
