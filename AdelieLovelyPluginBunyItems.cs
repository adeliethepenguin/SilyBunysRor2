using BepInEx;
using IL.RoR2.Audio;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Reflection;
using RoR2.Stats;
using RiskOfOptions;
using BepInEx.Configuration;
using RiskOfOptions.Options;


// hi :D

namespace AdelieLovelyPluginBunyItems
{
    // This attribute specifies that we have a dependency on a given BepInEx Plugin,
    // We need the R2API ItemAPI dependency because we are using for adding our item to the game.
    // You don't need this if you're not using R2API in your plugin,
    // it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]

    // This one is because we use a .language file for language tokens
    // More info in https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Localization/
    [BepInDependency(LanguageAPI.PluginGUID)]

    // This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    [BepInDependency("com.rune580.riskofoptions")]

    // This is the main declaration of our plugin class.
    // BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    // BaseUnityPlugin itself inherits from MonoBehaviour,
    // so you can use this as a reference for what you can declare and use in your plugin class
    // More information in the Unity Docs: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class AdelieLovelyPluginBunyItems : BaseUnityPlugin
    {
        
        

        private AssetBundle bunyAssets;

        // The Plugin GUID should be a unique ID for this plugin,
        // which is human readable (as it is used in places like the config).
        // If we see this PluginGUID as it is on thunderstore,
        // we will deprecate this mod.
        // Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "AdelieThePenguin";
        public const string PluginName = "SilyBunys";
        public const string PluginVersion = "1.0.6";
        
        // We need our item definition to persist through our functions, and therefore make it a class field.
        //private static ItemDef myItemDef;

        private static ItemDef babyBuny;

        private static ItemDef coolBuny;

        private static ItemDef superBuny;

        private static ItemDef peeSyringe;

        public static BepInEx.PluginInfo PInfo { get; private set; }
        public ConfigEntry<bool> toggleBaby; 
        public ConfigEntry<bool> toggleCool; 
        public ConfigEntry<bool> toggleSuper; 

        // The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            PInfo = Info;
            Log.Init(Logger);

            toggleBaby = Config.Bind("baby buny spawning", "Item spawning setting for baby buny item", true, "toggle on if you want baby bunys to spawn baby bunys on the map. toggle off if you want it sent directly to your inventory. (less lag)");
            toggleCool = Config.Bind("Cool Buny spawning", "Item spawning setting for Cool Buny item", true, "toggle on if you want Cool Bunys to spawn baby bunys and Cool Bunys on the map. toggle off if you want it sent directly to your inventory. (less lag)");
            toggleSuper = Config.Bind("SUPER EPIC BUNY spawning", "Item spawning setting for Cool Buny item", false, "toggle on if you want SUPER EPIC BUNY to spawn bnuys instead of send straight to your inventory. If you do so you are insane maybe? (disabled by default)");
            ModSettingsManager.AddOption(new CheckBoxOption(toggleBaby));
            ModSettingsManager.AddOption(new CheckBoxOption(toggleCool));
            ModSettingsManager.AddOption(new CheckBoxOption(toggleSuper));

            babyBuny = ScriptableObject.CreateInstance<ItemDef>();
            coolBuny = ScriptableObject.CreateInstance<ItemDef>();
            superBuny = ScriptableObject.CreateInstance<ItemDef>();
            //myItemDef = ScriptableObject.CreateInstance<ItemDef>();

            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(PInfo.Location), "bunymodo");
            Logger.LogInfo("path is: " + path);

            bunyAssets = AssetBundle.LoadFromFile(path);
            if (!bunyAssets)
            {
                Logger.LogError($"Failed to load AssetBundle at path: {path}");
                return;
            }
            foreach (var s in bunyAssets.GetAllAssetNames())
                Logger.LogInfo("Bundle asset: " + s);
            babyBuny.pickupIconSprite = bunyAssets.LoadAsset<Sprite>("assets/assets for bunymod/babybunyicon.png");
            babyBuny.pickupModelPrefab = bunyAssets.LoadAsset<GameObject>("assets/assets for bunymod/babybunypref.prefab");
            coolBuny.pickupIconSprite = bunyAssets.LoadAsset<Sprite>("assets/assets for bunymod/coolbunyicon.png");
            coolBuny.pickupModelPrefab = bunyAssets.LoadAsset<GameObject>("assets/assets for bunymod/coolbunypref.prefab");
            superBuny.pickupIconSprite = bunyAssets.LoadAsset<Sprite>("assets/assets for bunymod/epicbunyicon.png");
            superBuny.pickupModelPrefab = bunyAssets.LoadAsset<GameObject>("assets/assets for bunymod/epicbunypref.prefab");


