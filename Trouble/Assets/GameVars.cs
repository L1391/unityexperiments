using UnityEngine;
using System.Collections.Generic;
public class GameVars
{
    public static int numPlayers = 2;

    public static int piecePerPlayer = 4;

    public static int spacesPerPlayer = 20;

    public static int unitsPerSpace = 2;

    public static List<List<Piece.ColorClass>> playerPieces;

    public static bool isPermaDeath = false;

    
    public static int mod(int x, int m) {
        int r = x%m;
        return r<0 ? r+m : r;
    }

    public static bool eq(Vector2 a, Vector2 b) {
        return (a-b).magnitude < 0.001f;
    }
}
