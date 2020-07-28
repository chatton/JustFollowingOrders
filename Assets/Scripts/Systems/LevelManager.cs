using System;
using Core;
using Util;

namespace Systems
{
    public class LevelManager : Singleton<LevelManager>
    {
        public bool HasKey { get; set; }

        // private Chest _chest;
        //
        // private void Awake()
        // {
        //     _chest = FindObjectOfType<Chest>();
        // }
    }
}