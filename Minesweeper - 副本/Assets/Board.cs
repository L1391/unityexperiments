using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width = 10;
    public int height = 10;

    public int numMines = 10;

    public int colorRandomness = 50;

    public GameObject tile;

    GameObject[,] tiles;
    List<GameObject> mineTiles;
    List<Tile> searchedTiles;

    List<Tile> redTiles;


    List<Color> numColors = new List<Color>() {
        new Color(242/255f,197/255f,124/255f,1),
        new Color(221/255f,174/255f,126/255f,1),
        new Color(127/255f,182/255f,133/255f,1),
        new Color(66/255f,106/255f,90/255f,1),
        new Color(202/255f,174/255f,128/255f,1),
        new Color(185/255f,155/255f,127/255f,1),
        new Color(124/255f,156/255f,128/255f,1),
        new Color(69/255f,87/255f,80/255f,1),
    };

    // Start is called before the first frame update
    void Start()
    {
        LoadBoard();      
    }

    public void LoadBoard() {
        foreach( Tile oldTiles in FindObjectsOfType<Tile>()) {
            Destroy(oldTiles.gameObject);
        }

        redTiles = new List<Tile>(width * height);
        searchedTiles = new List<Tile>(width * height - numMines);

        numColors.Sort( (color1,color2) =>  Random.Range(-colorRandomness,colorRandomness + 1));

        float screenHeight = Camera.main.orthographicSize * 2.0f;
        float screenWidth = screenHeight * Screen.width / Screen.height;
        float tileHeight =  screenHeight/height;
        float tileWidth = screenWidth/width;

        tileHeight = tileWidth = Mathf.Min(tileHeight,tileWidth);

        Camera.main.transform.position = new Vector3(screenWidth/2,screenHeight/2,-10);

        tiles = new GameObject[width,height];
        mineTiles = new List<GameObject>(numMines);

        for (int row = 0; row < width; row++) {
            for (int col = 0; col < height; col++) {
                
                GameObject newTile = Instantiate(tile,new Vector2((screenWidth - width*tileWidth)/2f + tileWidth/2f + row*tileWidth, (screenHeight - height*tileHeight)/2f + tileHeight/2f +  col*tileHeight), new Quaternion(0,0,0,0),transform);
                newTile.transform.localScale = new Vector2(tileWidth,tileHeight);
                tiles[row,col] = newTile;
                
                Tile newTileCode = newTile.GetComponent<Tile>();
                newTileCode.x = row;
                newTileCode.y = col;
                newTileCode.onReveal += RevealAdj;
                newTileCode.onGameOver += GameOver;
            }
        }

        for (int i = 0; i < numMines; i++) {
            int randX = Random.Range(0,width);
            int randY = Random.Range(0,height);

            while(mineTiles.Contains(tiles[randX,randY])) {
                randX = Random.Range(0,width);
                randY = Random.Range(0,height);
            }

            tiles[randX,randY].GetComponent<Tile>().isMine = true;
            mineTiles.Add(tiles[randX,randY]);

            for (int xAdj = -1; xAdj <= 1; xAdj++) {
                for (int yAdj = -1; yAdj <= 1; yAdj++) {
                    if (randX + xAdj >= 0 && randX + xAdj < width && randY + yAdj >= 0 && randY + yAdj < height) {
                        Tile adjTile = tiles[randX + xAdj,randY +yAdj].GetComponent<Tile>();
                        adjTile.mineNumber++;
                        adjTile.revealColor = numColors[adjTile.mineNumber - 1];

                    }
                }
            }
        }
    }
    void RevealAdj(Tile tile) {
        StartCoroutine(RevealAdjCoroutine(tile));
    }

    IEnumerator RevealAdjCoroutine(Tile tile) {

        if (!searchedTiles.Contains(tile)) {
            searchedTiles.Add(tile);

            if (searchedTiles.Count + mineTiles.Count == width*height) {
                GameWin();
                yield break;
            }
        } else {
            yield break;
        }


        yield return new WaitForSeconds(0.1f);

        if (tile.mineNumber == 0 ) {
            for (int xAdj = -1; xAdj <= 1; xAdj++) {
                for (int yAdj = -1; yAdj <= 1; yAdj++) {
                    if (tile.x + xAdj >= 0 && tile.x + xAdj < width && tile.y + yAdj >= 0 && tile.y + yAdj < height) {
                        Tile adjTile = tiles[tile.x + xAdj,tile.y +yAdj].GetComponent<Tile>();
                        if (!searchedTiles.Contains(adjTile)) {
                            adjTile.Reveal();
                        }
                    }
                }
            }
        } 
    }



    void GameWin() {
        print("win");
        StartCoroutine(GameWinAnim());

    }

    IEnumerator GameWinAnim() {
        yield return StartCoroutine(BounceWave());
        yield return new WaitForSeconds(1);
        LoadBoard();
    }

    IEnumerator BounceWave() {
        foreach( Tile tiles in FindObjectsOfType<Tile>()) {
            StartCoroutine(tiles.Bounce());
            yield return new WaitForSeconds(0.01f);
        }
    }

    void GameOver(Tile tile) {
        print("lose");
        StartCoroutine(GameOverAnim(tile));
    }

    IEnumerator GameOverAnim(Tile tile) {
        redTiles.Clear();
        yield return StartCoroutine(RedSpread(tile));
    
        while(redTiles.Count != width*height) {
            yield return null;
        }

        yield return new WaitForSeconds(1);

        StopAllCoroutines();

        LoadBoard();
    }

    IEnumerator RedSpread(Tile tile) {

        if (!redTiles.Contains(tile)) {
            StartCoroutine(tile.Bounce());
            tile.GetComponent<SpriteRenderer>().color = Color.red;
            redTiles.Add(tile);

        } else {
            yield break;
        }

        yield return new WaitForSeconds(0.08f);

        for (int xAdj = -1; xAdj <= 1; xAdj++) {
            for (int yAdj = -1; yAdj <= 1; yAdj++) {
                if (tile.x + xAdj >= 0 && tile.x + xAdj < width && tile.y + yAdj >= 0 && tile.y + yAdj < height) {
                    //yield return new WaitForSeconds(0.01f);
                    Tile adjTile = tiles[tile.x + xAdj,tile.y +yAdj].GetComponent<Tile>();
                    if (!redTiles.Contains(adjTile)) {
                        StartCoroutine(RedSpread(adjTile));
                    }
                }
            }
         }
    
    }

}
