using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace NPCsPathfindThroughWindows;

[BepInPlugin("denyscrasav4ik.thedumbfactory.npcspathfindthroughwindows", "NPCs Pathfind Through Windows", "1.0.0")]
public class PathfindPlugin : BaseUnityPlugin
{
    private void Awake() => new Harmony("denyscrasav4ik.thedumbfactory.npcspathfindthroughwindows").PatchAll();
}

[HarmonyPatch(typeof(NPC))]
internal static class Patches
{
    [HarmonyPatch("Initialize")]
    [HarmonyPostfix]
    private static void Initialize_Postfix(NPC __instance)
    {
        if (__instance.Navigator != null && !__instance.Navigator.passableObstacles.Contains(PassableObstacle.BreakableWindow))
            __instance.Navigator.passableObstacles.Add(PassableObstacle.BreakableWindow);

        if (__instance.looker != null)
        {
            foreach (var window in Object.FindObjectsOfType<Window>())
            {
                if (window.colliders != null)
                {
                    foreach (var col in window.colliders)
                    {
                        if (col != null)
                            __instance.looker.IgnoreTransform(col.transform);
                    }
                }
            }
        }
    }

    [HarmonyPatch("WindowHit")]
    [HarmonyPrefix]
    private static bool WindowHit_Prefix(NPC __instance, Window window)
    {
        bool isBroken = (bool)AccessTools.Field(typeof(Window), "broken").GetValue(window);
        if (!isBroken)
            window.Break(true);

        var npcCollider = __instance.GetComponent<Collider>();
        if (npcCollider != null && window.colliders != null)
        {
            foreach (var winCol in window.colliders)
            {
                if (winCol != null)
                    Physics.IgnoreCollision(npcCollider, winCol, true);
            }
        }

        return false;
    }
}
