using Verse;

namespace Glass_Lights
{
	public interface IRoofMounted
	{
		void Poof();
	}

	public interface IWallMounted
	{
		void Poof();
	}

	public class Building_OnCeiling : Building, IRoofMounted
	{
		public void Poof()
		{
			this.Destroy(DestroyMode.Refund);
			//this.Kill(null, null);
		}
	}

	public class Building_WallMounted : Building, IWallMounted
	{
		public void Poof()
		{
			this.Destroy(DestroyMode.Refund);
			//if (!base.Position.Impassable(base.Map))
			//{
			//	this.Destroy(DestroyMode.Refund);
			//}
		}
	}

	public class PlaceWorker_LongWall : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			//loc is the empty space in front of the wall
			//c is the offset tile that is being checked as a wall

			//We want to check if the location that the player is placing the building is free and the second tile the building occupies is a wall. It is assumed the building is 1x2
			//If both of these conditions are met, we return true, elsewise false.

			AcceptanceReport result = false;
			IntVec3 c = loc + rot.FacingCell; //offset tile

			if(!loc.Impassable(map) && loc.InBounds(map))//Center tile is not impassible and in bounds
			{
				if (c.Impassable(map))//It is assumed that Impassable means a wall of some sort. This allows for placement on rocks and other larger buildings like geothermal generators if desired.
				{
					result = true;
				}
			}

			//bool flag = c.Impassable(map);
			//bool flag2 = !loc.Impassable(map) && loc.InBounds(map);
			//bool flag3 = !flag && !flag2;
			//if (!flag3)
			//{
			//	bool flag4 = !flag;
			//	if (!flag4)
			//	{
			//		bool flag5 = !flag2;
			//		if (!flag5)
			//		{
			//			result = true;
			//		}
			//	}
			//}
			return result;
		}
	}

	public class PlaceWorker_UnderRoof : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			AcceptanceReport result = new AcceptanceReport("Ceiling mounted buildings must be placed under a roof that is unobstructed by a wall.");
			result = map.roofGrid.Roofed(loc) && !loc.Impassable(map);
			return result;
		}
	}
}
