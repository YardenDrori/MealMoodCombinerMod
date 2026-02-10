using RimWorld;
using Verse;

namespace MealMoodCombiner
{
  public class Thought_MealCombined : Thought_Memory
  {
    private string customLabel;
    private string customDescription;

    public override string LabelCap => customLabel ?? base.LabelCap;

    public override string Description => customDescription ?? base.Description;

    public void SetCustomData(PendingThoughtData.CustomData data)
    {
      customLabel = data.Label;
      customDescription = data.Description;
      moodOffset = (int)(data.MoodOffset);
    }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Values.Look(ref customLabel, "customLabel");
      Scribe_Values.Look(ref customDescription, "customDescription");
    }
  }
}
