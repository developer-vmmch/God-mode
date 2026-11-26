using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSVS.GodMode.Environment
{
    /// <summary>
    /// Legacy wrapper routing to the authoritative KaraikalGeoProjection system.
    /// Preserves existing callers while guaranteeing mathematical GIS precision.
    /// </summary>
    public static class KaraikalGeoData
    {
        public const double OriginLat = KaraikalGeoProjection.OriginLatitude;
        public const double OriginLon = KaraikalGeoProjection.OriginLongitude;
        public const double MetersPerDegreeLat = KaraikalGeoProjection.MetersPerDegreeLat;
        public const double MetersPerDegreeLon = KaraikalGeoProjection.MetersPerDegreeLon;

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
            return KaraikalGeoProjection.GeoToWorld(lat, lon, altitude);
        }

        public static void WorldToGeo(Vector3 worldPos, out double lat, out double lon, out float alt)
        {
            KaraikalGeoProjection.WorldToGeo(worldPos, out lat, out lon, out alt);
        }

        public static List<Landmark> KeyLandmarks
        {
            get
            {
                var list = new List<Landmark>();
                foreach (var glm in KaraikalGeoProjection.ValidatedLandmarks)
                {
                    list.Add(new Landmark(glm.NameTamil, glm.NameEnglish, glm.WorldPosition, glm.District, glm.Description));
                }
                return list;
            }
        }

        public static KaraikalDistrictType GetDistrictAtPosition(Vector3 pos)
        {
            return KaraikalGeoProjection.GetDistrictAtPosition(pos);
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
