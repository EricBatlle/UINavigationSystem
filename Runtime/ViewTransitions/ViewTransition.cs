using Cysharp.Threading.Tasks;

namespace UINavigation
{
    using UnityEngine;

    public abstract class ViewTransition : MonoBehaviour
    {
        public abstract UniTask Animate(Transform target);
    }
}
