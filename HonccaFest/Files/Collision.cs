// Collision.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using HonccaFest.MainClasses;
using HonccaFest.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.Files
{
	public class Collision
	{
		public static bool IsColliding(GameObject objOne, GameObject objTwo)
		{
			if (!objOne.Active || !objTwo.Active)
				return false;

			return objOne.GetRectangle().Intersects(objTwo.GetRectangle());
		}

		public static bool TilesHasCollision(Tile[] tiles)
		{
			for (int currentTileIndex = 0; currentTileIndex < tiles.Length; currentTileIndex++)
			{
				Tile currentTile = tiles[currentTileIndex];

				if (currentTile.TileType == Tile.Type.COLLISION)
					return true;
			}

			return false;
		}
	}
}
