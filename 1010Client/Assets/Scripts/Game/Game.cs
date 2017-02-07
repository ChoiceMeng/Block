using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
	[HideInInspector]
	public List<Block>
		listBlockQueue;
	public GameObject blockModel;
	public Transform parentsBlockQueue;
    public GameObject bl = null;
    public GameObject tr = null;
    public GameObject bl2 = null;
	private Vector3 botleftBoardPos = new Vector3 (-4.15f, -4.076f, 0f);
    private Vector3 botleftBoardPos2 = new Vector3(-4.15f, -4.076f, 0f);
    private Vector3 toprightBoardPos = new Vector3 (4.221f, 4.013f, 0f);
	public int[,] board = new int[10, 8];
	public tk2dSprite[,] spriteBoardDown = new tk2dSprite[10, 8];
    public tk2dSprite[,] spriteBoardUp = new tk2dSprite[10, 8];
    public Transform parentsBoard;
	public GameObject cellMode;

    public GameObject netFlag;
	// Use this for initialization
	void Start ()
	{
        botleftBoardPos = bl.transform.position;
        toprightBoardPos = tr.transform.position;
        botleftBoardPos2 = bl2.transform.position;

        InitSpriteBoard();
        InitSpriteBoard(false);
        ResetBoard();
        createNewBlockQueue();

        NetTest();
	}

    void NetTest()
    {
        NetWriter.SetUrl("127.0.0.1:9001");
        Net.Instance.Send((int)ActionType.CreateRoom, OnRankingCallback, null);
    }

    void OnRankingCallback(ActionResult actionResult)
    {
        if (netFlag != null)
            netFlag.SetActive(true);
    }

    // Update is called once per frame
    void Update ()
	{
	
	}

	public void createNewBlockQueue ()
	{
		for (int i =0; i<3; i++) {
			GameObject go = Instantiate (blockModel) as GameObject;
			go.transform.parent = parentsBlockQueue;
			Block blockScript = go.GetComponent<Block> ();
			if (blockScript != null) {
				int type = Random.Range (1, 11);

				blockScript.mainGameScript = this;
				blockScript.setType (type, i);
				listBlockQueue.Add (blockScript);
			}

		}
	}

	public Block currSelectionBlock;

	public void OnTouchDown ()
	{
        Debug.Log("OnTouchDown");
        if (listBlockQueue.Count > 0 && currSelectionBlock == null) {
			Vector3 pointClick = GetTouchPos ();
			Debug.Log ("pointClick: " + pointClick);
			for (int i =0; i<listBlockQueue.Count; i++) {
				if (listBlockQueue [i].CheckPointIn (pointClick)) {
					currSelectionBlock = listBlockQueue [i];
					currSelectionBlock.SetSelection (GetTouchPos ());

					StartCoroutine (UpdateBlockSelection (currSelectionBlock));
					break;
				}
			}
		}
	}

	IEnumerator UpdateBlockSelection (Block block)
	{
		yield return new WaitForSeconds (0.05f);
		while (currSelectionBlock!=null) {
			block.SetSelectionPos (GetTouchPos ());
			yield return null;
		}
	}

	public void OnTouchUp ()
	{
        Debug.Log("OnTouchUp");
		if (currSelectionBlock != null) {
			StopCoroutine ("UpdateBlockSelection");
			currSelectionBlock.SetSelectionPos (GetTouchPos ());
			Vector3 pos = currSelectionBlock.transform.position;
			Debug.Log ("release: " + pos.ToString ());
			if ((pos.x + Config.CELL_SIZE / 2 < toprightBoardPos.x) && (pos.x + Config.CELL_SIZE / 2 > botleftBoardPos.x) && (pos.y + Config.CELL_SIZE / 2 < toprightBoardPos.y) && (pos.y + Config.CELL_SIZE / 2 > botleftBoardPos.y)) {

				Debug.Log ("" + ((pos.x - botleftBoardPos.x + Config.CELL_SIZE / 2) / Config.CELL_SIZE));
				int xTag = (int)((pos.x - botleftBoardPos.x + Config.CELL_SIZE / 2) / Config.CELL_SIZE);
				int yTag = (int)((pos.y - botleftBoardPos.y + Config.CELL_SIZE / 2) / Config.CELL_SIZE);
				if (xTag > board.GetLength(0))
					xTag = board.GetLength(0);
				if (yTag > board.GetLength(1))
					yTag = board.GetLength(1);
				Debug.Log ("pos Tag: " + xTag + ", " + yTag);
				bool isTag = true;
				for (int i = xTag; i<xTag+currSelectionBlock.w; i++) {
					for (int j =yTag; j<yTag+currSelectionBlock.h; j++) {
						if (currSelectionBlock.array [i - xTag, j - yTag] != 0 && board [i, j] != -1) {
							isTag = false;
						}
					}
				}

				if (!isTag) {
					currSelectionBlock.SetUnSelection ();
					currSelectionBlock = null;
				} else {
					Vector3 pos1 = new Vector3 (botleftBoardPos.x + Config.CELL_SIZE * (xTag + 0.5f), botleftBoardPos.y + Config.CELL_SIZE * (yTag + 0.5f), currSelectionBlock.transform.position.z);
					currSelectionBlock.TagToPos (pos1);
					currSelectionBlock.xTag = xTag;
					currSelectionBlock.yTag = yTag;
				}
			} else {
				currSelectionBlock.SetUnSelection ();
				currSelectionBlock = null;
			}



		}
	}

	public void FinishTagPos ()
	{
		if (currSelectionBlock != null) {
			int xTag = currSelectionBlock.xTag;
			int yTag = currSelectionBlock.yTag;

			for (int i = xTag; i<xTag+currSelectionBlock.w; i++) {
				for (int j =yTag; j<yTag+currSelectionBlock.h; j++) {
					spriteBoardDown [i, j].SetSprite ("1_" + currSelectionBlock.type);
					spriteBoardDown [i, j].gameObject.SetActive (true);
					board [i, j] = currSelectionBlock.type;
				}
			}

            SimNetPlayer(xTag, yTag, currSelectionBlock.w, currSelectionBlock.h, currSelectionBlock.type);
        }

		for (int i =0; i<currSelectionBlock.listSprite.Length; i++) {
			if (currSelectionBlock.listSprite [i] != null) {
				Destroy (currSelectionBlock.listSprite [i].gameObject);
			}
		}
		Destroy (currSelectionBlock.gameObject);
		listBlockQueue.Remove (currSelectionBlock);
		currSelectionBlock = null;
		List<int> listRow = new List<int> ();
		List<int> listCol = new List<int> ();

		for (int i =0; i< board.GetLength(0); i++) {
			bool addCol = true;
			for (int j =0; j< board.GetLength(1); j++) {
				if (board [i, j] == -1) {
					addCol = false;
				}
			}

			if (addCol) {
				for (int j =0; j< board.GetLength(0); j++) {
					board [i, j] = -1;

				}
				listCol.Add (i);
			}
		}

		for (int j =0; j< board.GetLength(1); j++) {
			bool addRow = true;
			for (int i =0; i< board.GetLength(0); i++) {
				if (board [i, j] == -1) {
					addRow = false;
				}
			}

			if (addRow) {
				for (int i =0; i< board.GetLength(0); i++) {
					board [i, j] = -1;

				}
				listRow.Add (j);
			}
		}
		StartCoroutine (DestroyRowCol (listRow, listCol));
	}

	IEnumerator DestroyRowCol (List<int> listRow, List<int> listCol)
	{
		List<tk2dSprite> listAct = new List<tk2dSprite> ();

		for (int j =0; j<listRow.Count; j++) {
			for (int i =0; i< board.GetLength(0); i++) {
				if (!listAct.Contains (spriteBoardDown [i, listRow [j]])) {
					listAct.Add (spriteBoardDown [i, listRow [j]]);
				}
			}
		}

		for (int i =0; i<listCol.Count; i++) {
			for (int j =0; j< board.GetLength(1); j++) {
				if (!listAct.Contains (spriteBoardDown [listCol [i], j])) {
					listAct.Add (spriteBoardDown [listCol [i], j]);
				}
			}
		}

		float p = 0;
		float timeRun = 0.5f;
		if (listAct.Count > 0) {
			Vector3 scale = Vector3.one;
			while (p<=1) {
				p += Time.deltaTime / timeRun;
				scale.x = 1 - p;
				scale.y = 1 - p;
				for (int i =0; i<listAct.Count; i++) {

					listAct [i].transform.localScale = scale;
				}
				yield return null;
			}
			for (int i =0; i<listAct.Count; i++) {
				
				listAct [i].transform.localScale = Vector3.one;
				listAct [i].SetSprite ("square");
				listAct [i].gameObject.SetActive (false);
			}
	
		}
		ReCreateNewListBlockQueue ();
	}

	public void ReCreateNewListBlockQueue ()
	{
		if (listBlockQueue.Count <= 0) {
			createNewBlockQueue ();
			StartCoroutine (MoveBlockQueueToScreen ());
		} else {
			CheckEndGame ();
		}

	}

	public void CheckEndGame ()
	{
		bool endGame = true;
		foreach (Block block in listBlockQueue) {

			for (int xTag =0; xTag< board.GetLength(1); xTag++) {
				for (int yTag = 0; yTag< board.GetLength(0); yTag++) {
		
					Debug.Log ("pos Tag: " + xTag + ", " + yTag);
					bool isContinue = true;
					for (int i = xTag; i<xTag+block.w; i++) {
						for (int j =yTag; j<yTag+block.h; j++) {
                            if (i - xTag >= block.array.GetLength(0) || j - yTag >= block.array.GetLength(1) || i >= board.GetLength(0) || j >= board.GetLength(1))
                                continue;

							if (block.array [i - xTag, j - yTag] != 0 && board [i, j] != -1) {
                                isContinue = false;
							}
						}
					}
				
					if (isContinue) {
						endGame = false;
					}
				
				}
			}
		}
		if (endGame) {
			Application.LoadLevel (Application.loadedLevel);
		}
	}

	IEnumerator MoveBlockQueueToScreen ()
	{
		Vector3 destPos = parentsBlockQueue.transform.position;
		Vector3 srcPos = destPos + new Vector3 (4.8f, 0, 0);

		float p = 0;
		float timeMove = 0.5f;
		while (p<=1) {
			p += Time.deltaTime / timeMove;
			parentsBlockQueue.transform.position = Vector3.Lerp (srcPos, destPos, p);
			yield return null;
		}
		CheckEndGame ();
	}

	public Vector3 GetTouchPos ()
	{
		Vector3 pointClick = Vector3.zero;
		{
			if (Application.isMobilePlatform) {
				pointClick = Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position);
			} else {
				pointClick = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				
			}
		}
		return pointClick;
	}

	public void InitSpriteBoard(bool bDown = true)
	{
        tk2dSprite[,] spriteBoard = null;
        Vector3 botleftPos = Vector3.zero;
        if (bDown)
        { 
            spriteBoard = spriteBoardDown;
            botleftPos = botleftBoardPos;
        }
        else
        {
            spriteBoard = spriteBoardUp;
            botleftPos = botleftBoardPos2;
        }

        for (int i =0; i< spriteBoard.GetLength(0); i++) {
			for (int j =0; j< spriteBoard.GetLength(1); j++) {
				if (spriteBoard[i, j] == null) {
					GameObject go = Instantiate (cellMode) as GameObject;
					go.name = "nen_cot: " + i + ",dong: " + j;
					go.transform.parent = parentsBoard;
					go.transform.position = new Vector3 (botleftPos.x + Config.CELL_SIZE * (i + 0.5f), botleftPos.y + Config.CELL_SIZE * (j + 0.5f), 2);

					tk2dSprite sprite = go.GetComponent<tk2dSprite> ();
					if (sprite != null) {

						sprite.SetSprite ("square");
					}
				}
			}
		}

		for (int i =0; i< spriteBoard.GetLength(0); i++) {
			for (int j =0; j< spriteBoard.GetLength(1); j++) {

				GameObject go = Instantiate (cellMode) as GameObject;
				go.name = "o_cot: " + i + ",dong: " + j;
				go.transform.parent = parentsBoard;
				go.transform.position = new Vector3 (botleftPos.x + Config.CELL_SIZE * (i + 0.5f), botleftPos.y + Config.CELL_SIZE * (j + 0.5f), 1);
					
				tk2dSprite sprite = go.GetComponent<tk2dSprite> ();
				if (sprite != null) {
                    spriteBoard[i, j] = sprite;
					sprite.SetSprite ("square");
				}
				go.SetActive (false);
			}

		}
	}

	public void ResetBoard (bool bDown = true)
	{
		// clear board

		// new board;

		for (int i =0; i<board.GetLength(0); i++) {
			for (int j =0; j<board.GetLength(1); j++) {
				board [i, j] = -1;
				spriteBoardDown [i, j].SetSprite ("square");
                spriteBoardUp [i, j].SetSprite("square");
            }
		}
	}

    // simulator net message
    public void SimNetPlayer(int xPos, int yPos, int nWidth, int nHeight, int nType)
    {
        int xTag = xPos;
        int yTag = yPos;

        for (int i = xTag; i < xTag + nWidth; i++)
        {
            for (int j = yTag; j < yTag + nHeight; j++)
            {
                spriteBoardUp[i, j].SetSprite("1_" + nType);
                spriteBoardUp[i, j].gameObject.SetActive(true);
            }
        }
    }
}

