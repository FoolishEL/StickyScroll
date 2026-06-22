using UnityEngine;

namespace Game.CMS
{
    public interface IContentManagementSystem
    {
        public T LoadPrefab<T>() where T : MonoBehaviour;
        public T LoadPrefab<T>(string id) where T : MonoBehaviour;
    }

}