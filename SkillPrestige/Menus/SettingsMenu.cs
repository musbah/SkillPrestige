﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.InputHandling;
using SkillPrestige.Logging;
using SkillPrestige.Menus.Elements.Buttons;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Menus
{
    /// <summary>
    /// Represents a menu where players can change their per-save settings.
    /// </summary>
    internal class SettingsMenu : IClickableMenu
    {
        private static bool _buttonClickRegistered;
        private int _debouceWaitTime;
        private bool _inputInitiated;

        private Checkbox _resetRecipesCheckbox;
        private IntegerEditor _tierOneCostEditor;
        private IntegerEditor _tierTwoCostEditor;

        public SettingsMenu(Rectangle bounds) : base(bounds.X, bounds.Y, bounds.Width, bounds.Height, true)
        {
            Logger.LogVerbose("New Settings Menu created.");

            exitFunction = DeregisterMouseEvents;
        }


        private void RegisterMouseEvents()
        {
            if (_buttonClickRegistered) return;
            _buttonClickRegistered = true;
            Logger.LogVerbose("Settings menu - Registering mouse events...");
            Mouse.MouseMoved += _resetRecipesCheckbox.CheckForMouseHover;
            Mouse.MouseClicked += _resetRecipesCheckbox.CheckForMouseClick;
            _tierOneCostEditor.RegisterMouseEvents();
            _tierTwoCostEditor.RegisterMouseEvents();
            Logger.LogVerbose("Settings menu - Mouse events registered.");
        }

        private void DeregisterMouseEvents()
        {
            Logger.LogVerbose("Settings menu - Deregistering mouse events...");
            if (!_buttonClickRegistered) return;
            Mouse.MouseMoved -= _resetRecipesCheckbox.CheckForMouseHover;
            Mouse.MouseClicked -= _resetRecipesCheckbox.CheckForMouseClick;
            _tierOneCostEditor.DeregisterMouseEvents();
            _tierTwoCostEditor.DeregisterMouseEvents();
            _buttonClickRegistered = false;
            Logger.LogVerbose("Settings menu - Mouse events deregistered.");
        }


        public override void receiveRightClick(int x, int y, bool playSound = true) { }

        private void InitiateInput()
        {
            if (_inputInitiated) return;
            _inputInitiated = true;
            Logger.LogVerbose("Settings menu - intiating input.");
            var resetRecipeCheckboxBounds = new Rectangle(xPositionOnScreen + spaceToClearSideBorder * 3, yPositionOnScreen + (int)Math.Floor(Game1.tileSize * 3.5), 9*Game1.pixelZoom, 9 * Game1.pixelZoom);
            _resetRecipesCheckbox = new Checkbox(PerSaveOptions.Instance.ResetRecipesOnPrestige, "Reset Recipes upon prestige.", resetRecipeCheckboxBounds, ChangeRecipeReset);
            var padding = 4*Game1.pixelZoom;
            var tierOneEditorLocation = new Vector2(resetRecipeCheckboxBounds.X, resetRecipeCheckboxBounds.Y + resetRecipeCheckboxBounds.Height + padding);
            _tierOneCostEditor = new IntegerEditor("Cost of Tier 1 Prestige", PerSaveOptions.Instance.CostOfTierOnePrestige, 1, 100, tierOneEditorLocation, ChangeTierOneCost);
            var tierTwoEditorLocation = tierOneEditorLocation;
            tierTwoEditorLocation.Y += _tierOneCostEditor.Bounds.Height + padding;
            _tierTwoCostEditor = new IntegerEditor("Cost of Tier 2 Prestige", PerSaveOptions.Instance.CostOfTierTwoPrestige, 1, 100, tierTwoEditorLocation, ChangeTierTwoCost);
        }

        private static void ChangeRecipeReset(bool resetRecipes)
        {
            PerSaveOptions.Instance.ResetRecipesOnPrestige = resetRecipes;
            PerSaveOptions.Save();
        }

        private static void ChangeTierOneCost(int cost)
        {
            PerSaveOptions.Instance.CostOfTierOnePrestige = cost;
            PerSaveOptions.Save();
        }

        private static void ChangeTierTwoCost(int cost)
        {
            PerSaveOptions.Instance.CostOfTierTwoPrestige = cost;
            PerSaveOptions.Save();
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            if (_debouceWaitTime < 10)
            {
                _debouceWaitTime++;
            }
            else
            {
                RegisterMouseEvents();
            }

            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            upperRightCloseButton?.draw(spriteBatch);
            DrawHeader(spriteBatch);
            if (!_inputInitiated) InitiateInput();
            _resetRecipesCheckbox.Draw(spriteBatch);
            _tierOneCostEditor.Draw(spriteBatch);
            _tierTwoCostEditor.Draw(spriteBatch);
            Mouse.DrawCursor(spriteBatch);
        }

        private void DrawHeader(SpriteBatch spriteBatch)
        {
            const string title = "Skill Prestige Settings";
            spriteBatch.DrawString(Game1.dialogueFont, title, new Vector2(xPositionOnScreen + width / 2 - Game1.dialogueFont.MeasureString(title).X / 2f, yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4), Game1.textColor);
            drawHorizontalPartition(spriteBatch, yPositionOnScreen + (int)Math.Floor(Game1.tileSize * 2.5));
        }
    }
}
