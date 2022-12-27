using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject playerPanel;
    public GameObject playerPiece;
    int selectionX = 0;
    int selectionY = 0;

    List<List<Piece.ColorClass>> playerPieces;

    int numPlayers = 2;
    // Start is called before the first frame update
    void Start()
    {
        playerPieces = new List<List<Piece.ColorClass>>();
        
        for (int i = 0; i < numPlayers; i++) {
            List<Piece.ColorClass> pieces = new List<Piece.ColorClass>(GameVars.piecePerPlayer) {
                Piece.ColorClass.Red,
                Piece.ColorClass.Yellow,
                Piece.ColorClass.Green,
                Piece.ColorClass.Blue
            };

            playerPieces.Add(pieces);
        }

        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (selectionX <= numPlayers) {
                selectionX++;
            }

        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (selectionX > 0) {
                selectionX--;
            }

        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (selectionY <= GameVars.piecePerPlayer) {
                selectionY++;
            }

        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (selectionY > 0) {
                selectionY--;
            }

        } else if (Input.GetKeyDown(KeyCode.C)) {

        } else if (Input.GetKeyDown(KeyCode.X)) {

        }
        
    }
}
