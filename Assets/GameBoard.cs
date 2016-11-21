using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour {

	public int[,] boardData;

	public IntVector2 dimensions;

	// Use this for initialization
	public void Start () {
		boardData = new int[dimensions.x, dimensions.y];
	}
	
	// Update is called once per frame
	public void Update () {
	
	}


}
