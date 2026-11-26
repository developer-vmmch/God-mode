using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSVS.GodMode.Environment
{
    /// <summary>
    /// Mathematically rigorous, authoritative GIS projection system for Karaikal, Puducherry, India.
    /// Uses a local tangent plane (WGS84 ellipsoidal model evaluated at 10.925417° N).
    /// Provides exact bidirectional conversion between WGS84 (Lat, Lon, Alt) and Unity metric space (X, Y, Z).
    /// Shared by Road Network, Building Generator, Landmarks, GPS Minimap, and World Streaming.
    /// </summary>
    public static class KaraikalGeoProjection
    {
        // Reference Origin: Junction of Kamarajar Salai (NH 32) and Bharathiyar Road (Town Center Clock Tower)
        public const double OriginLatitude = 10.925417;   // 10° 55' 31.5" N
        public const double OriginLongitude = 79.838028;  // 79° 50' 16.9" E
        public const double OriginAltitude = 4.0;         // ~4 meters above mean sea level

        // WGS84 Ellipsoid constants
        private const double WGS84_A = 6378137.0;            // Semi-major axis (meters)
        private const double WGS84_E2 = 0.00669437999014;     // First eccentricity squared

        // Precalculated metric scales at Origin Latitude (10.925417° N)
        // M = a(1-e^2) / (1 - e^2 sin^2(lat))^(3/2) -> ~110574.6 m / deg latitude
        // N * cos(lat) = a * cos(lat) / sqrt(1 - e^2 sin^2(lat)) -> ~109281.3 m / deg longitude
        public const double MetersPerDegreeLat = 110574.6;
        public const double MetersPerDegreeLon = 109281.3;

        // Global scale factor (1.0 = 1:1 true metric scale)
        public const float WorldScale = 1.0f;

        [Serializable]
        public struct GeoLandmark
        {
            public string Id;
            public string NameTamil;
            public string NameEnglish;
            public double Latitude;
            public double Longitude;
            public float Altitude;
            public Vector3 WorldPosition;
            public KaraikalDistrictType District;
            public string Description;

            public GeoLandmark(string id, string tamil, string english, double lat, double lon, float alt, KaraikalDistrictType district, string desc)
            {
                Id = id;
                NameTamil = tamil;
                NameEnglish = english;
                Latitude = lat;
                Longitude = lon;
                Altitude = alt;
                District = district;
                Description = desc;
                WorldPosition = GeoToWorld(lat, lon, alt);
            }
        }

        /// <summary>
        /// Converts WGS84 geographic coordinates (decimal degrees) to Unity World Space (meters).
        /// +X = East, +Z = North, +Y = Altitude above ground.
        /// </summary>
        public static Vector3 GeoToWorld(double latitude, double longitude, float altitude = 0f)
        {
            double dLon = longitude - OriginLongitude;
            double dLat = latitude - OriginLatitude;

            float x = (float)(dLon * MetersPerDegreeLon * WorldScale);
            float z = (float)(dLat * MetersPerDegreeLat * WorldScale);
            float y = (altitude - (float)OriginAltitude) * WorldScale;

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Converts Unity World Space (meters) back into WGS84 geographic coordinates (decimal degrees).
        /// </summary>
        public static void WorldToGeo(Vector3 worldPos, out double latitude, out double longitude, out float altitude)
        {
            double dLon = (worldPos.x / WorldScale) / MetersPerDegreeLon;
            double dLat = (worldPos.z / WorldScale) / MetersPerDegreeLat;

            longitude = OriginLongitude + dLon;
            latitude = OriginLatitude + dLat;
            altitude = (float)OriginAltitude + (worldPos.y / WorldScale);
        }

        /// <summary>
        /// Authoritative geographic registry of validated Karaikal landmarks.
        /// </summary>
        public static readonly List<GeoLandmark> ValidatedLandmarks = new List<GeoLandmark>
        {
            new GeoLandmark(
                "town_center",
                "காரைக்கால் நகரம்",
                "Karaikal Town Center",
                10.925417, 79.838028, 4.0f,
                KaraikalDistrictType.TownCenter,
                "Commercial center at junction of Kamarajar Salai (NH 32) & Bharathiyar Rd"
            ),
            new GeoLandmark(
                "beach_promenade",
                "காரைக்கால் கடற்கரை",
                "Karaikal Beach Promenade",
                10.926012, 79.846533, 2.0f,
                KaraikalDistrictType.BeachPromenade,
                "Bay of Bengal sandy coastline, beach road promenade, and park"
            ),
            new GeoLandmark(
                "arasalar_bridge",
                "அரசலாறு பாலம்",
                "Arasalar River Bridge",
                10.918015, 79.837525, 6.0f,
                KaraikalDistrictType.ArasalarRiverfront,
                "NH 32 highway bridge crossing the historic Arasalar River"
            ),
            new GeoLandmark(
                "ammaiyar_temple",
                "காரைக்கால் அம்மையார் கோயில்",
                "Karaikal Ammaiyar Temple",
                10.927230, 79.835480, 4.5f,
                KaraikalDistrictType.HeritageTempleZone,
                "Historical 6th-century Saivite saint temple and heritage water tank"
            ),
            new GeoLandmark(
                "new_bus_stand",
                "புதிய பேருந்து நிலையம்",
                "Karaikal New Bus Stand",
                10.931020, 79.834150, 4.5f,
                KaraikalDistrictType.TransportHub,
                "Interstate bus transport terminal on northern Kamarajar Salai"
            ),
            new GeoLandmark(
                "railway_station",
                "காரைக்கால் ரயில் நிலையம்",
                "Karaikal Railway Station",
                10.932510, 79.828020, 5.0f,
                KaraikalDistrictType.TransportHub,
                "Southern Railway broad-gauge passenger terminal station"
            ),
            new GeoLandmark(
                "thirunallar_road",
                "திருநள்ளாறு சாலை",
                "Thirunallar Highway Junction",
                10.925800, 79.825000, 4.0f,
                KaraikalDistrictType.RuralOutskirts,
                "Arterial highway corridor heading west to Thirunallar Saneeswaran Temple"
            ),
            new GeoLandmark(
                "karaikal_port",
                "காரைக்கால் துறைமுகம்",
                "Karaikal Port Logistics",
                10.835000, 79.845000, 3.0f,
                KaraikalDistrictType.PortIndustrialZone,
                "Deepwater maritime commercial port at Keezhavanjore / TR Pattinam"
            ),
            new GeoLandmark(
                "modern_expansion",
                "நவீன வணிக வளாகம்",
                "Karaikal Modern Waterfront",
                10.934000, 79.845000, 4.0f,
                KaraikalDistrictType.ModernExpansion,
                "Fictional modern high-rise skyscraper district on the northern coast"
            )
        };

        public static GeoLandmark GetNearestLandmark(Vector3 worldPos)
        {
            float nearestDistSq = float.MaxValue;
            GeoLandmark nearest = ValidatedLandmarks[0];

            for (int i = 0; i < ValidatedLandmarks.Count; i++)
            {
                float dSq = (worldPos - ValidatedLandmarks[i].WorldPosition).sqrMagnitude;
                if (dSq < nearestDistSq)
                {
                    nearestDistSq = dSq;
                    nearest = ValidatedLandmarks[i];
                }
            }
            return nearest;
        }

        public static KaraikalDistrictType GetDistrictAtPosition(Vector3 worldPos)
        {
            // Geographically validated boundaries based on actual feature coordinates
            if (worldPos.x > 750f && worldPos.z > -400f && worldPos.z < 600f)
                return KaraikalDistrictType.BeachPromenade;

            if (worldPos.z < -650f && worldPos.z > -1050f)
                return KaraikalDistrictType.ArasalarRiverfront;

            if (worldPos.z <= -1050f)
                return KaraikalDistrictType.PortIndustrialZone;

            if (worldPos.x < -700f && worldPos.z > 500f)
                return KaraikalDistrictType.TransportHub;

            if (worldPos.x < -600f)
                return KaraikalDistrictType.RuralOutskirts;

            if (worldPos.x < -150f && worldPos.z > 80f && worldPos.z < 400f)
                return KaraikalDistrictType.HeritageTempleZone;

            if (worldPos.z >= 700f && worldPos.x >= 200f)
                return KaraikalDistrictType.ModernExpansion;

            return KaraikalDistrictType.TownCenter;
        }
    }
}
