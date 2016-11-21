using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameBoardGenerator {
	
	public GameBoard gameBoard;
	IntVector2 cachedGraphRoot;

	public GameBoardGenerator(GameBoard gameBoard) {
		this.gameBoard = gameBoard;
		cachedGraphRoot = new IntVector2 ();
	}
	public void GenerateDataModel () {

	}

	public void GenerateWall(int length) {
		IntVector2 cur = ChooseRandomSquare ();
		if (cur.x < 0) {
			return;
		}
		Build (cur, length);
	}

	public bool Build(IntVector2 parent, int depth) {
		if (depth <= 0) {
			return false; // should never happen
		}
		if (GetData (parent) > 0) {
			return false; // should never happen
		}
		SetData (parent, 1);
		if (!IsGraphConnected ()) {
			SetData (parent, 0);
			return false;
		}
		if (depth == 1) {
			return true; // base case; we made it to the end
		}
		List<IntVector2> neighbors = GetNeighborsRandomOrder (parent);
		for (int i = 0; i < neighbors.Count; i++) {
			// recur
			if (Build (neighbors [i], depth - 1)) {
				return true;
			}
		}
		SetData (parent, 0);
		return false; // no neighbor succeeded
	}

	public bool IsGraphConnected () {
		PickRoot ();
		if (!IsGraphRootOpen ()) {
			return true;
		}
		MarkConnected ();
		bool connected = VerifyMarks ();
		UnMark ();
		return connected;
	}

	public bool IsGraphRootOpen() {
		return gameBoard.boardData [cachedGraphRoot.x, cachedGraphRoot.y] == 0;
	}

	public List<IntVector2> GetNeighbors(IntVector2 square) {
		List<IntVector2> ret = new List<IntVector2> ();
		for (int i = square.x - 1; i <= square.x + 1; i++) {
			for (int j = square.y - 1; j <= square.y + 1; j++) {
				try {
					if (GetData(i, j) == 0) {
						ret.Add (new IntVector2(i, j));
					}
				} catch (System.IndexOutOfRangeException ignored) {}
			}
		}
		return ret;
	}

	public List<IntVector2> GetNeighborsRandomOrder(IntVector2 square) {
		List<IntVector2> neighbors = GetNeighbors (square);
		for (int i = 0; i < neighbors.Count; i++) {
			IntVector2 temp = neighbors[i];
			int randomIndex = Random.Range(i, neighbors.Count);
			neighbors[i] = neighbors[randomIndex];
			neighbors[randomIndex] = temp;
		}
		return neighbors;
	}

	public void PickRoot() {
		if (!IsGraphRootOpen ()) {
			for (int i = 0; i < gameBoard.boardData.GetLength (0); i++) {
				bool doBreak = false;
				for (int j = 0; j < gameBoard.boardData.GetLength (1); j++) {
					if (GetData(i, j) == 0) {
						cachedGraphRoot.x = i;
						cachedGraphRoot.y = j;
						doBreak = true;
						break;
					}
				}
				if (doBreak) {
					break;
				}
			}
		}
	}

	public void MarkConnected() {
		// Check connectivity
		Queue queue = new Queue();
		gameBoard.boardData [cachedGraphRoot.x, cachedGraphRoot.y] = -1;
		queue.Enqueue (cachedGraphRoot);
		while (queue.Count > 0) {
			IntVector2 cur = (IntVector2) queue.Dequeue ();
			List<IntVector2> neighbors = GetNeighbors (cur);
			neighbors.ForEach (delegate(IntVector2 obj) {
				SetData (obj, -1);
				queue.Enqueue (obj);
			});
		}
	}

	public bool VerifyMarks() {
		for (int i = 0; i < gameBoard.boardData.GetLength (0); i++) {
			for (int j = 0; j < gameBoard.boardData.GetLength (1); j++) {
				if (GetData(i, j) == 0) {
					return false;
				}
			}
		}
		return true;
	}

	public void UnMark() {
		for (int i = 0; i < gameBoard.boardData.GetLength (0); i++) {
			for (int j = 0; j < gameBoard.boardData.GetLength (1); j++) {
				if (GetData(i, j) == -1) {
					SetData(i, j,  0);
				}
			}
		}
	}

	public List<IntVector2> GetOpenSquares() {
		List<IntVector2> openSquares = new List<IntVector2> ();
		for (int i = 0; i < gameBoard.boardData.GetLength (0); i++) {
			for (int j = 0; j < gameBoard.boardData.GetLength (1); j++) {
				if (GetData(i, j) == 0) {
					openSquares.Add (new IntVector2 (i, j));
				}
			}
		}
		return openSquares;
	}

	public IntVector2 ChooseRandomSquare() {
		List<IntVector2> openSquares = GetOpenSquares ();
		if (openSquares.Count == 0) {
			return new IntVector2(-1, -1);
		}
		return openSquares [Random.Range (0, openSquares.Count - 1)];
	}

	public int CountOpenSquares() {
		List<IntVector2> openSquares = GetOpenSquares ();
		return openSquares.Count;
	}

	public int GetData(IntVector2 square) {
		return gameBoard.boardData [square.x, square.y];
	}

	public int GetData(int x, int y) {
		return gameBoard.boardData [x, y];
	}

	public void SetData(IntVector2 square, int value) {
		gameBoard.boardData [square.x, square.y] = value;
	}

	public void SetData(int x, int y, int value) {
		gameBoard.boardData [x, y] = value;
	}
}
