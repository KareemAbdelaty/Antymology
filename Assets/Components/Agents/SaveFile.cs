using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace Antymology.Terrain
{
[Serializable]
public class SaveFile
{ public int health_share_with_queen_chance;
	public int queen_health_share_chance;
	public int health_share_chance;
	public int eatChance;
	public int digChance;
	public int NestSize;
	public int createNestChance;

	public static void Save(int health_share_with_queen_chance, int queen_health_share_chance, int eatChance, int digChance, int NestSize, int createNestChance,int health_share_chance)
	{
			if (File.Exists("Best.data"))
			{
				File.Delete("Best.data");
			}
			BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create("Best.data");
		SaveFile data = new SaveFile();
		data.health_share_with_queen_chance = health_share_with_queen_chance;
		data.queen_health_share_chance = queen_health_share_chance;
		data.eatChance = eatChance;
		data.digChance = digChance;
		data.NestSize = NestSize;
		data.createNestChance = createNestChance;
		data.health_share_chance = health_share_chance;
		bf.Serialize(file, data);
		file.Close();
	}
	public static SaveFile load()
    {
			if (File.Exists(Application.persistentDataPath
			   + "/Best.data"))
			{
				BinaryFormatter bf = new BinaryFormatter();
				FileStream file =
						   File.Open( "Best.data", FileMode.Open);
				SaveFile data = (SaveFile)bf.Deserialize(file);
				file.Close();
				Debug.Log("Game data loaded!");
				return data;
			}
			else
				return null;
		}


}
}