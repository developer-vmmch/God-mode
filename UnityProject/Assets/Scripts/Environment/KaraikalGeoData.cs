using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSVS.GodMode.Environment
{
    public enum KaraikalDistrictType
    {
        TownCenter = 0,
        BeachPromenade = 1,
        ArasalarRiverfront = 2,
        HeritageTempleZone = 3,
        TransportHub = 4,
        PortIndustrialZone = 5,
        RuralOutskirts = 6,
        ModernExpansion = 7
    }

    /// <summary>
    /// Geographic metadata and landmark coordinates for Karaikal, Puducherry, India.
    /// Converts WGS84 Geographic coordinates into local Unity space (meters).
    /// Reference Origin (0,0,0) is centered at Bharathiyar Road & Kamarajar Salai (10.9254 N, 79.8380 E).
    /// </summary>
    public static class KaraikalGeoData
    {
        public const double OriginLat = 10.9254; // Reference latitude
        public const double OriginLon = 79.8380; // Reference longitude

        // Approximate meters per degree at 10.9 degrees latitude
        public const double MetersPerDegreeLat = 110574.0;
        public const double MetersPerDegreeLon = 109280.0;

        public struct Landmark
        {
            public string NameTamil;
            public string NameEnglish;
            public Vector3 LocalPosition;
            public KaraikalDistrictType District;
            public string Description;

            public Landmark(string tamil, string english, Vector3 pos, KaraikalDistrictType district, string desc)
            {
                NameTamil = tamil;
                NameEnglish = english;
                LocalPosition = pos;
                District = district;
                Description = desc;
            }
        }

        public static Vector3 GeoToWorld(double lat, double lon, float altitude = 0f)
        {
            float x = (float)((lon - OriginLon) * MetersPerDegreeLon);
            float z = (float)((lat - OriginLat) * MetersPerDegreeLat);
            return new Vector3(x, altitude, z);
        }

        public static readonly List<Landmark> KeyLandmarks = new List<Landmark>
        {
            new Landmark("காரைக்கால் நகரம்", "Karaikal Town Center", new Vector3(0f, 0f, 0f), KaraikalDistrictType.TownCenter, "Historic commercial center at Kamarajar Salai & Bharathiyar Rd"),
            new Landmark("காரைக்கால் கடற்கரை", "Karaikal Beach Promenade", new Vector3(650f, 0f, 20f), KaraikalDistrictType.BeachPromenade, "Bay of Bengal coastal promenade, beach dunes, and lighthouse park"),
            new Landmark("அரசலாறு பாலம்", "Arasalar River Bridge", new Vector3(0f, 2f, -380f), KaraikalDistrictType.ArasalarRiverfront, "NH32 highway bridge crossing the historic Arasalar River into southern districts"),
            new Landmark("அம்மையார் கோயில்", "Karaikal Ammaiyar Temple", new Vector3(-220f, 0f, 180f), KaraikalDistrictType.HeritageTempleZone, "Celebrated 6th-century Saivite saint heritage temple and cultural tank area"),
            new Landmark("புதிய பேருந்து நிலையம்", "Karaikal New Bus Stand", new Vector3(-180f, 0f, 520f), KaraikalDistrictType.TransportHub, "Interstate transport terminal connecting Puducherry, Tamil Nadu, and Chennai"),
            new Landmark("காரைக்கால் ரயில் நிலையம்", "Karaikal Railway Station", new Vector3(-550f, 0f, 650f), KaraikalDistrictType.TransportHub, "Southern Railway terminal station serving coastal delta passenger lines"),
            new Landmark("திருநள்ளாறு சாலை", "Thirunallar Highway", new Vector3(-750f, 0f, 50f), KaraikalDistrictType.RuralOutskirts, "Arterial highway heading west to Thirunallar Saneeswaran Temple"),
            new Landmark("காரைக்கால் துறைமுகம்", "Karaikal Port & Logistics", new Vector3(350f, 0f, -850f), KaraikalDistrictType.PortIndustrialZone, "Deepwater maritime cargo terminal, shipping container yards, and cranes"),
            new Landmark("நவீன கடற்கரை வளாகம்", "Karaikal Modern Waterfront", new Vector3(450f, 0f, 650f), KaraikalDistrictType.ModernExpansion, "Fictional modern commercial high-rise district with destructible towers")
        };

        public static KaraikalDistrictType GetDistrictAtPosition(Vector3 pos)
        {
            if (pos.x > 500f && pos.z > -300f && pos.z < 450f)
                return KaraikalDistrictType.BeachPromenade;
            if (pos.z < -250f && pos.z > -500f)
                return KaraikalDistrictType.ArasalarRiverfront;
            if (pos.z <= -500f)
                return KaraikalDistrictType.PortIndustrialZone;
            if (pos.x < -400f && pos.z > 300f)
                return KaraikalDistrictType.TransportHub;
            if (pos.x < -350f)
                return KaraikalDistrictType.RuralOutskirts;
            if (pos.x < 0f && pos.z > 50f && pos.z < 350f)
                return KaraikalDistrictType.HeritageTempleZone;
            if (pos.z >= 500f && pos.x >= 200f)
                return KaraikalDistrictType.ModernExpansion;

            return KaraikalDistrictType.TownCenter;
        }

        public static string GetDistrictDisplayName(KaraikalDistrictType district)
        {
            switch (district)
            {
                case KaraikalDistrictType.TownCenter: return "காரைக்கால் நகரம் | KARAIKAL TOWN CENTER";
                case KaraikalDistrictType.BeachPromenade: return "காரைக்கால் கடற்கரை | KARAIKAL BEACH";
                case KaraikalDistrictType.ArasalarRiverfront: return "அரசலாறு ஆறு | ARASALAR RIVERFRONT";
                case KaraikalDistrictType.HeritageTempleZone: return "பாரம்பரிய பகுதி | HERITAGE & TEMPLE ZONE";
                case KaraikalDistrictType.TransportHub: return "போக்குவரத்து மையம் | TRANSPORT HUB";
                case KaraikalDistrictType.PortIndustrialZone: return "காரைக்கால் துறைமுகம் | KARAIKAL PORT";
                case KaraikalDistrictType.RuralOutskirts: return "திருநள்ளாறு வழி | THIRUNALLAR OUTSKIRTS";
                case KaraikalDistrictType.ModernExpansion: return "நவீன வளாகம் | MODERN HIGH-RISE DISTRICT";
                default: return "காரைக்கால் | KARAIKAL";
            }
        }
    }
}
