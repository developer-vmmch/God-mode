// MissionSystem.cs
// CSVS GOD MODE — V0.3 Original Mission Framework
// All missions are original creative work by CSVS IT Tech.
// No Rockstar Games missions, characters or IP are reproduced.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CSVS.GodMode.Core;
using CSVS.GodMode.Player;

namespace CSVS.GodMode.Missions
{
    public enum MissionState { Locked, Available, Active, Completed, Failed }
    public enum ObjectiveType  { ReachLocation, ProtectCivilian, DestroyTarget, CollectItem, SurviveTime, AltitudeFlight }

    [Serializable]
    public class MissionObjective
    {
        public string Description;
        public ObjectiveType Type;
        public Vector3 TargetPosition;
        public float Radius = 10f;
        public int RequiredCount = 1;
        public float TimeLimitSeconds = -1f;  // -1 = no time limit

        [HideInInspector] public int Progress;
        [HideInInspector] public bool IsComplete;

        public bool CheckComplete(Vector3 playerPos)
        {
            switch (Type)
            {
                case ObjectiveType.ReachLocation:
                    if (Vector3.Distance(playerPos, TargetPosition) <= Radius)
                    { IsComplete = true; Progress = RequiredCount; }
                    break;
                case ObjectiveType.CollectItem:
                case ObjectiveType.DestroyTarget:
                case ObjectiveType.ProtectCivilian:
                    if (Progress >= RequiredCount) IsComplete = true;
                    break;
                case ObjectiveType.AltitudeFlight:
                    if (playerPos.y >= TargetPosition.y) IsComplete = true;
                    break;
            }
            return IsComplete;
        }
    }

    [Serializable]
    public class MissionData
    {
        public string MissionID;
        public string Title;
        [TextArea(2, 5)]
        public string BriefingText;
        [TextArea(1, 3)]
        public string CompletionText;
        public int ReputationReward;
        public MissionState State;
        public List<MissionObjective> Objectives = new List<MissionObjective>();
        public bool RequiresSuperheroMode;
        public string UnlocksMissionID;   // Which mission to unlock on completion
    }

    /// <summary>
    /// Central runtime mission manager.
    /// Tracks active mission, objective progress, rewards, and save state.
    /// </summary>
    public class MissionSystem : MonoBehaviour
    {
        // ─── Inspector ────────────────────────────────────────────────────
        [SerializeField] private List<MissionData> allMissions = new List<MissionData>();
        [SerializeField] private MissionHUDController hudController;
        [SerializeField] private Transform waypointMarker;    // Reusable world-space arrow/marker

        // ─── Events ───────────────────────────────────────────────────────
        public event Action<MissionData> OnMissionStarted;
        public event Action<MissionData> OnMissionCompleted;
        public event Action<MissionData> OnMissionFailed;
        public event Action<MissionObjective> OnObjectiveCompleted;

        // ─── State ─────────────────────────────────────────────────────────
        public MissionData ActiveMission { get; private set; }
        private bool _isMissionActive;
        private Coroutine _missionCoroutine;

        private static MissionSystem _instance;
        public static MissionSystem Instance => _instance;

        // ─── Lifecycle ─────────────────────────────────────────────────────
        private void Awake()
        {
            _instance = this;
            InitializeDefaultMissions();
        }

