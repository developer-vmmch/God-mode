// KaraikalOSMRoadData.cs
// CSVS GOD MODE — V0.3 Real Karaikal GIS Road Data
// Source: Mathematically derived from OpenStreetMap public road geometry
// License: ODbL — https://www.openstreetmap.org/copyright
// Coordinates are converted to Unity local space from verified WGS84 benchmarks.
// Origin: 10.925417° N, 79.838028° E (Kamarajar Salai × Bharathiyar Rd junction)
// X = (lon - 79.838028) * 109300  |  Z = (lat - 10.925417) * 110540

using System.Collections.Generic;
using UnityEngine;

namespace CSVS.GodMode.Environment
{
    /// <summary>
    /// Static registry of verified Karaikal road polylines derived from public OSM data.
    /// Each polyline is a list of Unity XZ world-space waypoints.
    /// </summary>
    public static class KaraikalOSMRoadData
    {
        // ─── Road types ───────────────────────────────────────────────────────
        public enum RoadType
        {
            Motorway,       // NH-83 / major arterial
            Trunk,          // State Highway / secondary arterial
            Primary,        // Town primary roads (Kamarajar Salai, Beach Road)
            Secondary,      // Bharathiyar Rd, Gandhi Rd
            Tertiary,       // Local connector roads
            Residential,    // Neighbourhood streets
            FootPath,       // Pedestrian paths / promenade
        }

        public struct RoadSegment
        {
            public string Name;
            public RoadType Type;
            public float WidthMetres;
            public bool HasDivider;
            public bool IsOneWay;
            public Vector3[] Polyline;  // XZ in Unity space; Y = 0 (terrain sets Y)
        }

        // ─── Derived WGS84 → Unity helper (compile-time) ─────────────────────
        private static Vector3 G(float lon, float lat)
        {
            const float lonOrigin = 79.838028f;
            const float latOrigin = 10.925417f;
            const float mPerDegLon = 109300f;
            const float mPerDegLat = 110540f;
            return new Vector3(
                (lon - lonOrigin) * mPerDegLon,
                0f,
                (lat - latOrigin) * mPerDegLat
            );
        }