            Logger.LogInfo("SilyBunys asset bundle loaded successfully.");
            //myItemDef.pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
           // myItemDef.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();


            

            // Language Tokens, explained there https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/Assets/Localization/
            babyBuny.name = "ADELIETHEPENGUIN_BABYBUNY";
            babyBuny.nameToken = "BABYBUNY_NAME";
            babyBuny.pickupToken = "BABYBUNY_PICKUP";
            babyBuny.descriptionToken = "BABYBUNY_DESC";
            babyBuny.loreToken = "BABYBUNY_LORE";

            coolBuny.name = "ADELIETHEPENGUIN_COOLBUNY";
            coolBuny.nameToken = "COOLBUNY_NAME";
            coolBuny.pickupToken = "COOLBUNY_PICKUP";
            coolBuny.descriptionToken = "COOLBUNY_DESC";
            coolBuny.loreToken = "COOLBUNY_LORE";

            superBuny.name = "ADELIETHEPENGUIN_SUPERBUNY";
            superBuny.nameToken = "SUPERBUNY_NAME";
            superBuny.pickupToken = "SUPERBUNY_PICKUP";
            superBuny.descriptionToken = "SUPERBUNY_DESC";
            superBuny.loreToken = "SUPERBUNY_LORE";
           

            // The tier determines what rarity the item is:
            // Tier1=white, Tier2=green, Tier3=red, Lunar=Lunar, Boss=yellow,
            // and finally NoTier is generally used for helper items, like the tonic affliction
#pragma warning disable Publicizer001 // Accessing a member that was not originally public. Here we ignore this warning because with how this example is setup we are forced to do this
            //myItemDef._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/Tier2Def.asset").WaitForCompletion();

            babyBuny._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/Tier1Def.asset").WaitForCompletion();
            coolBuny._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/Tier2Def.asset").WaitForCompletion();
            superBuny._itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/Tier3Def.asset").WaitForCompletion();
#pragma warning restore Publicizer001
            // Instead of loading the itemtierdef directly, you can also do this like below as a workaround
            // myItemDef.deprecatedTier = ItemTier.Tier2;

            // You can create your own icons and prefabs through assetbundles, but to keep this boilerplate brief, we'll be using question marks.
            

            // Can remove determines
            // if a shrine of order,
            // or a printer can take this item,
            // generally true, except for NoTier items.
           // myItemDef.canRemove = true;
            babyBuny.canRemove = true;
            coolBuny.canRemove = true;
            superBuny.canRemove = true;


            // Hidden means that there will be no pickup notification,
            // and it won't appear in the inventory at the top of the screen.
            // This is useful for certain noTier helper items, such as the DrizzlePlayerHelper.
            //myItemDef.hidden = false;
            babyBuny.hidden = false;
            coolBuny.hidden = false;
            superBuny.hidden = false;

            // You can add your own display rules here,
            // where the first argument passed are the default display rules:
            // the ones used when no specific display rules for a character are found.
            // For this example, we are omitting them,
            // as they are quite a pain to set up without tools like https://thunderstore.io/package/KingEnderBrine/ItemDisplayPlacementHelper/
            var displayRules = new ItemDisplayRuleDict(null);

