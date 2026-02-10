using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MealMoodCombiner;

[HarmonyPatch(typeof(FoodUtility), "ThoughtsFromIngesting")]
public static class ThoughtsFromIngesting_MergeMoodPatch
{
  [HarmonyPostfix]
  public static void ThoughtsFromIngesting_Postfix(
    Pawn ingester,
    Thing foodSource,
    ThingDef foodDef,
    ref List<FoodUtility.ThoughtFromIngesting> __result
  )
  {
    if (__result.NullOrEmpty() || __result.Count == 1)
      return;

    HashSet<ThoughtDef> moodOffsetsApplied = new();

    string label = "ate meal";
    string desc = "ate meal desc";
    float mealMoodOffset = 0;
    if (foodDef.ingestible.tasteThought != null)
    {
      //TODO: add support for stages??
      label = $"{foodDef.ingestible.tasteThought.stages[0].label}";
      mealMoodOffset = foodDef.ingestible.tasteThought.stages[0].baseMoodEffect;
      desc = $"{foodDef.ingestible.tasteThought.stages[0].description} ({mealMoodOffset})\n\n\n";
      moodOffsetsApplied.Add(foodDef.ingestible.tasteThought);
    }
    else
    {
      //TODO: add support for stages??
      label = $"ate {foodDef.label}";
      desc = $"";
    }

    float totalMoodOffset = 0;
    Precept precept = null;
    foreach (var mood in __result)
    {
      if (moodOffsetsApplied.Contains(mood.thought))
        continue;

      moodOffsetsApplied.Add(mood.thought);

      //TODO: add support for stages??
      totalMoodOffset += mood.thought.stages[0].baseMoodEffect;
      if (precept == null && mood.fromPrecept != null)
      {
        precept = mood.fromPrecept;
      }

      desc +=
        $"{mood.thought.stages[0].label}:\n{mood.thought.stages[0].description} ({mood.thought.stages[0].baseMoodEffect})\n\n";
    }
    desc = desc.TrimEnd('\n');
    label = label.TrimEnd('\n');

    if (mealMoodOffset < totalMoodOffset)
    {
      label += " (enhanced)";
    }
    else if (mealMoodOffset > totalMoodOffset)
    {
      label += " (diminished)";
    }

    __result.Clear();
    __result.Add(
      new FoodUtility.ThoughtFromIngesting
      {
        thought = ThoughtDefOf.MealMoodCombined,
        fromPrecept = precept,
      }
    );

    PendingThoughtData.Store(ingester, label, desc, totalMoodOffset);

    return;
  }
}

public static class PendingThoughtData
{
  private static Dictionary<Pawn, CustomData> pendingData = new Dictionary<Pawn, CustomData>();

  public class CustomData
  {
    public string Label;
    public string Description;
    public float MoodOffset;
    public int TickAdded;
  }

  public static void Store(Pawn pawn, string label, string desc, float mood)
  {
    pendingData[pawn] = new CustomData
    {
      Label = label,
      Description = desc,
      MoodOffset = mood,
      TickAdded = Find.TickManager.TicksGame,
    };
  }

  public static CustomData GetAndRemove(Pawn pawn)
  {
    if (pendingData.TryGetValue(pawn, out var data))
    {
      pendingData.Remove(pawn);
      return data;
    }
    return null;
  }
}
