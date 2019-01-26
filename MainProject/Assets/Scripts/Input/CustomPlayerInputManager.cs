using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

partial class PlayerInputManager
{
	public const string BUTTON_SELECT = "BUTTON_SELECT";
	public const string BUTTON_BACK = "BUTTON_BACK";

	partial void SetCustomInputMapping()
	{
		// Remap the keyboard for player 1
		/*m_InputTypeMapping.Remove(InputType.KEYBOARD_PLAYER1);
		KeyCodeMapping keyboardMapping1 = new KeyCodeMapping();
		keyboardMapping1.AddMap(ButtonPS3.L1, "q");
		keyboardMapping1.AddMap(ButtonPS3.R1, "e");
		keyboardMapping1.AddMap(ButtonPS3.L1, "q");
		keyboardMapping1.AddMap(ButtonPS3.R1, "e");
		keyboardMapping1.AddMap(ButtonPS3.X, "space");
		keyboardMapping1.AddMap(ButtonPS3.Triangle, "h");
		keyboardMapping1.AddMap(ButtonPS3.Start, "return");
		keyboardMapping1.AddMap(ButtonPS3.ForceQuit, "escape");
		m_InputTypeMapping.Add(InputType.KEYBOARD_PLAYER1, keyboardMapping1);*/
	}
	
	partial void SetAliases()
	{
		AddAlias(BUTTON_SELECT, PlayerInputManager.ButtonPS3.X);
		AddAlias(BUTTON_BACK, PlayerInputManager.ButtonPS3.Circle);
	}
}
