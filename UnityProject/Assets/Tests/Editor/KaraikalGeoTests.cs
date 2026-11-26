using NUnit.Framework;
using UnityEngine;
using CSVS.GodMode.Environment;

namespace CSVS.GodMode.Tests.Editor
{
    [TestFixture]
    public class KaraikalGeoTests
    {
        [Test]
        public void OriginCoordinates_MapToExactZero()
        {
            Vector3 worldPos = KaraikalGeoProjection.GeoToWorld(
                KaraikalGeoProjection.OriginLatitude,
                KaraikalGeoProjection.OriginLongitude,
                (float)KaraikalGeoProjection.OriginAltitude
            );

            Assert.AreEqual(0.0f, worldPos.x, 0.001f, "Origin longitude must project to X = 0.");
            Assert.AreEqual(0.0f, worldPos.z, 0.001f, "Origin latitude must project to Z = 0.");
            Assert.AreEqual(0.0f, worldPos.y, 0.001f, "Origin altitude must project to Y = 0.");
        }

        [Test]
        public void GeoToWorld_And_WorldToGeo_BidirectionalRoundTrip()
        {
            double testLat = 10.927230;
            double testLon = 79.835480;
            float testAlt = 4.5f;

            Vector3 world = KaraikalGeoProjection.GeoToWorld(testLat, testLon, testAlt);
            KaraikalGeoProjection.WorldToGeo(world, out double outLat, out double outLon, out float outAlt);

            Assert.AreEqual(testLat, outLat, 0.000001, "Latitude round-trip precision must be within 1e-6 degrees (~0.1m).");
            Assert.AreEqual(testLon, outLon, 0.000001, "Longitude round-trip precision must be within 1e-6 degrees (~0.1m).");
            Assert.AreEqual(testAlt, outAlt, 0.001f, "Altitude round-trip precision must be within 1mm.");
        }

        [Test]
        public void ValidatedLandmarks_BeachCoordinates_MatchExpectedMetricOffset()
        {
            // Expected approximately X=929, Z=66
            var beach = KaraikalGeoProjection.ValidatedLandmarks.Find(lm => lm.Id == "beach_promenade");
            Assert.IsNotNull(beach.Id, "Beach landmark must be present in registry.");

            Assert.AreEqual(929.0f, beach.WorldPosition.x, 2.0f, "Beach promenade X offset should match expected GIS metric projection ~929m.");
            Assert.AreEqual(66.0f, beach.WorldPosition.z, 2.0f, "Beach promenade Z offset should match expected GIS metric projection ~66m.");
        }

        [Test]
        public void ValidatedLandmarks_ArasalarBridge_MatchExpectedMetricOffset()
        {
            // Expected approximately X=-55, Z=-818
            var bridge = KaraikalGeoProjection.ValidatedLandmarks.Find(lm => lm.Id == "arasalar_bridge");
            Assert.IsNotNull(bridge.Id, "Arasalar Bridge landmark must be present in registry.");

            Assert.AreEqual(-55.0f, bridge.WorldPosition.x, 2.0f, "Arasalar Bridge X offset should match expected GIS metric projection ~ -55m.");
            Assert.AreEqual(-818.0f, bridge.WorldPosition.z, 2.0f, "Arasalar Bridge Z offset should match expected GIS metric projection ~ -818m.");
        }

        [Test]
        public void ValidatedLandmarks_RailwayStation_MatchExpectedMetricOffset()
        {
            // Expected approximately X=-1093, Z=785
            var railway = KaraikalGeoProjection.ValidatedLandmarks.Find(lm => lm.Id == "railway_station");
            Assert.IsNotNull(railway.Id, "Railway Station landmark must be present in registry.");

            Assert.AreEqual(-1094.0f, railway.WorldPosition.x, 3.0f, "Railway Station X offset should match expected GIS metric projection ~ -1094m.");
            Assert.AreEqual(785.0f, railway.WorldPosition.z, 3.0f, "Railway Station Z offset should match expected GIS metric projection ~ 785m.");
        }

        [Test]
        public void ValidatedLandmarks_KaraikalPort_MatchExpectedMetricOffset()
        {
            // Expected approximately X=765, Z=-9996
            var port = KaraikalGeoProjection.ValidatedLandmarks.Find(lm => lm.Id == "karaikal_port");
            Assert.IsNotNull(port.Id, "Karaikal Port landmark must be present in registry.");

            Assert.AreEqual(762.0f, port.WorldPosition.x, 5.0f, "Karaikal Port X offset should match expected GIS metric projection ~ 762m.");
            Assert.AreEqual(-9998.0f, port.WorldPosition.z, 5.0f, "Karaikal Port Z offset should match expected GIS metric projection ~ -9998m.");
        }

        [Test]
        public void DistrictClassification_DetectsTownCenterAndBoundaries()
        {
            // Town center (0, 0, 0)
            Assert.AreEqual(KaraikalDistrictType.TownCenter, KaraikalGeoProjection.GetDistrictAtPosition(Vector3.zero));

            // Beach promenade (+900, 0, 50)
            Assert.AreEqual(KaraikalDistrictType.BeachPromenade, KaraikalGeoProjection.GetDistrictAtPosition(new Vector3(900f, 0f, 50f)));

            // Arasalar Riverfront (0, 0, -800)
            Assert.AreEqual(KaraikalDistrictType.ArasalarRiverfront, KaraikalGeoProjection.GetDistrictAtPosition(new Vector3(0f, 0f, -800f)));

            // Port Industrial Zone (700, 0, -2000)
            Assert.AreEqual(KaraikalDistrictType.PortIndustrialZone, KaraikalGeoProjection.GetDistrictAtPosition(new Vector3(700f, 0f, -2000f)));
        }

        [Test]
        public void ChunkStreaming_WorldToChunkCoord_And_ChunkCoordToWorldCenter()
        {
            // Origin chunk
            Vector2Int originCoord = KaraikalWorldStreamer.WorldToChunkCoord(Vector3.zero);
            Assert.AreEqual(new Vector2Int(0, 0), originCoord);

            Vector3 originCenter = KaraikalWorldStreamer.ChunkCoordToWorldCenter(originCoord);
            Assert.AreEqual(Vector3.zero, originCenter);

            // Chunk at (250, 0, 500)
            Vector3 testPos = new Vector3(250f, 0f, 500f);
            Vector2Int coord = KaraikalWorldStreamer.WorldToChunkCoord(testPos);
            Assert.AreEqual(new Vector2Int(1, 2), coord);

            Vector3 chunkCenter = KaraikalWorldStreamer.ChunkCoordToWorldCenter(coord);
            Assert.AreEqual(testPos, chunkCenter);
        }
    }
}
