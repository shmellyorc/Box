using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Box.Helpers
{
	public static class Box
	{
		public static uint Hash(string hash) => HashHelpers.Hash32(hash);
		public static uint Hash(Enum hash) => HashHelpers.Hash32(hash.ToEnumString());

		public static Surface GetSurf(string name) => Assets.GetSurface(name);
		public static Surface GetSurf(Enum name) => Assets.GetSurface(name);

		

	}
}
