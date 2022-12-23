public class TanSat : Auto
{
	public int templateId;

	public int mobId;

	public override void Run()
	{
		Char.getMyChar();
		if (Auto.isDie(Char.getMyChar()))
		{
			Hoisinh(isWait: true);
			return;
		}
		if (TileMap.mapID != mapID || TileMap.zoneID != zoneID)
		{
			Go(mapID, zoneID);
			return;
		}
		Default();
		PickDa();
		Attack(templateId, mobId);
	}

	public override string ToString()
	{
		if (templateId == -1)
		{
			return "Tàn sát: all";
		}
		return "Tàn sát: " + Mob.MobName(templateId);
	}

	public void update(int temlpateId, int mobId = -1)
	{
		Update();
		zoneID = TileMap.zoneID;
		mapID = TileMap.mapID;
		templateId = temlpateId;
		this.mobId = mobId;
	}
}
