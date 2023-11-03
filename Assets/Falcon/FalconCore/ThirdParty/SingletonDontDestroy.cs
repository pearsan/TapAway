// using Falcon.FalconCore.Scripts.Utils;
// using UnityEngine;
//
// namespace Falcon.FalconCore.ThirdParty
// {
//     public class SingletonDontDestroy<T> : MonoBehaviour where T : MonoBehaviour
//     {
//         private static T _instance;
//         public static T Instance
//         {
//             get
//             {
//                 if (_instance == null)
//                 {
//                     GetInstance();
//                 }
//                 return _instance;
//             }
//         }
//     
//         private static void GetInstance()
//         {
//             FalconGameObjectUtils.TriggerSync(() =>
//             {
//                 _instance = (T)FindObjectOfType(typeof(T));
//                 if (_instance == null)
//                 {
//                     var falconGameObject = FalconGameObjectUtils.GameObject;
//                     _instance = falconGameObject.AddComponent<T>();
//                 }
//             });
//             
//         }
//
//         protected virtual void Awake()
//         {
//             try {
//                 if (_instance == null)
//                 {
//                     GetInstance();
//                     DontDestroyOnLoad(gameObject);
//                 }
//             }
//             catch (System.Exception e)
//             {
//                 FalconLogUtils.Error(e, "#ecbd77");
//             }
//         }
//
//     
//     }
// }