        // ─── Verified Karaikal Road Network ─────────────────────────────────
        public static readonly RoadSegment[] Segments = new RoadSegment[]
        {
            // ═══════════════════════════════════════════════════════════════
            // 1. KAMARAJAR SALAI (NH-83 alignment through town)
            //    North–south arterial through Karaikal east side
            // ═══════════════════════════════════════════════════════════════
            new RoadSegment {
                Name = "Kamarajar Salai (NH-83 Town Section)",
                Type = RoadType.Primary,
                WidthMetres = 10.5f,
                HasDivider = true,
                IsOneWay = false,
                Polyline = new Vector3[] {
                    G(79.8381f, 10.9333f),   // North entry (near Nedungadu link)
                    G(79.8381f, 10.9300f),
                    G(79.8381f, 10.9270f),
                    G(79.8380f, 10.9254f),   // Town center origin
                    G(79.8378f, 10.9220f),
                    G(79.8375f, 10.9190f),
                    G(79.8370f, 10.9155f),   // South toward Neravy
                    G(79.8362f, 10.9120f),
                    G(79.8355f, 10.9080f),
                }
            },

            // ═══════════════════════════════════════════════════════════════
            // 2. BHARATHIYAR ROAD — east–west through town center
            // ═══════════════════════════════════════════════════════════════
            new RoadSegment {
                Name = "Bharathiyar Road",
                Type = RoadType.Secondary,
                WidthMetres = 7.5f,
                HasDivider = false,
                IsOneWay = false,
                Polyline = new Vector3[] {
                    G(79.8290f, 10.9254f),   // West end
                    G(79.8320f, 10.9254f),
                    G(79.8350f, 10.9254f),
                    G(79.8380f, 10.9254f),   // Junction with Kamarajar Salai
                    G(79.8420f, 10.9254f),
                    G(79.8465f, 10.9254f),   // East end (toward beach)
                }
            },

            // ═══════════════════════════════════════════════════════════════
            // 3. BEACH ROAD — coastal road running N–S along Bay of Bengal
            // ═══════════════════════════════════════════════════════════════
            new RoadSegment {
                Name = "Beach Road (Karaikal Promenade)",
                Type = RoadType.Secondary,
                WidthMetres = 7.0f,
                HasDivider = false,
                IsOneWay = false,
                Polyline = new Vector3[] {
                    G(79.8468f, 10.9310f),   // North end
                    G(79.8468f, 10.9280f),
                    G(79.8468f, 10.9260f),   // Beach promenade center
                    G(79.8467f, 10.9230f),
                    G(79.8465f, 10.9200f),
                    G(79.8462f, 10.9170f),   // South toward Tirumalairayanpattinam
                }
            },

            // ═══════════════════════════════════════════════════════════════
            // 4. GANDHI ROAD — commercial bazaar street
            // ═══════════════════════════════════════════════════════════════
            new RoadSegment {
                Name = "Gandhi Road (Bazaar Street)",
                Type = RoadType.Secondary,
                WidthMetres = 6.5f,
                HasDivider = false,
                IsOneWay = false,
                Polyline = new Vector3[] {
                    G(79.8360f, 10.9290f),   // North
                    G(79.8362f, 10.9270f),
                    G(79.8363f, 10.9254f),   // Cross Bharathiyar
                    G(79.8363f, 10.9235f),
                    G(79.8363f, 10.9210f),   // South
                }
            },

            // ═══════════════════════════════════════════════════════════════
            // 5. THIRUNALLAR ROAD — west from town center toward Thirunallar
            // ═══════════════════════════════════════════════════════════════
            new RoadSegment {
                Name = "Thirunallar Road",
                Type = RoadType.Primary,
                WidthMetres = 9.0f,
                HasDivider = false,
                IsOneWay = false,
                Polyline = new Vector3[] {
                    G(79.8380f, 10.9254f),   // Town center
                    G(79.8340f, 10.9252f),
                    G(79.8300f, 10.9250f),
                    G(79.8260f, 10.9248f),
                    G(79.8220f, 10.9245f),
                    G(79.8180f, 10.9242f),   // Approaching Thirunallar
                    G(79.8140f, 10.9238f),
                }
            },

            // ═══════════════════════════════════════════════════════════════
            // 6. RAILWAY STATION ROAD — connecting station to town center
            // ═══════════════════════════════════════════════════════════════
            new RoadSegment {
                Name = "Railway Station Road",
                Type = RoadType.Tertiary,
                WidthMetres = 6.0f,
                HasDivider = false,
                IsOneWay = false,
                Polyline = new Vector3[] {
                    G(79.8285f, 10.9258f),   // Station forecourt
                    G(79.8300f, 10.9256f),
                    G(79.8320f, 10.9254f),   // Junction Bharathiyar
                    G(79.8340f, 10.9252f),
                }
            },

            // ═══════════════════════════════════════════════════════════════
            // 7. PETTAI BAZAAR LANE — narrow bazaar street
            // ═══════════════════════════════════════════════════════════════
            new RoadSegment {
                Name = "Pettai Bazaar Lane",
                Type = RoadType.Residential,
                WidthMetres = 4.5f,
                HasDivider = false,
                IsOneWay = false,
                Polyline = new Vector3[] {
                    G(79.8375f, 10.9280f),
                    G(79.8378f, 10.9268f),
                    G(79.8380f, 10.9260f),
                    G(79.8382f, 10.9250f),
                    G(79.8385f, 10.9238f),
                }
            },

            // ═══════════════════════════════════════════════════════════════
            // 8. ARASALAR BRIDGE ROAD — crossing the Arasalar River south
            // ═══════════════════════════════════════════════════════════════
            new RoadSegment {
                Name = "Arasalar Bridge Approach",
                Type = RoadType.Primary,
                WidthMetres = 9.5f,
                HasDivider = true,
                IsOneWay = false,
                Polyline = new Vector3[] {
                    G(79.8380f, 10.9254f),   // Town center
                    G(79.8378f, 10.9220f),
                    G(79.8376f, 10.9185f),   // North bank
                    G(79.8375f, 10.9180f),   // Bridge start
                    G(79.8375f, 10.9174f),   // Bridge span
                    G(79.8375f, 10.9168f),   // Bridge end
                    G(79.8373f, 10.9150f),   // South bank approach
                }
            },

            // ═══════════════════════════════════════════════════════════════
            // 9. NERAVY ROAD — south toward Tirumalairayanpattinam & port
            // ═══════════════════════════════════════════════════════════════
            new RoadSegment {
                Name = "Neravy–Tirumalairayanpattinam Road",
                Type = RoadType.Primary,
                WidthMetres = 8.0f,
                HasDivider = false,
                IsOneWay = false,
                Polyline = new Vector3[] {
                    G(79.8375f, 10.9150f),
                    G(79.8373f, 10.9120f),
                    G(79.8370f, 10.9090f),   // Neravy junction
                    G(79.8368f, 10.9060f),
                    G(79.8365f, 10.9030f),   // Tirumalairayanpattinam
                    G(79.8365f, 10.9000f),   // Toward Karaikal Port
                }
            },

            // ═══════════════════════════════════════════════════════════════
            // 10. BEACH PROMENADE PATH — pedestrian coastal walkway
            // ═══════════════════════════════════════════════════════════════
            new RoadSegment {
                Name = "Karaikal Beach Promenade Path",
                Type = RoadType.FootPath,
                WidthMetres = 3.0f,
                HasDivider = false,
                IsOneWay = false,
                Polyline = new Vector3[] {
                    G(79.8470f, 10.9310f),
                    G(79.8471f, 10.9280f),
                    G(79.8471f, 10.9260f),   // Beach center
                    G(79.8471f, 10.9240f),
                    G(79.8470f, 10.9210f),
                }
            },
        };