        // ─── Default Mission Definitions ───────────────────────────────────
        public void InitializeDefaultMissions()
        {
            if (allMissions != null && allMissions.Count > 0) return;  // Already configured in Inspector

            // Using KaraikalOSMRoadData coordinate helpers
            Vector3 railStation    = new Vector3(-1041f, 0f,   42f);
            Vector3 beach          = new Vector3(  929f, 0f,   66f);
            Vector3 araSalarBridge = new Vector3(  -55f, 0f, -818f);
            Vector3 portGate       = new Vector3(  925f, 0f,-1881f);
            Vector3 townCenter     = Vector3.zero;
            Vector3 industrialZone = new Vector3(1200f, 0f, -950f);  // Fictional
            Vector3 chaosArena     = new Vector3( 850f, 0f, -300f);  // Fictional

            allMissions = new List<MissionData>
            {
                // ═══════════════════════════════════════════════════════════
                new MissionData {
                    MissionID = "M01_Welcome",
                    Title     = "Welcome to Karaikal",
                    BriefingText = "Discover Karaikal — the coastal pearl of Puducherry. "
                                 + "Walk to the railway station, then drive to the town center marketplace.",
                    CompletionText = "You've taken your first steps through Karaikal. The city awaits.",
                    ReputationReward = 10,
                    State = MissionState.Available,
                    UnlocksMissionID = "M02_Awakening",
                    Objectives = new List<MissionObjective> {
                        new MissionObjective { Description = "Walk to Karaikal Railway Station",   Type = ObjectiveType.ReachLocation, TargetPosition = railStation,  Radius = 25f },
                        new MissionObjective { Description = "Drive to Town Center (Pettai Bazaar)", Type = ObjectiveType.ReachLocation, TargetPosition = townCenter, Radius = 30f },
                    }
                },

                // ═══════════════════════════════════════════════════════════
                new MissionData {
                    MissionID = "M02_Awakening",
                    Title     = "Awakening",
                    BriefingText = "Something has changed inside you. "
                                 + "Press T to awaken your superhero form. Fly to the beach promenade.",
                    CompletionText = "The power is real. Karaikal's guardian has awakened.",
                    ReputationReward = 25,
                    State = MissionState.Locked,
                    RequiresSuperheroMode = true,
                    UnlocksMissionID = "M03_CoastalRescue",
                    Objectives = new List<MissionObjective> {
                        new MissionObjective { Description = "Transform into superhero mode (press T)", Type = ObjectiveType.ReachLocation, TargetPosition = townCenter, Radius = 500f },
                        new MissionObjective { Description = "Fly to Karaikal Beach Promenade",          Type = ObjectiveType.ReachLocation, TargetPosition = beach,     Radius = 40f },
                        new MissionObjective { Description = "Reach an altitude of 100m",                Type = ObjectiveType.AltitudeFlight, TargetPosition = new Vector3(0, 100f, 0) },
                    }
                },

                // ═══════════════════════════════════════════════════════════
                new MissionData {
                    MissionID = "M03_CoastalRescue",
                    Title     = "Coastal Rescue",
                    BriefingText = "Fishing boats are in distress near Karaikal Lighthouse. "
                                 + "Fly to the coastal rescue zone and use your powers to protect the civilians.",
                    CompletionText = "The fishermen are safe. The coast is clear.",
                    ReputationReward = 40,
                    State = MissionState.Locked,
                    RequiresSuperheroMode = true,
                    UnlocksMissionID = "M04_IndustrialCrisis",
                    Objectives = new List<MissionObjective> {
                        new MissionObjective { Description = "Fly to the coastal rescue zone",        Type = ObjectiveType.ReachLocation,   TargetPosition = beach + Vector3.forward * 200f, Radius = 50f },
                        new MissionObjective { Description = "Save 3 civilians in distress",           Type = ObjectiveType.ProtectCivilian, RequiredCount = 3 },
                        new MissionObjective { Description = "Return to shore",                        Type = ObjectiveType.ReachLocation,   TargetPosition = beach, Radius = 30f },
                    }
                },

                // ═══════════════════════════════════════════════════════════
                new MissionData {
                    MissionID = "M04_IndustrialCrisis",
                    Title     = "Industrial Crisis",
                    BriefingText = "An accident at the fictional CSVS Industrial Complex south of town "
                                 + "is threatening workers. Investigate and neutralize the hazard.",
                    CompletionText = "Crisis contained. The industrial zone is secure.",
                    ReputationReward = 50,
                    State = MissionState.Locked,
                    RequiresSuperheroMode = true,
                    UnlocksMissionID = "M05_CityGuardian",
                    Objectives = new List<MissionObjective> {
                        new MissionObjective { Description = "Fly to the CSVS Industrial Complex",    Type = ObjectiveType.ReachLocation, TargetPosition = industrialZone, Radius = 60f },
                        new MissionObjective { Description = "Destroy 5 hazard containers",            Type = ObjectiveType.DestroyTarget, RequiredCount = 5 },
                        new MissionObjective { Description = "Secure the perimeter",                  Type = ObjectiveType.SurviveTime,   TimeLimitSeconds = 30f },
                    }
                },

                // ═══════════════════════════════════════════════════════════
                new MissionData {
                    MissionID = "M05_CityGuardian",
                    Title     = "City Guardian",
                    BriefingText = "Rogue elements are terrorizing Pettai Bazaar. "
                                 + "Protect citizens and defeat all hostile opponents.",
                    CompletionText = "Pettai Bazaar is safe. The city knows its guardian.",
                    ReputationReward = 75,
                    State = MissionState.Locked,
                    RequiresSuperheroMode = true,
                    UnlocksMissionID = "M06_PowerUnleashed",
                    Objectives = new List<MissionObjective> {
                        new MissionObjective { Description = "Reach Pettai Bazaar",                  Type = ObjectiveType.ReachLocation, TargetPosition = townCenter, Radius = 50f },
                        new MissionObjective { Description = "Defeat 10 hostile combatants",          Type = ObjectiveType.DestroyTarget, RequiredCount = 10 },
                        new MissionObjective { Description = "Protect civilians (survival timer)",    Type = ObjectiveType.SurviveTime,   TimeLimitSeconds = 60f },
                    }
                },

                // ═══════════════════════════════════════════════════════════
                new MissionData {
                    MissionID = "M06_PowerUnleashed",
                    Title     = "Power Unleashed — Chaos Arena",
                    BriefingText = "Welcome to the Chaos Arena — a fictional combat training zone "
                                 + "outside Karaikal. Test every superpower without restraint.",
                    CompletionText = "All powers mastered. You are the guardian of Karaikal.",
                    ReputationReward = 100,
                    State = MissionState.Locked,
                    RequiresSuperheroMode = true,
                    UnlocksMissionID = "",
                    Objectives = new List<MissionObjective> {
                        new MissionObjective { Description = "Fly to the Chaos Arena",               Type = ObjectiveType.ReachLocation, TargetPosition = chaosArena, Radius = 60f },
                        new MissionObjective { Description = "Destroy 20 targets with any power",    Type = ObjectiveType.DestroyTarget, RequiredCount = 20 },
                        new MissionObjective { Description = "Complete the arena challenge",          Type = ObjectiveType.SurviveTime,   TimeLimitSeconds = 120f },
                    }
                },
            };
        }

