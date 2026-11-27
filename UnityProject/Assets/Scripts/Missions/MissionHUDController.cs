// MissionHUDController.cs  — stub referenced by MissionSystem
using UnityEngine;
namespace CSVS.GodMode.Missions
{
    public class MissionHUDController : MonoBehaviour
    {
        public void ShowMissionStart(string title, string briefing) =>
            Debug.Log($"[Mission HUD] STARTED: {title}\n{briefing}");
        public void ShowMissionComplete(string text, int rep) =>
            Debug.Log($"[Mission HUD] COMPLETE: {text}  (+{rep} REP)");
        public void ShowMissionFailed() =>
            Debug.Log("[Mission HUD] FAILED");
        public void UpdateObjective(string desc) =>
            Debug.Log($"[Mission HUD] Objective: {desc}");
        public void UpdateObjectiveProgress(string desc, int prog, int max) =>
            Debug.Log($"[Mission HUD] {desc}: {prog}/{max}");
        public void UpdateTimer(float remaining) { /* update HUD countdown */ }
    }
}

