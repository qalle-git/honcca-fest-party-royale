using HonccaFest.MainClasses;
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
	}
}
