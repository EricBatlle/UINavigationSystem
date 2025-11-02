using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace UINavigation
{
    [CreateAssetMenu(fileName = "ViewsContainer", menuName = "Scriptable Objects/ViewsContainer")]
    public class ViewsContainer : ScriptableObject
    {
        [SerializeField] 
        private List<ViewIdPrefabTuple> views;

        [CanBeNull]
        public GameObject GetViewPrefab(string viewId)
        {
            return views.FirstOrDefault(tuple => tuple.Id == viewId)?.Prefab;
        }
    }
}