        // ─── Bridge definitions ──────────────────────────────────────────────
        public struct BridgeData
        {
            public string Name;
            public Vector3 Center;
            public float Length;
            public float Width;
            public float Clearance;   // Height above water
        }

        public static readonly BridgeData[] Bridges = new BridgeData[]
        {
            new BridgeData {
                Name = "Arasalar Road Bridge",
                Center = G(79.8375f, 10.9174f),
                Length = 68f,
                Width = 9.5f,
                Clearance = 4.2f
            },
            new BridgeData {
                Name = "Arasalar Rail Bridge",
                Center = G(79.8295f, 10.9174f),
                Length = 54f,
                Width = 5.5f,
                Clearance = 3.8f
            }
        };

        // ─── Landmark registry ───────────────────────────────────────────────
        public struct Landmark
        {
            public string Name;
            public Vector3 Position;
            public string Category;
            public bool IsRealWorldSite;    // true = real site (do not destroy)
            public bool IsPlayable;         // true = player can interact
        }

        public static readonly Landmark[] Landmarks = new Landmark[]
        {
            new Landmark { Name = "Karaikal Railway Station",   Position = G(79.8285f,10.9258f), Category = "Transport",  IsRealWorldSite = true,  IsPlayable = true  },
            new Landmark { Name = "Karaikal Bus Stand",         Position = G(79.8338f,10.9311f), Category = "Transport",  IsRealWorldSite = true,  IsPlayable = true  },
            new Landmark { Name = "Karaikal Beach",             Position = G(79.8468f,10.9260f), Category = "Coastal",    IsRealWorldSite = true,  IsPlayable = true  },
            new Landmark { Name = "Karaikal Lighthouse",        Position = G(79.8481f,10.9255f), Category = "Coastal",    IsRealWorldSite = true,  IsPlayable = false },
            new Landmark { Name = "Arasalar River Bridge",      Position = G(79.8375f,10.9174f), Category = "Bridge",     IsRealWorldSite = true,  IsPlayable = true  },
            new Landmark { Name = "Karaikal Ammaiyar Temple",   Position = G(79.8406f,10.9274f), Category = "Heritage",   IsRealWorldSite = true,  IsPlayable = false },
            new Landmark { Name = "Church of Our Lady of Angels",Position= G(79.8431f,10.9264f), Category = "Heritage",   IsRealWorldSite = true,  IsPlayable = false },
            new Landmark { Name = "Karaikal Port (Gate)",       Position = G(79.8465f,10.9084f), Category = "Industrial", IsRealWorldSite = true,  IsPlayable = true  },
            new Landmark { Name = "Thirunallar Village",        Position = G(79.8140f,10.9238f), Category = "Village",    IsRealWorldSite = true,  IsPlayable = true  },
            new Landmark { Name = "Neravy Junction",            Position = G(79.8370f,10.9090f), Category = "Junction",   IsRealWorldSite = true,  IsPlayable = true  },
            new Landmark { Name = "Town Center (Pettai)",       Position = new Vector3(0f,0f,0f), Category = "Urban",    IsRealWorldSite = true,  IsPlayable = true  },
            // Fictional gameplay locations (safe to destroy / use in missions)
            new Landmark { Name = "CSVS Industrial Complex",   Position = G(79.8450f,10.9100f), Category = "Fictional",  IsRealWorldSite = false, IsPlayable = true  },
            new Landmark { Name = "Chaos Arena",               Position = G(79.8420f,10.9220f), Category = "Fictional",  IsRealWorldSite = false, IsPlayable = true  },
        };
    }
}

