using System;
using System.Threading;
using TerrainCalculator.OptionsFramework;

namespace TerrainCalculator
{
    public static class TerrainCalculatorHook
    {
        private static readonly object ClassLock = new object();

        private static volatile int _currentThread = -1;
        public static void ImUpToNoGood(string modName)
        {
            UnityEngine.Debug.Log($"Terrain Calculator - enable prevent props & trees from disappearing ({modName})");
            lock (ClassLock)
            {
                if (_currentThread != -1)
                {
                    UnityEngine.Debug.LogError("Terrain Calculator - TerrainCalculatorHook::ImUpToNoGood() - _currentThread wasn't null");
                    throw new Exception("Some other code is already using Terrain Calculator hook. Make sure all calls happen in the simulation thread");
                }
                _currentThread = Thread.CurrentThread.ManagedThreadId;
                DetoursManager.Deploy(false);
            }
        }

        public static void MischiefManaged(string modName)
        {
            UnityEngine.Debug.Log($"Terrain Calculator - disable prevent props & trees from disappearing ({modName})");
            lock (ClassLock)
            {
                if (_currentThread != Thread.CurrentThread.ManagedThreadId)
                {
                    UnityEngine.Debug.LogError("Terrain Calculator - TerrainCalculatorHook::MischiefManaged() - current thread no equal to _currentThread");
                    throw new Exception("Some other code is already using Terrain Calculator hook. Make sure all calls happen in the simulation thread");
                }
                if (!DetoursManager.GetCachedDeployedState())
                {
                    DetoursManager.Revert(false);
                }
                _currentThread = -1;
            }
        }

    }
}