using HarmonyLib;
using Verse;

namespace MealMoodCombiner
{
  public class MealMoodCombinerMod : Mod
  {
    public MealMoodCombinerMod(ModContentPack content)
      : base(content)
    {
      var harmony = new Harmony("blacksparrow.mealmoodcombiner");
      harmony.PatchAll();
    }
  }
}
