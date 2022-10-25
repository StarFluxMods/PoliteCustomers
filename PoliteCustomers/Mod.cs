using KitchenLib;
using KitchenLib.Event;
using KitchenData;
using KitchenLib.Utils;
using HarmonyLib;
using Kitchen;

namespace PoliteCustomers
{
    public class Mod : BaseMod
    {
        public Mod() : base("politecustomers", "1.0.5") {}

        public override void OnInitializeMelon()
        {
            Events.BuildGameDataEvent += (s, args) =>
            {
                foreach (Item item in args.gamedata.Get<Item>())
                {
                    if (item.DirtiesTo != null)
                        if (item.DirtiesTo.ID == GDOUtils.GetExistingGDO(1517992271).ID)
                            item.DirtiesTo = (Item)GDOUtils.GetExistingGDO(793377380);
                }
            };
        }
    }
    
    [HarmonyPatch(typeof(CreateNewMesses), "FindMessLocation")]
    class CreateNewMesses_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            __result = false;
        }
    }
}
