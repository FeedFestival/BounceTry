using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class GPS : MonoBehaviour
{
    public string playerPrefsKey = "Country";

    //      Asakusa.
    //      float latitude = 35.71477f;
    //      float longitude = 139.79256f;
    public float Latitude;
    public float Longitude;

    public string Country;
    public string CountryCode;
    public string City;
    public string Neighborhood;

    //
    private IEnumerator _googleGps;
    private IEnumerator _ipGps;
    private IEnumerator _queryGoogleGps;

    private string _useSensor = "sensor=true";
    private string _apiKey = "key=AIzaSyBG3NYRlE3Yw70F_fzmp0a7Lb0vghIpoWY";

    private GameObject _countryObject;
    private float _rotationAngle;

    void Start()
    {
        GetGeographicalCoordinates();
    }

    public void GetGeographicalCoordinates()
    {
        if (Input.location.isEnabledByUser)
        {
            _googleGps = GetGeographicalCoordinatesByMobile();
            StartCoroutine(_googleGps);
        }
        else
        {
            _queryGoogleGps = QueryGoogleGps();
            StartCoroutine(_queryGoogleGps);
        }
    }

    #region Promises

    private IEnumerator GetGeographicalCoordinatesByMobile()
    {
        Input.location.Start();
        int maximumWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maximumWait > 0)
        {
            yield return new WaitForSeconds(1);
            maximumWait--;
        }
        if (maximumWait < 1 || Input.location.status == LocationServiceStatus.Failed)
        {
            Input.location.Stop();
            yield break;
        }
        Latitude = Input.location.lastData.latitude;
        Longitude = Input.location.lastData.longitude;
        Input.location.Stop();

        _queryGoogleGps = QueryGoogleGps();
        StartCoroutine(_queryGoogleGps);
    }

    private IEnumerator GetGeographicalCoordinatesByIp()
    {
        WWW www = new WWW("http://ip-api.com/json");

        yield return www;

        if (www.error != null)
            yield break;

        ReadIspData(www.text);
    }

    private IEnumerator QueryGoogleGps()
    {
        var request = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + Latitude + "," + Longitude
                          + "&" + _useSensor
                          + "&" + _apiKey;
        WWW www = new WWW(request);

        yield return www;

        if (string.IsNullOrEmpty(www.text) || www.error != null)
        {
            _ipGps = GetGeographicalCoordinatesByIp();
            StartCoroutine(_ipGps);
        }
        else
        {
            FormatGpsData(www.text);
        }
    }

    #endregion

    #region Private Methods

    private void FormatGpsData(string response)
    {
        GoogleGpsData gpsData = JsonConvert.DeserializeObject<GoogleGpsData>(response);
        foreach (var data in gpsData.results)
        {
            foreach (var addrC in data.address_components)
            {
                if (addrC.types.Contains("country") && addrC.types.Contains("political")
                    && string.IsNullOrEmpty(Country))
                {
                    Country = addrC.long_name;
                    CountryCode = addrC.short_name;
                }
                else if (addrC.types.Contains("administrative_area_level_2") || addrC.types.Contains("administrative_area_level_1") && addrC.types.Contains("political")
                    && string.IsNullOrEmpty(City))
                {
                    City = addrC.long_name;
                }
                else if (addrC.types.Contains("neighborhood") || addrC.types.Contains("neighbourhood")
                    && string.IsNullOrEmpty(Neighborhood))
                {
                    Neighborhood = addrC.long_name;
                }
            }
        }
    }

    private void ReadIspData(string response)
    {
        TextReader tR = new StringReader(response);
        JsonReader reader = new JsonTextReader(tR);
        JObject result = JObject.Load(reader);

        if (KeyExists(result, "city"))
            City = result.SelectToken("city").ToString();

        if (KeyExists(result, "country"))
            Country = result.SelectToken("country").ToString();

        if (KeyExists(result, "countryCode"))
            CountryCode = result.SelectToken("countryCode").ToString();

        if (KeyExists(result, "isp"))
            Neighborhood = "Area";

        if (KeyExists(result, "lat"))
        {
            var lat = result.SelectToken("lat").ToString();
            Latitude = float.Parse(lat);
        }
        if (KeyExists(result, "lat"))
        {
            var lon = result.SelectToken("lon").ToString();
            Longitude = float.Parse(lon);
        }

        ShowSelectedCountry();
    }

    private bool KeyExists(JObject o, string key)
    {
        foreach (JProperty property in o.Properties())
        {
            if (property.Name == key)
                return true;
        }
        return false;
    }

    private void ShowSelectedCountry()
    {
        foreach (Transform t in transform)
        {
            if (t.gameObject.name == Country)
            {
                _countryObject = t.gameObject;
                break;
            }
        }

        var camPos = Camera.main.transform.position;
        var globePos = transform.position;
        var countryPos = transform.position - transform.TransformPoint(_countryObject.transform.position);

        _rotationAngle = GetAngleOfRotationByDirections(
            new Vector2(camPos.x, camPos.z),
            new Vector2(globePos.x, globePos.z),
            new Vector2(countryPos.x, countryPos.z)
            );

        var aBitToTheSide = 20f;

        LTDescr ltRotate = LeanTween.rotateY(gameObject, _rotationAngle + aBitToTheSide, 5f);
        ltRotate.setEase(LeanTweenType.easeInOutBack);

        LeanTween.move(gameObject, new Vector3(-0.3f, 0.35f, -2.35f), 4f);
    }

    private float GetAngleOfRotationByDirections(Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
        //Debug.Log(pointA);
        //Debug.Log(pointB);
        //Debug.Log(pointC);

        var dir = (pointC - pointB).normalized;
        var dir2 = (pointB - pointA).normalized;
        var angle = Vector2.Angle(dir, dir2);
        angle = 360f - angle;
        return angle;
    }

    #endregion
}
