using UnityEngine.Profiling;
using Unity.Profiling;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace ServerSDK.Common
{
    public static class Performance
    {
        private static ProfilerRecorder UpdateBehaviour;

        public static void Init()
        {
            //if (Debug.isDebugBuild)
            //{
            //    // https://docs.unity3d.com/ScriptReference/Unity.Profiling.ProfilerRecorder.StartNew.html
            //    UpdateBehaviour = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "BehaviourUpdate");
            //}
        }

        public static string GetMemroyUsageDesc()
        {
            string res = string.Format(@"Profiler
                    Memory Heap(MB): (total: {0} reserved: {1} unused: {2}) 
                    Memory Mono(MB): (total: {3} used: {4})",
                (float)Profiler.GetTotalReservedMemoryLong() / 1024 / 1024,
                (float)Profiler.GetTotalAllocatedMemoryLong() / 1024 / 1024,
                (float)Profiler.GetTotalUnusedReservedMemoryLong() / 1024 / 1024,
                (float)Profiler.GetMonoHeapSizeLong() / 1024 / 1024,
                (float)Profiler.GetMonoUsedSizeLong() / 1024 / 1024);

            //if (Debug.isDebugBuild)
            //{
            //    res += string.Format(@"
            //        Cost(MS): (behaviour update: {0})",
            //        UpdateBehaviour.CurrentValue / 100000f);
            //}

            return res;
        }

        public static int OnMessageCallCount = 0;
        public static int ReadFromByteCallCount = 0;
        public static int PackageCount = 0;
        public static int ProcessPackageCount = 0;
        public static int MaxPackageQueueCount = 0;
        public static int InvalidPackage = 0;
        public static int AvatarCreated = 0;
        public static int AvatarDeleted = 0;
        public static int AvatarGet = 0;
        public static int AvatarPut = 0;
        public static int SceneViewTaskCount = 0;

        private static readonly Dictionary<string, Timer> timers = new();
        public static void StartTimer(string name)
        {
            timers.Add(name, new Timer()
            {
                Name = name,
                StartTime = DateTime.Now.Ticks
            });
        }
        public static void EndTimer(string name)
        {
            if (!timers.TryGetValue(name, out Timer timer))
                return;

            timers.Remove(name);
            var nowt = DateTime.Now.Ticks;
            var cost = (nowt - timer.StartTime) / 10000;

            Debug.LogFormat("[Timer] {0} cost = {1} (MS)", name, cost);
        }
    }
    class Timer
    {
        public string Name;
        public long StartTime;
    }
}
