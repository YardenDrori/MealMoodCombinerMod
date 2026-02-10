using HarmonyLib;
using RimWorld;
using Verse;

namespace MealMoodCombiner
{
  [HarmonyPatch(
    typeof(MemoryThoughtHandler),
    "TryGainMemory",
    new[] { typeof(Thought_Memory), typeof(Pawn) }
  )]
  public static class TryGainMemory_Patch
  {
    [HarmonyPrefix]
    public static void Prefix(MemoryThoughtHandler __instance, Thought_Memory newThought)
    {
      if (newThought.def != ThoughtDefOf.MealMoodCombined)
        return;

      PendingThoughtData.CustomData memoryData = PendingThoughtData.GetAndRemove(__instance.pawn);
      if (memoryData == null)
      {
        Log.Error($"Failed to fetch custom memoryData for {__instance.pawn.Name}");
        return;
      }

      Thought_MealCombined thought = newThought as Thought_MealCombined;
      if (thought == null)
      {
        Log.Error($"Failed to convert {newThought.def.defName} to Thought_MealCombined");
        return;
      }

      thought.SetCustomData(memoryData);
    }
  }
}
