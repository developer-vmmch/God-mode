using UnityEngine;

namespace CSVS.GodMode.Environment
{
    /// <summary>Runtime component attached to every landmark GameObject for GPS / minimap queries.</summary>
    public class LandmarkData : MonoBehaviour
    {
        public string LandmarkName;
        public string Category;
        public bool IsRealWorldSite;
        public bool IsPlayable;
    }
}

