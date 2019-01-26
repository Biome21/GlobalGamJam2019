using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public partial class PlayerInputManager
{	
	private static PlayerInputManager m_Instance;
	public static PlayerInputManager Instance	
	{
		get
		{
			if (m_Instance == null)
				m_Instance = new PlayerInputManager();
				
			return m_Instance;
		}
	}
	
	// List that contains an alias for certain ButtonId
	private Dictionary<string, ButtonId> m_ButtonAliases = new Dictionary<string, ButtonId>();		
	
	// List for the mapping of every input type
	protected Dictionary<InputType, KeyCodeMapping> m_InputTypeMapping = new Dictionary<InputType, KeyCodeMapping>();			
	
	// List for the input type of the players
	private Dictionary<ControllerInput, InputType> m_PlayerInputTypes = new Dictionary<ControllerInput, InputType>();
	
	#region Enums
	public enum ControllerInput
	{
		Controller1 = 1,
		Controller2,
		Controller3,
		Controller4,
		Controller5,
		Controller6,
		Keyboard1,
		Keyboard2,
		Keyboard3,
		Keyboard4,
		Mouse1,
		None
	}
	
	public enum InputType
	{
		PS3_CONTROLLER,
		XBOX360_CONTROLLER,
		GENERIC_CONTROLLER,
		MOTIONINJOY_PS3_CONTROLLER,
		SNES_CONTROLLER,
		LOGITECH_WINGMAN_CONTROLLER,
		KEYBOARD_PLAYER1,
		KEYBOARD_PLAYER2,
		KEYBOARD_PLAYER3,
		KEYBOARD_PLAYER4,
		MOUSE1,
		XBOX360_MAC_CONTROLLER
	}
	
	public enum ButtonId
	{
		Button0 = 0,
		Button1,
		Button2,
		Button3,
		Button4,
		Button5,
		Button6,
		Button7,
		Button8,
		Button9,
		Button10,
		Button11,
		Button12,
		Button13,
		Button14,
		Button15,
		Button16,
		Button17,
		Button18,
		Button19,
		Up,
		Down,
		Left,
		Right,
		ForceQuit
	}
	
	public enum ButtonPS3
	{
		X = 0,
		Circle,
		Square,
		Triangle,
		Select,
		Start,
		L1,
		R1,
		L2,
		R2,
		PS,
		Button11,
		Button12,
		Button13,
		Button14,
		Button15,
		Button16,
		Button17,
		Button18,
		Button19,
		Up,
		Down,
		Left,
		Right,
		ForceQuit
	}
	
	public enum ButtonXBOX360
	{
		A = 0,
		B,
		X,
		Y,
		Back,
		Start,
		LBump,
		RBump,
		Button8,
		Button9,
		Button10,
		Button11,
		Button12,
		Button13,
		Button14,
		Button15,
		Button16,
		Button17,
		Button18,
		Button19,
		Up,
		Down,
		Left,
		Right,
		ForceQuit
	}
	
	public enum ButtonSNES
	{
		B = 0,
		A,
		Y,
		X,
		Select,
		Start,
		L,
		R,
		Button8,
		Button9,
		Button10,
		Button11,
		Button12,
		Button13,
		Button14,
		Button15,
		Button16,
		Button17,
		Button18,
		Button19,
		Up,
		Down,
		Left,
		Right,
		ForceQuit
	}
	
	public enum ButtonLogitechWingMan
	{
		A = 0,
		B,
		X,
		Y,
		Button4,
		Start,
		L,
		R,
		C,
		Z,
		Button10,
		Button11,
		Button12,
		Button13,
		Button14,
		Button15,
		Button16,
		Button17,
		Button18,
		Button19,
		Up,
		Down,
		Left,
		Right,
		ForceQuit
	}
	#endregion
	
	protected PlayerInputManager()
	{
		// Initialize the players
		string[] joysticks =  Input.GetJoystickNames();
		for (int i = 0; i < joysticks.Length; ++i) 
		{
			InputType inputType = InputType.GENERIC_CONTROLLER;
			if (joysticks[i].Contains("PLAYSTATION(R)3"))
				inputType = InputType.PS3_CONTROLLER;
			else if (joysticks[i].Contains("360"))					// Not sure yet??
				inputType = InputType.XBOX360_CONTROLLER;
			else if (joysticks[i].Contains("MotioninJoy"))
				inputType = InputType.MOTIONINJOY_PS3_CONTROLLER;
			else if (joysticks[i].Contains("2Axes 11Keys"))
				inputType = InputType.SNES_CONTROLLER;
			else if (joysticks[i].Contains("WingMan"))
				inputType = InputType.LOGITECH_WINGMAN_CONTROLLER;
			else if (joysticks[i] == "")
				inputType = InputType.XBOX360_MAC_CONTROLLER;
			
			Debug.Log("Player " + (i + 1) + " uses " + inputType + ": " + joysticks[i]);
			m_PlayerInputTypes.Add((ControllerInput)(i + 1), inputType);
		}
		m_PlayerInputTypes.Add(ControllerInput.Keyboard1, InputType.KEYBOARD_PLAYER1);
		m_PlayerInputTypes.Add(ControllerInput.Keyboard2, InputType.KEYBOARD_PLAYER2);
		m_PlayerInputTypes.Add(ControllerInput.Keyboard3, InputType.KEYBOARD_PLAYER3);
		m_PlayerInputTypes.Add(ControllerInput.Keyboard4, InputType.KEYBOARD_PLAYER4);
		m_PlayerInputTypes.Add(ControllerInput.Mouse1, InputType.MOUSE1);
		
		Init();
	}
	
	~PlayerInputManager()
	{
	}
	
	private void Init()
	{
		// Map the PS3 controller
		KeyCodeMapping ps3Mapping = new KeyCodeMapping();
		ps3Mapping.AddMap(ButtonPS3.X, "joystick# button 14");		
		ps3Mapping.AddMap(ButtonPS3.Circle, "joystick# button 13");	
		ps3Mapping.AddMap(ButtonPS3.Square, "joystick# button 15");	
		ps3Mapping.AddMap(ButtonPS3.Triangle, "joystick# button 12");
		ps3Mapping.AddMap(ButtonPS3.Select, "joystick# button 0");
		ps3Mapping.AddMap(ButtonPS3.Start, "joystick# button 3");
		ps3Mapping.AddMap(ButtonPS3.L1, "joystick# button 10");
		ps3Mapping.AddMap(ButtonPS3.R1, "joystick# button 11");
		ps3Mapping.AddMap(ButtonPS3.L2, "joystick# button 8");
		ps3Mapping.AddMap(ButtonPS3.R2, "joystick# button 9");
		ps3Mapping.AddMap(ButtonPS3.PS, "joystick# button 16");
		m_InputTypeMapping.Add(InputType.PS3_CONTROLLER, ps3Mapping);
		
		// Map the XBOX 360 controller
		KeyCodeMapping xbox360Mapping = new KeyCodeMapping();
#if UNITY_STANDALONE_OSX
		xbox360Mapping.AddMap(ButtonXBOX360.A, "joystick# button 16");		
		xbox360Mapping.AddMap(ButtonXBOX360.B, "joystick# button 17");	
		xbox360Mapping.AddMap(ButtonXBOX360.X, "joystick# button 18");
		xbox360Mapping.AddMap(ButtonXBOX360.Y, "joystick# button 19");
		xbox360Mapping.AddMap(ButtonXBOX360.Start, "joystick# button 9");
		xbox360Mapping.AddMap(ButtonXBOX360.LBump, "joystick# button 13");
		xbox360Mapping.AddMap(ButtonXBOX360.RBump, "joystick# button 14");
#else
		xbox360Mapping.AddMap(ButtonXBOX360.A, "joystick# button 0");		
		xbox360Mapping.AddMap(ButtonXBOX360.B, "joystick# button 1");	
		xbox360Mapping.AddMap(ButtonXBOX360.X, "joystick# button 2");
		xbox360Mapping.AddMap(ButtonXBOX360.Y, "joystick# button 3");
		xbox360Mapping.AddMap(ButtonXBOX360.Start, "joystick# button 7");
		xbox360Mapping.AddMap(ButtonXBOX360.LBump, "joystick# button 4");
		xbox360Mapping.AddMap(ButtonXBOX360.RBump, "joystick# button 5");
		/*motionInjoyMapping.AddMap(ButtonPS3.L2, "joystick# button 8"); // TODO: L2 and R2 are considerated like Axises.
		motionInjoyMapping.AddMap(ButtonPS3.R2, "joystick# button 9");*/
#endif
		m_InputTypeMapping.Add(InputType.XBOX360_CONTROLLER, xbox360Mapping);
		
		KeyCodeMapping xbox360MacMapping = new KeyCodeMapping();
		xbox360MacMapping.AddMap(ButtonXBOX360.A, "joystick# button 16");		
		xbox360MacMapping.AddMap(ButtonXBOX360.B, "joystick# button 17");	
		xbox360MacMapping.AddMap(ButtonXBOX360.X, "joystick# button 18");
		xbox360MacMapping.AddMap(ButtonXBOX360.Y, "joystick# button 19");
		xbox360MacMapping.AddMap(ButtonXBOX360.Start, "joystick# button 9");
		xbox360MacMapping.AddMap(ButtonXBOX360.LBump, "joystick# button 13");
		xbox360MacMapping.AddMap(ButtonXBOX360.RBump, "joystick# button 14");
		m_InputTypeMapping.Add(InputType.XBOX360_MAC_CONTROLLER, xbox360MacMapping);
		
		// Map the generic controller
		KeyCodeMapping genericMapping = new KeyCodeMapping();
		genericMapping.AddMap(ButtonId.Button0, "joystick# button 0");		
		genericMapping.AddMap(ButtonId.Button1, "joystick# button 1");	
		genericMapping.AddMap(ButtonId.Button2, "joystick# button 3");	
		genericMapping.AddMap(ButtonId.Button3, "joystick# button 4");
		genericMapping.AddMap(ButtonId.Button5, "joystick# button 9");
		genericMapping.AddMap(ButtonId.Button6, "joystick# button 6");
		genericMapping.AddMap(ButtonId.Button7, "joystick# button 7");
		genericMapping.AddMap(ButtonId.Button4, "joystick# button 8");
		genericMapping.AddMap(ButtonId.Button8, "joystick# button 2");
		genericMapping.AddMap(ButtonId.Button9, "joystick# button 5");
		m_InputTypeMapping.Add(InputType.GENERIC_CONTROLLER, genericMapping);
		
		// Map the MotionInjoy controller
		KeyCodeMapping motionInjoyMapping = new KeyCodeMapping();
		motionInjoyMapping.AddMap(ButtonPS3.X, "joystick# button 2");		
		motionInjoyMapping.AddMap(ButtonPS3.Circle, "joystick# button 1");	
		motionInjoyMapping.AddMap(ButtonPS3.Square, "joystick# button 3");	
		motionInjoyMapping.AddMap(ButtonPS3.Triangle, "joystick# button 0");
		motionInjoyMapping.AddMap(ButtonPS3.Select, "joystick# button 8");
		motionInjoyMapping.AddMap(ButtonPS3.Start, "joystick# button 9");
		motionInjoyMapping.AddMap(ButtonPS3.L1, "joystick# button 6");
		motionInjoyMapping.AddMap(ButtonPS3.R1, "joystick# button 7");
		/*motionInjoyMapping.AddMap(ButtonPS3.L2, "joystick# button 8"); // TODO: L2 and R2 are considerated like Axises.
		motionInjoyMapping.AddMap(ButtonPS3.R2, "joystick# button 9");*/
		motionInjoyMapping.AddMap(ButtonPS3.PS, "joystick# button 12");
		m_InputTypeMapping.Add(InputType.MOTIONINJOY_PS3_CONTROLLER, motionInjoyMapping);
		
		// Map the SNES controller
		KeyCodeMapping snesMapping = new KeyCodeMapping();
		snesMapping.AddMap(ButtonSNES.B, "joystick# button 2");		
		snesMapping.AddMap(ButtonSNES.A, "joystick# button 1");	
		snesMapping.AddMap(ButtonSNES.Y, "joystick# button 3");	
		snesMapping.AddMap(ButtonSNES.X, "joystick# button 0");
		snesMapping.AddMap(ButtonSNES.Select, "joystick# button 8");
		snesMapping.AddMap(ButtonSNES.Start, "joystick# button 9");
		snesMapping.AddMap(ButtonSNES.L, "joystick# button 4");
		snesMapping.AddMap(ButtonSNES.R, "joystick# button 5");
		snesMapping.AddMap(ButtonSNES.Button8, "joystick# button 6");
		snesMapping.AddMap(ButtonSNES.Button9, "joystick# button 7");
		snesMapping.AddMap(ButtonSNES.Button10, "joystick# button 10");
		m_InputTypeMapping.Add(InputType.SNES_CONTROLLER, snesMapping);
		
		// Map the Logitech WingMan controller
		KeyCodeMapping wingmanMapping = new KeyCodeMapping();
		wingmanMapping.AddMap(ButtonLogitechWingMan.A, "joystick# button 0");		
		wingmanMapping.AddMap(ButtonLogitechWingMan.B, "joystick# button 1");	
		wingmanMapping.AddMap(ButtonLogitechWingMan.X, "joystick# button 3");	
		wingmanMapping.AddMap(ButtonLogitechWingMan.Y, "joystick# button 4");
		wingmanMapping.AddMap(ButtonLogitechWingMan.Start, "joystick# button 8");
		wingmanMapping.AddMap(ButtonLogitechWingMan.L, "joystick# button 6");
		wingmanMapping.AddMap(ButtonLogitechWingMan.R, "joystick# button 7");
		wingmanMapping.AddMap(ButtonLogitechWingMan.C, "joystick# button 2");
		wingmanMapping.AddMap(ButtonLogitechWingMan.Z, "joystick# button 5");
		wingmanMapping.AddMap(ButtonLogitechWingMan.Button4, "joystick# button 9");
		wingmanMapping.AddMap(ButtonLogitechWingMan.Button10, "joystick# button 10");
		m_InputTypeMapping.Add(InputType.LOGITECH_WINGMAN_CONTROLLER, wingmanMapping);
		
		// Map the keyboard player 1.
		KeyCodeMapping keyboardMapping1 = new KeyCodeMapping();
		keyboardMapping1.AddMap(ButtonPS3.X, "return");
		keyboardMapping1.AddMap(ButtonPS3.Up, "up");
		keyboardMapping1.AddMap(ButtonPS3.Down, "down");
		keyboardMapping1.AddMap(ButtonPS3.Left, "left");
		keyboardMapping1.AddMap(ButtonPS3.Right, "right");
		keyboardMapping1.AddMap(ButtonPS3.ForceQuit, "escape");
		m_InputTypeMapping.Add(InputType.KEYBOARD_PLAYER1, keyboardMapping1);
		
		// Map the keyboard player 2.
		KeyCodeMapping keyboardMapping2 = new KeyCodeMapping();
		keyboardMapping2.AddMap(ButtonPS3.X, "space");		
		keyboardMapping2.AddMap(ButtonPS3.Up, "w");
		keyboardMapping2.AddMap(ButtonPS3.Down, "s");
		keyboardMapping2.AddMap(ButtonPS3.Left, "a");
		keyboardMapping2.AddMap(ButtonPS3.Right, "d");
		keyboardMapping2.AddMap(ButtonPS3.ForceQuit, "escape");
		m_InputTypeMapping.Add(InputType.KEYBOARD_PLAYER2, keyboardMapping2);
		
		SetCustomInputMapping();
		SetAliases();
	}
	
	partial void SetCustomInputMapping();
	partial void SetAliases();
	
	public void DebugButton()
	{
		/*foreach (var button in Enum.GetValues(typeof(PlayerInputManager.ButtonId)).Cast<PlayerInputManager.ButtonId>()) 
		{
			if (GetButtonDown(button))
				Debug.Log(button.ToString());
		}*/
		
		for (int i = 0; i < 20; ++i) 
		{
			string keyCode = "joystick button " + i;
			if (Input.GetKeyDown(keyCode))
				Debug.Log(keyCode);
		}
	}
	
	public void AddAlias(string alias, ButtonPS3 buttonId)
	{
		AddAlias(alias, (ButtonId)buttonId);
	}
	
	public void AddAlias(string alias, ButtonXBOX360 buttonId)
	{
		AddAlias(alias, (ButtonId)buttonId);
	}
	
	public void AddAlias(string alias, ButtonId buttonId)
	{
		if (!m_ButtonAliases.ContainsKey(alias))
			m_ButtonAliases.Add(alias, buttonId);
	}
	
	#region Joystick
	public List<Vector2> GetLeftJoystick(out List<ControllerInput> inputs)
	{
		List<Vector2> leftJoysticks = new List<Vector2>();
		inputs = new List<ControllerInput>();
		Vector2 leftJoystick = Vector2.zero;
		
		foreach (var player in Enum.GetValues(typeof(PlayerInputManager.ControllerInput)).Cast<PlayerInputManager.ControllerInput>())
		{
			if (player != ControllerInput.None)
			{
				leftJoystick = GetLeftJoystick(player);
				if (leftJoystick != Vector2.zero)
				{
					leftJoysticks.Add(leftJoystick);
					inputs.Add(player);
				}
			}
		}
		
		return leftJoysticks;
	}
	
	public Vector2 GetLeftJoystick(ControllerInput player)
	{
		Vector2 movement = Vector2.zero;
		if (player != ControllerInput.None)
		{
			// Support the keyboard for player 1.
			if (player == ControllerInput.Keyboard1)
			{
				movement = GetLeftJoystick(InputType.KEYBOARD_PLAYER1);
			}
			// Support the keyboard for player 2.
			else if (player == ControllerInput.Keyboard2)
			{
				movement = GetLeftJoystick(InputType.KEYBOARD_PLAYER2);
			}
			// Support the keyboard for player 3.
			else if (player == ControllerInput.Keyboard3)
			{
				movement = GetLeftJoystick(InputType.KEYBOARD_PLAYER3);
			}
			// Support the keyboard for player 4.
			else if (player == ControllerInput.Keyboard4)
			{
				movement = GetLeftJoystick(InputType.KEYBOARD_PLAYER4);
			}
			// Support the keyboard for mouse.
			else if (player == ControllerInput.Mouse1)
			{
				movement = GetLeftJoystick(InputType.MOUSE1);
			}
			else
			{
				return GetJoystick("Left", player);
			}
			
			if (movement != Vector2.zero)
			{
				return movement;
			}
		}
		
		return Vector2.zero;
	}
	
	public Vector2 GetLeftJoystick(InputType inputType)
	{
		float x = 0.0f;
		float y = 0.0f;
		KeyCodeMapping keyCodeMapping;
		if (m_InputTypeMapping.TryGetValue(inputType, out keyCodeMapping))
		{
			string keyCode = keyCodeMapping.GetKeyCode(ButtonId.Up);
			if (keyCode != null && keyCode.Trim() != "" && Input.GetKey(keyCode))
			{
				y = 1.0f;
			}
			else
			{
				keyCode = keyCodeMapping.GetKeyCode(ButtonId.Down);
				if (keyCode != null && keyCode.Trim() != "" && Input.GetKey(keyCode))
				{
					y = -1.0f;
				}
			}
			keyCode = keyCodeMapping.GetKeyCode(ButtonId.Left);
			if (keyCode != null && keyCode.Trim() != "" && Input.GetKey(keyCode))
			{
				x = -1.0f;
			}
			else
			{
				keyCode = keyCodeMapping.GetKeyCode(ButtonId.Right);
				if (keyCode != null && keyCode.Trim() != "" && Input.GetKey(keyCode))
				{
					x = 1.0f;
				}
			}
		}
		
		if (x != 0.0f || y != 0.0f)
		{
			Vector2 raw = new Vector2(x, y);
			Vector2 normalized = raw.normalized;
			return raw.sqrMagnitude < normalized.sqrMagnitude ? raw : normalized;
		}
		
		return Vector2.zero;
	}
	
	public Vector2 GetRightJoystick(ControllerInput player)
	{
		return GetJoystick("Right", player);
	}
	
	public Vector2 GetJoystick(string joystick, ControllerInput player)
	{
		if (player == ControllerInput.None || player == 0)
		{
			return Vector2.zero;
		}

		// TODO ppoirier: Fix this. No more hack.
#if UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
		Vector2 raw = Vector2.zero;
		if (joystick != "Right")
		{
			raw = new Vector2(Input.GetAxis("Player" + (int)player + joystick + "Horizontal"), 
		                          Input.GetAxis("Player" + (int)player + joystick + "Vertical"));
		}
		else
		{
			// TODO ppoirier: This is pretty ugly. We need to specify wich axis fits with each controller on each platform.
			raw = new Vector2(Input.GetAxis("Player" + (int)player + joystick + "HorizontalXbox"), 
			                  Input.GetAxis("Player" + (int)player + joystick + "VerticalXbox"));
		}
#else
		Vector2 raw = new Vector2(Input.GetAxis("Player" + (int)player + joystick + "Horizontal"), 
							Input.GetAxis("Player" + (int)player + joystick + "Vertical"));
#endif
		Vector2 normalized = raw.normalized;
		
		// Need to make sure that the magnitude of the direction isn't longer than 1
		return raw.sqrMagnitude < normalized.sqrMagnitude ? raw : normalized;
	}
	
	public List<ControllerInput> GetPlayerInputButtonDown(string alias)
	{
		List<ControllerInput> playerInputs = new List<ControllerInput>();
		foreach (var playerInputTemp in Enum.GetValues(typeof(PlayerInputManager.ControllerInput)).Cast<PlayerInputManager.ControllerInput>())
		{
			if (playerInputTemp != ControllerInput.None)
			{
				if (GetButtonDown(playerInputTemp, alias))
				{
					playerInputs.Add(playerInputTemp);
				}
			}
		}
		
		return playerInputs;
	}
	#endregion
	
	#region Buttons
	#region GetButton
	public bool GetButton(ControllerInput player)
	{
		bool isDown = false;
		foreach (var buttonId in Enum.GetValues(typeof(PlayerInputManager.ButtonId)).Cast<PlayerInputManager.ButtonId>())
		{
			if (GetButton(player, buttonId))
			{
				isDown = true;
				break;
			}
		}
		
		return isDown;
	}
	
	public bool GetButton(ControllerInput player, ButtonPS3 buttonId)
	{
		return GetButton(player, (ButtonId)buttonId);
	}
	
	public bool GetButton(ControllerInput player, ButtonXBOX360 buttonId)
	{
		return GetButton(player, (ButtonId)buttonId);
	}
	
	public bool GetButton(ControllerInput player, ButtonId buttonId)
	{
		bool isDown = false;
		
		string keyCode = GetKeyCode(player, buttonId);
		if (!string.IsNullOrEmpty(keyCode))
			isDown = Input.GetKey(keyCode);
		
		/*if (!isDown)
		{
			if (player == ControllerInput.Controller1)
			{
				isDown = GetButton(InputType.KEYBOARD_PLAYER1, buttonId);
			}
			else if (player == ControllerInput.Controller2)
			{
				isDown = GetButton(InputType.KEYBOARD_PLAYER2, buttonId);
			}
			else if (player == ControllerInput.Controller3)
			{
				isDown = GetButton(InputType.KEYBOARD_PLAYER3, buttonId);
			}
			else if (player == ControllerInput.Controller4)
			{
				isDown = GetButton(InputType.KEYBOARD_PLAYER4, buttonId);
			}
		}*/
		
		return isDown;
	}
	
	public bool GetButton(InputType inputType, ButtonId buttonId)
	{
		KeyCodeMapping keyCodeMapping;
		if (m_InputTypeMapping.TryGetValue(inputType, out keyCodeMapping))
		{
			string keyCode = keyCodeMapping.GetKeyCode(buttonId);
			if (keyCode != null && keyCode.Trim() != "" && Input.GetKey(keyCode))
			{
				return true;
			}
		}
		
		return false;
	}
	
	public bool GetButton(ControllerInput player, string alias)
	{
		bool isDown = false;
		
		if (m_ButtonAliases.ContainsKey(alias))
			isDown = GetButton(player, m_ButtonAliases[alias]);
		
		return isDown;
	}
	
	public bool GetButton(ButtonPS3 buttonId)
	{
		return GetButton((ButtonId)buttonId);
	}
	
	public bool GetButton(ButtonXBOX360 buttonId)
	{
		return GetButton((ButtonId)buttonId);
	}
	
	public bool GetButton(ButtonId buttonId)
	{
		return GetButton(ControllerInput.Controller1, buttonId) || 
				GetButton(ControllerInput.Controller2, buttonId) ||
				GetButton(ControllerInput.Controller3, buttonId) ||
				GetButton(ControllerInput.Controller4, buttonId) ||
				GetButton(ControllerInput.Controller5, buttonId) ||
				GetButton(ControllerInput.Controller6, buttonId) ||
				GetButton(ControllerInput.Keyboard1, buttonId) ||
				GetButton(ControllerInput.Keyboard2, buttonId) ||
				GetButton(ControllerInput.Keyboard3, buttonId) ||
				GetButton(ControllerInput.Keyboard4, buttonId) ||
				GetButton(ControllerInput.Mouse1, buttonId);
	}
	
	public bool GetButton(string alias)
	{
		return GetButton(ControllerInput.Controller1, alias) || 
				GetButton(ControllerInput.Controller2, alias) ||
				GetButton(ControllerInput.Controller3, alias) ||
				GetButton(ControllerInput.Controller4, alias) ||
				GetButton(ControllerInput.Controller5, alias) ||
				GetButton(ControllerInput.Controller6, alias) ||
				GetButton(ControllerInput.Keyboard1, alias) ||
				GetButton(ControllerInput.Keyboard2, alias) ||
				GetButton(ControllerInput.Keyboard3, alias) ||
				GetButton(ControllerInput.Keyboard4, alias) ||
				GetButton(ControllerInput.Mouse1, alias);
	}
	#endregion
	
	#region GetButtonDown
	public bool GetButtonDown(ControllerInput player)
	{
		bool isDown = false;
		foreach (var buttonId in Enum.GetValues(typeof(PlayerInputManager.ButtonId)).Cast<PlayerInputManager.ButtonId>())
		{
			if (GetButtonDown(player, buttonId))
			{
				isDown = true;
				break;
			}
		}
		
		return isDown;
	}
	
	public bool GetButtonDown(ControllerInput player, ButtonPS3 buttonId)
	{
		return GetButtonDown(player, (ButtonId)buttonId);
	}
	
	public bool GetButtonDown(ControllerInput player, ButtonXBOX360 buttonId)
	{
		return GetButtonDown(player, (ButtonId)buttonId);
	}
	
	public bool GetButtonDown(ControllerInput player, ButtonId buttonId)
	{
		bool isDown = false;
		
		string keyCode = GetKeyCode(player, buttonId);
		if (!string.IsNullOrEmpty(keyCode))
			isDown = Input.GetKeyDown(keyCode);
		
		/*if (!isDown)
		{
			if (player == ControllerInput.Controller1)
			{
				isDown = GetButtonDown(InputType.KEYBOARD_PLAYER1, buttonId);
			}
			else if (player == ControllerInput.Controller2)
			{
				isDown = GetButtonDown(InputType.KEYBOARD_PLAYER2, buttonId);
			}
			else if (player == ControllerInput.Controller3)
			{
				isDown = GetButtonDown(InputType.KEYBOARD_PLAYER3, buttonId);
			}
			else if (player == ControllerInput.Controller4)
			{
				isDown = GetButtonDown(InputType.KEYBOARD_PLAYER4, buttonId);
			}
		}*/
		
		return isDown;
	}
	
	public bool GetButtonDown(InputType inputType, ButtonId buttonId)
	{
		KeyCodeMapping keyCodeMapping;
		if (m_InputTypeMapping.TryGetValue(inputType, out keyCodeMapping))
		{
			string keyCode = keyCodeMapping.GetKeyCode(buttonId);
			if (keyCode != null && keyCode.Trim() != "" && Input.GetKeyDown(keyCode))
			{
				return true;
			}
		}
		
		return false;
	}
	
	public bool GetButtonDown(ControllerInput player, List<string> aliases)
	{
		foreach (string alias in aliases)
		{
			if (GetButtonDown(player, alias))
			{
				return true;
			}
		}
		
		return false;
	}
	
	public bool GetButtonDown(ControllerInput player, string alias)
	{
		bool isDown = false;
		
		if (player != ControllerInput.None)
		{
			if (m_ButtonAliases.ContainsKey(alias))
			{
				isDown = GetButtonDown(player, m_ButtonAliases[alias]);
			}
		}
		else
		{
			isDown = GetButtonDown(alias);
		}
		
		return isDown;
	}
	
	public bool GetButtonDown(ButtonPS3 buttonId)
	{
		return GetButtonDown((ButtonId)buttonId);
	}
	
	public bool GetButtonDown(ButtonXBOX360 buttonId)
	{
		return GetButtonDown((ButtonId)buttonId);
	}
	
	public bool GetButtonDown(ButtonId buttonId)
	{
		return GetButtonDown(ControllerInput.Controller1, buttonId) || 
				GetButtonDown(ControllerInput.Controller2, buttonId) ||
				GetButtonDown(ControllerInput.Controller3, buttonId) ||
				GetButtonDown(ControllerInput.Controller4, buttonId) ||
				GetButtonDown(ControllerInput.Controller5, buttonId) ||
				GetButtonDown(ControllerInput.Controller6, buttonId) ||
				GetButtonDown(ControllerInput.Keyboard1, buttonId) ||
				GetButtonDown(ControllerInput.Keyboard2, buttonId) ||
				GetButtonDown(ControllerInput.Keyboard3, buttonId) ||
				GetButtonDown(ControllerInput.Keyboard4, buttonId) ||
				GetButtonDown(ControllerInput.Mouse1, buttonId);
	}
	
	public bool GetButtonDown(string alias)
	{
		return GetButtonDown(ControllerInput.Controller1, alias) || 
				GetButtonDown(ControllerInput.Controller2, alias) ||
				GetButtonDown(ControllerInput.Controller3, alias) ||
				GetButtonDown(ControllerInput.Controller4, alias) ||
				GetButtonDown(ControllerInput.Controller5, alias) ||
				GetButtonDown(ControllerInput.Controller6, alias) ||
				GetButtonDown(ControllerInput.Keyboard1, alias) ||
				GetButtonDown(ControllerInput.Keyboard2, alias) ||
				GetButtonDown(ControllerInput.Keyboard3, alias) ||
				GetButtonDown(ControllerInput.Keyboard4, alias) ||
				GetButtonDown(ControllerInput.Mouse1, alias);
	}
	#endregion
	
	#region GetButtonUp
	public bool GetButtonUp(ControllerInput player)
	{
		bool isUp = false;
		foreach (var buttonId in Enum.GetValues(typeof(PlayerInputManager.ButtonId)).Cast<PlayerInputManager.ButtonId>())
		{
			if (GetButtonUp(player, buttonId))
			{
				isUp = true;
				break;
			}
		}
		
		return isUp;
	}
	
	public bool GetButtonUp(ControllerInput player, ButtonPS3 buttonId)
	{
		return GetButtonUp(player, (ButtonId)buttonId);
	}
	
	public bool GetButtonUp(ControllerInput player, ButtonXBOX360 buttonId)
	{
		return GetButtonUp(player, (ButtonId)buttonId);
	}
	
	public bool GetButtonUp(ControllerInput player, ButtonId buttonId)
	{
		bool isUp = false;
		
		string keyCode = GetKeyCode(player, buttonId);
		if (!string.IsNullOrEmpty(keyCode))
			isUp = Input.GetKeyUp(keyCode);
		
		/*if (!isUp)
		{
			if (player == ControllerInput.Controller1)
			{
				isUp = GetButtonUp(InputType.KEYBOARD_PLAYER1, buttonId);
			}
			else if (player == ControllerInput.Controller2)
			{
				isUp = GetButtonUp(InputType.KEYBOARD_PLAYER2, buttonId);
			}
			else if (player == ControllerInput.Controller3)
			{
				isUp = GetButtonUp(InputType.KEYBOARD_PLAYER3, buttonId);
			}
			else if (player == ControllerInput.Controller4)
			{
				isUp = GetButtonUp(InputType.KEYBOARD_PLAYER4, buttonId);
			}
		}*/
		
		return isUp;
	}
	
	public bool GetButtonUp(InputType inputType, ButtonId buttonId)
	{
		KeyCodeMapping keyCodeMapping;
		if (m_InputTypeMapping.TryGetValue(inputType, out keyCodeMapping))
		{
			string keyCode = keyCodeMapping.GetKeyCode(buttonId);
			if (keyCode != null && keyCode.Trim() != "" && Input.GetKeyUp(keyCode))
			{
				return true;
			}
		}
		
		return false;
	}
	
	public bool GetButtonUp(ControllerInput player, List<string> aliases)
	{
		foreach (string alias in aliases)
		{
			if (GetButtonUp(player, alias))
			{
				return true;
			}
		}
		
		return false;
	}
	
	public bool GetButtonUp(ControllerInput player, string alias)
	{
		bool isDown = false;
		
		if (player != ControllerInput.None)
		{
			if (m_ButtonAliases.ContainsKey(alias))
				isDown = GetButtonUp(player, m_ButtonAliases[alias]);
		}
		else
		{
			isDown = GetButtonUp(alias);
		}
		
		return isDown;
	}
	
	public bool GetButtonUp(ButtonPS3 buttonId)
	{
		return GetButtonUp((ButtonId)buttonId);
	}
	
	public bool GetButtonUp(ButtonXBOX360 buttonId)
	{
		return GetButtonUp((ButtonId)buttonId);
	}
	
	public bool GetButtonUp(ButtonId buttonId)
	{
		return GetButtonUp(ControllerInput.Controller1, buttonId) || 
				GetButtonUp(ControllerInput.Controller2, buttonId) ||
				GetButtonUp(ControllerInput.Controller3, buttonId) ||
				GetButtonUp(ControllerInput.Controller4, buttonId) ||
				GetButtonUp(ControllerInput.Controller5, buttonId) ||
				GetButtonUp(ControllerInput.Controller6, buttonId) ||
				GetButtonUp(ControllerInput.Keyboard1, buttonId) ||
				GetButtonUp(ControllerInput.Keyboard2, buttonId) ||
				GetButtonUp(ControllerInput.Keyboard3, buttonId) ||
				GetButtonUp(ControllerInput.Keyboard4, buttonId) ||
				GetButtonUp(ControllerInput.Mouse1, buttonId);
	}
	
	public bool GetButtonUp(string alias)
	{
		return GetButtonUp(ControllerInput.Controller1, alias) || 
				GetButtonUp(ControllerInput.Controller2, alias) ||
				GetButtonUp(ControllerInput.Controller3, alias) ||
				GetButtonUp(ControllerInput.Controller4, alias) ||
				GetButtonUp(ControllerInput.Controller5, alias) ||
				GetButtonUp(ControllerInput.Controller6, alias) ||
				GetButtonUp(ControllerInput.Keyboard1, alias) ||
				GetButtonUp(ControllerInput.Keyboard2, alias) ||
				GetButtonUp(ControllerInput.Keyboard3, alias) ||
				GetButtonUp(ControllerInput.Keyboard4, alias) ||
				GetButtonUp(ControllerInput.Mouse1, alias);
	}
	
	public bool GetButtonUp(string alias, out List<ControllerInput> inputs)
	{
		inputs = new List<ControllerInput>();
		if (GetButtonUp(ControllerInput.Controller1, alias))
		{
			inputs.Add(ControllerInput.Controller1);
		}
		if (GetButtonUp(ControllerInput.Controller2, alias))
		{
			inputs.Add(ControllerInput.Controller2);
		}
		if (GetButtonUp(ControllerInput.Controller3, alias))
		{
			inputs.Add(ControllerInput.Controller3);
		}
		if (GetButtonUp(ControllerInput.Controller4, alias))
		{
			inputs.Add(ControllerInput.Controller4);
		}
		if (GetButtonUp(ControllerInput.Controller5, alias))
		{
			inputs.Add(ControllerInput.Controller5);
		}
		if (GetButtonUp(ControllerInput.Controller6, alias))
		{
			inputs.Add(ControllerInput.Controller6);
		}
		if (GetButtonUp(ControllerInput.Keyboard1, alias))
		{
			inputs.Add(ControllerInput.Keyboard1);
		}
		if (GetButtonUp(ControllerInput.Keyboard2, alias))
		{
			inputs.Add(ControllerInput.Keyboard2);
		}
		if (GetButtonUp(ControllerInput.Keyboard3, alias))
		{
			inputs.Add(ControllerInput.Keyboard3);
		}
		if (GetButtonUp(ControllerInput.Keyboard4, alias))
		{
			inputs.Add(ControllerInput.Keyboard4);
		}
		if (GetButtonUp(ControllerInput.Mouse1, alias))
		{
			inputs.Add(ControllerInput.Mouse1);
		}
		
		return inputs.Count > 0;
	}
	#endregion
	
	private string GetKeyCode(ControllerInput player, ButtonId buttonId)
	{
		string keyCode = null;
		
		if (m_PlayerInputTypes.ContainsKey(player))
		{
			// Get the player's input type
			InputType inputType = m_PlayerInputTypes[player];
			
			// Get the key code mapping
			if (m_InputTypeMapping.ContainsKey(inputType))
			{
				KeyCodeMapping mapping = m_InputTypeMapping[inputType];
				
				// Get the corresponding
				keyCode = mapping.GetKeyCode(buttonId);
				if (!string.IsNullOrEmpty(keyCode))	
					keyCode = keyCode.Replace("#", " " + (int)player);
				else
					keyCode = null;
			}
			else
			{
				keyCode = null;
			}
		}
		
		return keyCode;
	}
	#endregion
	
	/// <summary>
	/// Holds the mapping between the PlayerInputManager buttons and the Input.KeyCode.
	/// </summary>
	public class KeyCodeMapping
	{
		private Dictionary<PlayerInputManager.ButtonId, string> m_KeyCodeMaps = new Dictionary<PlayerInputManager.ButtonId, string>();
		
		public KeyCodeMapping()
		{
		}
		
		public void AddMap(PlayerInputManager.ButtonPS3 buttonId, string inputKey)
		{
			AddMap((PlayerInputManager.ButtonId)buttonId, inputKey);
		}
		
		public void AddMap(PlayerInputManager.ButtonXBOX360 buttonId, string inputKey)
		{
			AddMap((PlayerInputManager.ButtonId)buttonId, inputKey);
		}
		
		public void AddMap(PlayerInputManager.ButtonSNES buttonId, string inputKey)
		{
			AddMap((PlayerInputManager.ButtonId)buttonId, inputKey);
		}
		
		public void AddMap(PlayerInputManager.ButtonLogitechWingMan buttonId, string inputKey)
		{
			AddMap((PlayerInputManager.ButtonId)buttonId, inputKey);
		}
		
		public void AddMap(PlayerInputManager.ButtonId buttonId, string inputKey)
		{
			if (!m_KeyCodeMaps.ContainsKey(buttonId))
				m_KeyCodeMaps.Add(buttonId, inputKey);
		}
		
		public string GetKeyCode(PlayerInputManager.ButtonId buttonId)
		{
			string keyCode = null;
			
			if (m_KeyCodeMaps.ContainsKey(buttonId))
				keyCode = m_KeyCodeMaps[buttonId];
			
			return keyCode;
		}
	}
}
