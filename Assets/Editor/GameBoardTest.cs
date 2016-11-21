using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;

public class GameBoardTest {

	GameBoard gb;
	GameBoardGenerator gen;

	[SetUp]
	public void SetUp() {
		gb = new GameBoard ();
		gb.dimensions = new IntVector2 (10, 20);
		gb.Start ();
		gen = new GameBoardGenerator (gb);
	}

	[Test]
	public void Construction() {
		Assert.AreEqual (10, gb.dimensions.x);
		Assert.AreEqual (20, gb.dimensions.y);
		Assert.AreEqual (10, gb.boardData.GetLength (0));
		Assert.AreEqual (20, gb.boardData.GetLength (1));
	}

	[Test]
	public void Start_Connected() {
		Assert.IsTrue (gen.IsGraphConnected ());
	}

	[Test]
	public void Start_RootOpen() {
		Assert.IsTrue (gen.IsGraphRootOpen ());
	}

	[Test]
	public void BlockRoot_RootClosed() {
		gb.boardData [0, 0] = 1;
		Assert.IsFalse (gen.IsGraphRootOpen ());
	}

	[Test]
	public void BlockRoot_IsConnected() {
		gb.boardData [0, 0] = 1;
		Assert.IsTrue (gen.IsGraphConnected ());
	}

	[Test]
	public void BlockAll_IsConnected() {
		gb.boardData = new int[1, 1];
		gb.boardData [0, 0] = 1;
		Assert.IsTrue (gen.IsGraphConnected ());
	}

	[Test]
	public void BlockLine_IsNotConnected() {
		gb.boardData = new int[3, 3];
		gb.boardData [1, 0] = 1;
		gb.boardData [1, 1] = 1;
		gb.boardData [1, 2] = 1;
		Assert.IsFalse (gen.IsGraphConnected ());
	}

	[Test]
	public void Empty_MarkConnectedMarksAll() {
		for (int scale = 1; scale < 20; scale++) {
			gb.dimensions = new IntVector2 (scale, scale);
			gb.Start ();
			gen.MarkConnected ();
			Assert.IsTrue (gen.VerifyMarks (), "Fail on scale " + scale);
		}
	}

	[Test]
	public void GenerateWall() {
		int initialCount = gen.CountOpenSquares ();
		int wallSize = 7;
		gen.GenerateWall (wallSize);
		Assert.AreEqual (initialCount - wallSize, gen.CountOpenSquares ());
		Assert.IsTrue (gen.IsGraphConnected ());
	}

	[Test]
	public void GenerateManyWalls() {
		for (int i = 0; i < 10; i++) {
			int initialCount = gen.CountOpenSquares ();
			int wallSize = i;
			gen.GenerateWall (wallSize);
			Assert.AreEqual (initialCount - wallSize, gen.CountOpenSquares ());
			Assert.IsTrue (gen.IsGraphConnected ());
		}
	}

	[Test]
	public void BuildFull_Builds() {
		gb.dimensions = new IntVector2 (2, 2);
		gb.Start ();
		gen.Build (new IntVector2(0, 0), 4);
		Assert.AreEqual (0, gen.CountOpenSquares ());
	}

	[Test]
	public void BuildAlmostFull_Builds() {
		gb.dimensions = new IntVector2 (2, 3);
		gb.Start ();
		gen.Build (new IntVector2(0, 0), 5);
		Assert.AreEqual (1, gen.CountOpenSquares ());
	}

	[Test]
	public void BuildOne_Builds() {
		gb.dimensions = new IntVector2 (2, 2);
		gb.Start ();
		gen.Build (new IntVector2(0, 0), 1);
		Assert.AreEqual (3, gen.CountOpenSquares ());
		Assert.AreEqual (1, gb.boardData [0, 0]);
	}

	[Test]
	public void BuildTwo_Builds() {
		gb.dimensions = new IntVector2 (2, 2);
		gb.Start ();
		gen.Build (new IntVector2(0, 0), 2);
		Assert.AreEqual (2, gen.CountOpenSquares ());
		Assert.AreEqual (1, gb.boardData [0, 0]);
	}

	[Test]
	public void BuildThree_Builds() {
		gb.dimensions = new IntVector2 (2, 2);
		gb.Start ();
		gen.Build (new IntVector2(0, 0), 3);
		Assert.AreEqual (1, gen.CountOpenSquares ());
		Assert.AreEqual (1, gb.boardData [0, 0]);
	}

	[Test]
	public void BuildTooMuch_DoesntBuild() {
		gb.dimensions = new IntVector2 (2, 2);
		gb.Start ();
		gen.Build (new IntVector2(0, 0), 5);
		Assert.AreEqual (4, gen.CountOpenSquares ());
	}


}
