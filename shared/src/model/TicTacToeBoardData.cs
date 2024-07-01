using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace shared
{
	/**
	 * Simple board model for Quoridor.
	 */
	public class TicTacToeBoardData : ASerializable
	{
		//board representation in 1d array, one element for each cell
		//0 is empty, 1 is player 1, 2 is player 2

		public int[] board = new int[81];
		public int[] verticalWalls = new int[64];
		public int[] horizontalWalls = new int[64];


		 int winner = -1;

		public int WhoHasWon()
		{
			return winner;							
		}
		
		public override void Serialize(Packet pPacket)
		{
			for (int i = 0; i < board.Length; i++) pPacket.Write(board[i]);
            for (int i = 0; i < verticalWalls.Length; i++) pPacket.Write(verticalWalls[i]);
            for (int i = 0; i < horizontalWalls.Length; i++) pPacket.Write(horizontalWalls[i]);

        }

		public override void Deserialize(Packet pPacket)
		{
			for (int i = 0; i < board.Length; i++) board[i] = pPacket.ReadInt(); 
			for (int i = 0; i < verticalWalls.Length; i++) verticalWalls[i] = pPacket.ReadInt();
            for (int i = 0; i < horizontalWalls.Length; i++) horizontalWalls[i] = pPacket.ReadInt();
        }

		public override string ToString()
		{
			return GetType().Name +":"+ string.Join(",", board);
		}

		public void SetWinner(int pWinner)
		{
			winner = pWinner;

        }
	}
}

