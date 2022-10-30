using KitchenLib;
using KitchenLib.Event;
using KitchenData;
using KitchenLib.Utils;
using HarmonyLib;
using Kitchen;
using Kitchen.Modules;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace PoliteCustomers
{
    public class Mod : BaseMod
    {
        public Mod() : base("politecustomers", "1.0.5") {}

        private List<int> dirtyDishes = new List<int>();

        public override void OnInitializeMelon()
        {
            //Registering Preferences
            KitchenLib.BoolPreference cleanplates = PreferenceUtils.Register<KitchenLib.BoolPreference>("politecustomers", "cleanplates", "Clean Plates");
            KitchenLib.BoolPreference nomesss = PreferenceUtils.Register<KitchenLib.BoolPreference>("politecustomers", "nomesss", "No Messes");
            
            //Setting Preference Defaults
            cleanplates.Value = true;
            nomesss.Value = true;

            //Loading Preferences File
            PreferenceUtils.Load();

            //Setting Up Preferences Menus
            SetupSettings();


            Events.BuildGameDataEvent += (s, args) =>
            {
                bool customersCleanPlates = PreferenceUtils.Get<KitchenLib.BoolPreference>("politecustomers", "cleanplates").Value;
                foreach (Item item in args.gamedata.Get<Item>())
                {
                    if (item.DirtiesTo != null)
                    {
                        if (item.DirtiesTo.ID == GDOUtils.GetExistingGDO(1517992271).ID)
                        {
                            dirtyDishes.Add(item.ID);
                            if (customersCleanPlates)
                                item.DirtiesTo = (Item)GDOUtils.GetExistingGDO(793377380);
                        }
                    }
                }
            };

            
            Events.PreferencesSaveEvent += (s, args) =>
            {
                bool customersCleanPlates = PreferenceUtils.Get<KitchenLib.BoolPreference>("politecustomers", "cleanplates").Value;
                foreach (Item item in GameData.Main.Get<Item>())
                {
                    if (dirtyDishes.Contains(item.ID))
                    {
                        Mod.Log("Updating: " + item.name);
                        if (customersCleanPlates)
                            item.DirtiesTo = (Item)GDOUtils.GetExistingGDO(793377380);
                        else
                            item.DirtiesTo = (Item)GDOUtils.GetExistingGDO(1517992271);   
                    }
                }
            };
            
        }

        private void SetupSettings()
        {
			//Setting Up For Main Menu
			Events.PreferenceMenu_MainMenu_SetupEvent += (s, args) =>
			{
				Type type = args.instance.GetType().GetGenericArguments()[0];
				args.mInfo.Invoke(args.instance, new object[] { "Polite Customers", typeof(PoliteCustomersSettings<>).MakeGenericType(type), false });
			};

			Events.PreferenceMenu_MainMenu_CreateSubmenusEvent += (s, args) =>
			{
				args.Menus.Add(typeof(PoliteCustomersSettings<MainMenuAction>), new PoliteCustomersSettings<MainMenuAction>(args.Container, args.Module_list));
			};

			//Setting Up For Pause Menu
			Events.PreferenceMenu_PauseMenu_SetupEvent += (s, args) =>
			{
				Type type = args.instance.GetType().GetGenericArguments()[0];
				args.mInfo.Invoke(args.instance, new object[] { "Polite Customers", typeof(PoliteCustomersSettings<>).MakeGenericType(type), false });
			};
			Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) =>
			{
				args.Menus.Add(typeof(PoliteCustomersSettings<PauseMenuAction>), new PoliteCustomersSettings<PauseMenuAction>(args.Container, args.Module_list));
			};
			
        }
    }

    [HarmonyPatch(typeof(CreateNewMesses), "FindMessLocation")]
    class CreateNewMesses_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            if (PreferenceUtils.Get<KitchenLib.BoolPreference>("politecustomers","nomesss").Value == true)
                __result = false;
        }
    }

    public class PoliteCustomersSettings<T> : KLMenu<T>
    {
	    public PoliteCustomersSettings(Transform container, ModuleList module_list) : base(container, module_list)
	    {
	    }

	    public override void Setup(int player_id)
	    {

            AddLabel("Clean Plates");
            BoolOption(PreferenceUtils.Get<KitchenLib.BoolPreference>("politecustomers","cleanplates"));
            AddLabel("No Messes");
            BoolOption(PreferenceUtils.Get<KitchenLib.BoolPreference>("politecustomers","nomesss"));


            New<SpacerElement>();
		    New<SpacerElement>();

		    AddButton("Apply", delegate
		    {
                PreferenceUtils.Save();
		    });
			
            AddButton(base.Localisation["MENU_BACK_SETTINGS"], delegate
		    {
			    RequestPreviousMenu();
		    });
	    } 
    }
}