            // Then finally add it to R2API
            ItemAPI.Add(new CustomItem(babyBuny, displayRules));
            //ItemAPI.Add(new CustomItem(myItemDef, displayRules));
            ItemAPI.Add(new CustomItem(coolBuny, displayRules));
            ItemAPI.Add(new CustomItem(superBuny, displayRules));

            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            RecalculateStatsAPI.GetStatCoefficients += ItemStatCalculator;
        }

        private void ItemStatCalculator(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var supers = sender.inventory.GetItemCount(superBuny.itemIndex);
            var cools = sender.inventory.GetItemCount(coolBuny.itemIndex);
            var babys = sender.inventory.GetItemCount(babyBuny.itemIndex);
            if (sender.inventory.GetItemCount(superBuny.itemIndex) > 0 && babys>0)
            {
                args.baseHealthAdd += 1 * babys;
                args.baseHealthAdd += 1 * supers;
                args.baseHealthAdd += 1 * cools;
            }
        }


        private void GlobalEventManager_onServerDamageDealt(DamageReport report)
        {
            if (!report.attacker || !report.attackerBody)
            {
                return;
            }
            
            var attackerCharacterBody = report.attackerBody;

            if (attackerCharacterBody.inventory)
            {

                // Super bunny on-hit spawn bunny chance
                var superCount = attackerCharacterBody.inventory.GetItemCount(superBuny.itemIndex);
                if (superCount > 0 &&
                    // Roll for our 1% chance to spawn Baby bunny
                    Util.CheckRoll(1 * superCount, attackerCharacterBody.master))
                {
                    var transform = report.victimBody.transform;

                    if (!toggleSuper.Value)
                    {
                        attackerCharacterBody.inventory.GiveItem(babyBuny, 1);
                    }
                    else
                    {
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(babyBuny.itemIndex), transform.position, transform.forward * 20f);
                    }
                }
            }

        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport report)
        {
            // If a character was killed by the world, we shouldn't do anything.
            if (!report.attacker || !report.attackerBody)
            {
                return;
            }

            var attackerCharacterBody = report.attackerBody;

            // We need an inventory to do check for our item
            if (attackerCharacterBody.inventory)
            {
                //For the bunny items
                
                //BABY BUNNY
                var babyCount = attackerCharacterBody.inventory.GetItemCount(babyBuny.itemIndex);
                if (babyCount > 0 &&
                    // Roll for our 1% chance.
                    Util.CheckRoll(1*babyCount, attackerCharacterBody.master))
                {
                    var transform = report.victimBody.transform;

                    if (toggleBaby.Value)
                    {
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(babyBuny.itemIndex), transform.position, transform.forward * 20f);
                    }
                    else
                    {
                        attackerCharacterBody.inventory.GiveItem(babyBuny, 1);
                    }
                }



                //COOL BUNNY
                var coolCount = attackerCharacterBody.inventory.GetItemCount(coolBuny.itemIndex);
                if (coolCount > 0 &&
                    // Roll for our 10% chance to spawn Baby bunny
                    Util.CheckRoll(10 * coolCount, attackerCharacterBody.master))
                {
                    var transform = report.victimBody.transform;
                    if (toggleCool.Value)
                    {
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(babyBuny.itemIndex), transform.position, transform.forward * 20f);
                    }
                    else
                    {
                        attackerCharacterBody.inventory.GiveItem(coolBuny, 1);
                    }
                }
                if (coolCount > 0 &&
                    // Roll for our 1% chance.
                    Util.CheckRoll(1 * coolCount, attackerCharacterBody.master))
                {
                    var transform = report.victimBody.transform;
                    if(toggleCool.Value)
                    {
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(coolBuny.itemIndex), transform.position, transform.forward * 20f);
                    }
                    else
                    {
                        attackerCharacterBody.inventory.GiveItem(coolBuny, 1);
                    }
                }

                //SUPER BUNNY
                var superCount = attackerCharacterBody.inventory.GetItemCount(superBuny.itemIndex);
                if (superCount > 0)
                {
                    var transform = report.victimBody.transform;
                    for (int i = 0; i < superCount; i++)
                    {
                        if (!toggleSuper.Value)
                        {
                            attackerCharacterBody.inventory.GiveItem(babyBuny, 1);
                        }
                        else
                        {
                            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(babyBuny.itemIndex), transform.position, transform.forward * (20f + superCount));
                        }
                        
                    }
                }




                // Store the amount of our item we have
                /*var garbCount = attackerCharacterBody.inventory.GetItemCount(myItemDef.itemIndex);
                if (garbCount > 0 &&
                    // Roll for our 50% chance.
                    Util.CheckRoll(50, attackerCharacterBody.master))
                {
                    // Since we passed all checks, we now give our attacker the cloaked buff.
                    // Note how we are scaling the buff duration depending on the number of the custom item in our inventory.
                    attackerCharacterBody.AddTimedBuff(RoR2Content.Buffs.Cloak, 3 + garbCount);
                }*/

                
            }
        }

        // The Update() method is run on every frame of the game.
        /*
        private void Update()
        {

            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                // Get the player body to use a position:
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                // And then drop our defined item in front of the player.

                Log.Info($"Player pressed F2. Spawning our custom item at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(babyBuny.itemIndex), transform.position, transform.forward * 20f);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                // Get the player body to use a position:
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                // And then drop our defined item in front of the player.

                Log.Info($"Player pressed F2. Spawning our custom item at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(coolBuny.itemIndex), transform.position, transform.forward * 20f);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                // Get the player body to use a position:
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                // And then drop our defined item in front of the player.

                Log.Info($"Player pressed F2. Spawning our custom item at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(superBuny.itemIndex), transform.position, transform.forward * 20f);
            }
            
        }*/
    }
}
