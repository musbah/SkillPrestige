﻿using System;
using System.IO;
using JsonNet.PrivateSettersContractResolvers;
using Newtonsoft.Json;
using SkillPrestige.Logging;

namespace SkillPrestige
{
    /// <summary>
    /// Represents options for this mod per save file.
    /// </summary>
    [Serializable]
    public class PerSaveOptions
    {
        /// <summary>
        /// Whether or not to reset the recipes of a skill on load.
        /// </summary>
        public bool ResetRecipesOnPrestige { get; set; }

        public int CostOfTierOnePrestige { get; set; }

        public int CostOfTierTwoPrestige { get; set; }

        private PerSaveOptions() { }
        private static PerSaveOptions _instance;

        public static PerSaveOptions Instance 
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new PerSaveOptions();
                LoadPerSaveOptions();
                return _instance;
            }
        }

        private static void LoadPerSaveOptions()
        {
            Logger.LogInformation($"per save options file path: {SkillPrestigeMod.CurrentSaveOptionsPath}");
            if (!File.Exists(SkillPrestigeMod.CurrentSaveOptionsPath)) SetupPerSaveOptionsFile();
            var settings = new JsonSerializerSettings { ContractResolver = new PrivateSetterContractResolver() };
            Logger.LogInformation("Deserializing per save options file...");
            try
            {
                _instance = JsonConvert.DeserializeObject<PerSaveOptions>(File.ReadAllText(SkillPrestigeMod.CurrentSaveOptionsPath), settings);
                if (Instance.CostOfTierOnePrestige <= 0)
                {
                    Logger.LogWarning("Tier one prestige cost loaded without value, defaulting to a cost of 1.");
                    Instance.CostOfTierOnePrestige = 1;
                }
                if (Instance.CostOfTierTwoPrestige <= 0)
                {
                    Logger.LogWarning("Tier two prestige cost loaded without value, defaulting to a cost of 2.");
                    Instance.CostOfTierTwoPrestige = 2;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error deserializing per-save options file. {Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}"); 
                Logger.LogInformation(" Attempting to create new per-save options file...");
                SetupPerSaveOptionsFile();
            }
            Logger.LogInformation("Per save options loaded.");
        }

        private static void SetupPerSaveOptionsFile()
        {
            Logger.LogInformation("Creating new options file...");
            try
            {
                Instance.ResetRecipesOnPrestige =  true;
                Instance.CostOfTierOnePrestige =  1;
                Instance.CostOfTierTwoPrestige =  2;
                Save();
            }
            catch(Exception exception)
            {
                Logger.LogError($"Error while attempting to create a per save options file. {Environment.NewLine} {exception}");
                throw;
            }
            Logger.LogInformation("Successfully created new per save options file.");
        }

        public static void Save()
        {
            Directory.CreateDirectory(SkillPrestigeMod.PerSaveOptionsDirectory);
            File.WriteAllLines(SkillPrestigeMod.CurrentSaveOptionsPath, new[] { JsonConvert.SerializeObject(_instance) });
            Logger.LogInformation("Per save options file saved.");
        }

        /// <summary>
        /// Empty procedure to force the lazy load of the instance.
        /// </summary>
        public void Check() { }
    }
}