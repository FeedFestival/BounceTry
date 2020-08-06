using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DataModel
{

}

public class GameData
{
    public int Id { get; set; }

    public int HighScore { get; set; }

    public string Date { get; set; }

    public GameData()
    {

    }

    public GameData(int score)
    {
        HighScore = score;
    }
}

public class GoogleGpsData
{
    public List<GpsResult> results { get; set; }
}

public class GpsResult
{
    public List<AddressComponent> address_components { get; set; }
    public List<string> types { get; set; }
}

public class AddressComponent
{
    public string long_name { get; set; }
    public string short_name { get; set; }
    public List<string> types { get; set; }
}