        // ─── API ───────────────────────────────────────────────────────────
        public void StartMission(string missionID)
        {
            if (_isMissionActive)
            {
                Debug.LogWarning("[Mission] Already running a mission. Fail current first.");
                return;
            }
            var md = allMissions.Find(m => m.MissionID == missionID);
            if (md == null || md.State == MissionState.Locked)
            {
                Debug.LogWarning($"[Mission] {missionID} not available.");
                return;
            }
            ActiveMission = md;
            md.State = MissionState.Active;
            _isMissionActive = true;
            _missionCoroutine = StartCoroutine(RunMission(md));
        }

        private IEnumerator RunMission(MissionData md)
        {
            hudController?.ShowMissionStart(md.Title, md.BriefingText);
            OnMissionStarted?.Invoke(md);

            // Reset objective progress
            foreach (var obj in md.Objectives) { obj.Progress = 0; obj.IsComplete = false; }

            // Activate first waypoint
            int currentObjIdx = 0;
            SetWaypoint(md.Objectives[0].TargetPosition);
            hudController?.UpdateObjective(md.Objectives[0].Description);

            while (currentObjIdx < md.Objectives.Count)
            {
                var obj = md.Objectives[currentObjIdx];
                Vector3 playerPos = GetPlayerPosition();

                // Handle time-limited objectives
                if (obj.Type == ObjectiveType.SurviveTime && obj.TimeLimitSeconds > 0)
                {
                    float elapsed = 0f;
                    while (elapsed < obj.TimeLimitSeconds)
                    {
                        elapsed += Time.deltaTime;
                        hudController?.UpdateTimer(obj.TimeLimitSeconds - elapsed);
                        yield return null;
                    }
                    obj.IsComplete = true;
                }
                else
                {
                    obj.CheckComplete(playerPos);
                }

                if (obj.IsComplete)
                {
                    OnObjectiveCompleted?.Invoke(obj);
                    currentObjIdx++;
                    if (currentObjIdx < md.Objectives.Count)
                    {
                        SetWaypoint(md.Objectives[currentObjIdx].TargetPosition);
                        hudController?.UpdateObjective(md.Objectives[currentObjIdx].Description);
                    }
                }

                yield return null;
            }

            // Mission complete
            md.State = MissionState.Completed;
            _isMissionActive = false;
            hudController?.ShowMissionComplete(md.CompletionText, md.ReputationReward);
            OnMissionCompleted?.Invoke(md);

            // Unlock next mission
            if (!string.IsNullOrEmpty(md.UnlocksMissionID))
            {
                var next = allMissions.Find(m => m.MissionID == md.UnlocksMissionID);
                if (next != null) next.State = MissionState.Available;
            }

            // Apply reputation reward
            var rep = FindObjectOfType<ReputationChaosSystem>();
            if (rep) rep.AddReputation(md.ReputationReward);
        }

        public void FailActiveMission()
        {
            if (!_isMissionActive || ActiveMission == null) return;
            if (_missionCoroutine != null) StopCoroutine(_missionCoroutine);
            ActiveMission.State = MissionState.Failed;
            hudController?.ShowMissionFailed();
            OnMissionFailed?.Invoke(ActiveMission);
            _isMissionActive = false;
        }

        // Record kill/rescue for counting objectives
        public void RecordObjectiveProgress(ObjectiveType type, int amount = 1)
        {
            if (!_isMissionActive || ActiveMission == null) return;
            foreach (var obj in ActiveMission.Objectives)
            {
                if (!obj.IsComplete && obj.Type == type)
                {
                    obj.Progress += amount;
                    hudController?.UpdateObjectiveProgress(obj.Description, obj.Progress, obj.RequiredCount);
                    break;
                }
            }
        }

        // ─── Utility ───────────────────────────────────────────────────────
        private void SetWaypoint(Vector3 worldPos)
        {
            if (waypointMarker)
                waypointMarker.position = worldPos + Vector3.up * 2f;
        }

        private Vector3 GetPlayerPosition()
        {
            var hcc = FindObjectOfType<HumanoidCharacterController>();
            return hcc ? hcc.transform.position : Vector3.zero;
        }

        public List<MissionData> GetAvailableMissions()
            => allMissions.FindAll(m => m.State == MissionState.Available);

        public List<MissionData> GetCompletedMissions()
            => allMissions.FindAll(m => m.State == MissionState.Completed);
    }
}

