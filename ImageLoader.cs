using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Image;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
#endif

namespace iruca.ImageLoader
{
#if UNITY_EDITOR && !COMPILER_UDONSHARP
    [Serializable]
    public class Images
    {
        [SerializeField] public string imageUrl;
        [SerializeField] public Material material;
    }

    [InitializeOnLoad]//エディター起動時にコンストラクタが呼ばれるように
    public static class PlayModeStateChanged
    {
        /// <summary>
        /// コンストラクタ(InitializeOnLoad属性によりエディター起動時に呼び出される)
        /// </summary>
        static PlayModeStateChanged()
        {
            //playModeStateChangedイベントにメソッド登録
            EditorApplication.playModeStateChanged += OnChangedPlayMode;
        }

        //プレイモードが変更された
        private static void OnChangedPlayMode(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                ImageLoader imageLoader = Resources.FindObjectsOfTypeAll<ImageLoader>()[0];
                foreach (var material in imageLoader.materials)
                {
                    if (material != null)
                    {
                        material.SetTexture("_MainTex", imageLoader.defaultTexture);
                    }
                }
            }
        }
    }
#endif

    [AddComponentMenu("iruca/ImageLoader")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ImageLoader : UdonSharpBehaviour
    {
        [SerializeField] VRCUrl[] urls;
        public Material[] materials;

        [SerializeField] UdonBehaviour udonBehaviour;

#if UNITY_EDITOR && !COMPILER_UDONSHARP
        [HideInInspector] public List<Images> images = new List<Images>();
        public Texture defaultTexture;

        private void OnValidate()
        {
            List<VRCUrl> urlList = new List<VRCUrl>();
            List<Material> materialList = new List<Material>();

            foreach (var item in images)
            {
                urlList.Add(new VRCUrl(item.imageUrl));
                materialList.Add(item.material);
            }
            urls = urlList.ToArray();
            materials = materialList.ToArray();
            foreach (var material in materials)
            {
                if(material != null)
                {
                    material.SetTexture("_MainTex", defaultTexture);
                }
            }
        }
#endif

        void Start()
        {
            VRCImageDownloader imageDownloader = new VRCImageDownloader();
            for (int i = 0; i < materials.Length; i++)
            {
                imageDownloader.DownloadImage(urls[i], materials[i], udonBehaviour != null ? udonBehaviour : (VRC.Udon.Common.Interfaces.IUdonEventReceiver)this);
            }
        }
    }
}
