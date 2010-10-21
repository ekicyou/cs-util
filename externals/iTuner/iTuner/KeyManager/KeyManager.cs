﻿//************************************************************************************************
// Copyright © 2010 Steven M. Cohn. All Rights Reserved.
//
//************************************************************************************************

namespace iTuner
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Runtime.Serialization;
	using System.Windows.Forms;
	using System.Xml;
	using Resx = Properties.Resources;


	/// <summary>
	/// Declares the signature of a handler method expected to process hot key sequences
	/// as they are performed by the user.
	/// </summary>

	internal delegate void HotKeyHandler (HotKey hotkey);


	//********************************************************************************************
	// class KeyManager
	//********************************************************************************************

	/// <summary>
	/// 
	/// </summary>

	internal class KeyManager : IDisposable
	{
		#region HKEventHandler

		/// <summary>
		/// Defines the signature of an event handler method for internally handling 
		/// Keypress events.
		/// </summary>
		/// <param name="e">A description of the key sequence.</param>

		private delegate void HKEventHandler (HKEventArgs e);


		/// <summary>
		/// Define the arguments passed internally from the DriverWindow to the KeyManager
		/// handlers.
		/// </summary>

		private class HKEventArgs : EventArgs
		{

			/// <summary>
			/// Initialize a new instance with the specified key and modifier flags.
			/// </summary>
			/// <param name="code">The keyboard key-code identifier.</param>
			/// <param name="modifier">The keyboard key modifiers.</param>

			public HKEventArgs (Keys code, KeyModifier modifier)
			{
				this.Code = code;
				this.Modifier = modifier;
			}


			/// <summary>
			/// Gets the keyboard key-code identifier.
			/// </summary>

			public Keys Code
			{
				get;
				private set;
			}


			/// <summary>
			/// Gets the keyboard key modififers.
			/// </summary>

			public KeyModifier Modifier
			{
				get;
				private set;
			}
		}

		#endregion HKEventHandler

		#region DriverWindow

		/// <summary>
		/// A native window used to monitor all system keyboard activity and
		/// intercept hot key sequences.
		/// </summary>

		private class DriverWindow : NativeWindow, IDisposable
		{
			// well known Windows definition indicating a registered hotkey was pressed
			private const int WM_HOTKEY = 0x312;


			public DriverWindow ()
			{
				// create a generic window with no class name
				base.CreateHandle(new CreateParams());
			}


			public void Dispose ()
			{
				base.DestroyHandle();
				GC.SuppressFinalize(this);
			}


			public event HKEventHandler KeyPressed;


			protected override void WndProc (ref Message message)
			{
				base.WndProc(ref message);
				if (message.Msg == WM_HOTKEY)
				{
					if (KeyPressed != null)
					{
						Keys code = ((Keys)(((int)message.LParam) >> 0x10)) & Keys.KeyCode;
						KeyModifier modifier = (KeyModifier)((int)message.LParam) & (KeyModifier)0xFFFF;

						KeyPressed(new HKEventArgs(code, modifier));
					}
				}
			}
		}

		#endregion WndProc Driver

		#region KeyInterops

		private static class KeyInterops
		{
			/// <summary>
			/// The RegisterHotKey function defines a system-wide hot key.
			/// </summary>
			/// <param name="hWnd">
			/// Handle to the window that will receive WM_HOTKEY messages generated by the hot key.
			/// If this parameter is NULL, WM_HOTKEY messages are posted to the message queue of
			/// the calling thread and must be processed in the message loop.
			/// </param>
			/// <param name="id">
			/// Specifies the identifier of the hot key. No other hot key in the calling thread
			/// should have the same identifier. An application must specify a value in the range
			/// 0x0000 through 0xBFFF. A shared DLL must specify a value in the range 0xC000
			/// through 0xFFFF (the range returned by the GlobalAddAtom function). To avoid conflicts
			/// with hot-key identifiers defined by other shared DLLs, a DLL should use the
			/// GlobalAddAtom function to obtain the hot-key identifier.
			/// </param>
			/// <param name="fsModifiers">
			/// Specifies keys that must be pressed in combination with the key specified by the
			/// uVirtKey parameter in order to generate the WM_HOTKEY message. The fsModifiers
			/// parameter can be a combination of the following values.
			/// </param>
			/// <param name="vlc">
			/// Specifies the virtual-key code of the hot key.
			/// </param>
			/// <returns>
			/// If the function succeeds, the return value is nonzero.  If the function fails,
			/// the return value is zero. To get extended error information, call GetLastError.
			/// </returns>

			[DllImport("user32.dll", SetLastError = true)]
			public static extern bool RegisterHotKey (IntPtr hWnd, int id, int fsModifiers, int vlc);


			/// <summary>
			/// The UnregisterHotKey function frees a hot key previously registered by the
			/// calling thread. 
			/// </summary>
			/// <param name="hWnd">
			/// Handle to the window associated with the hot key to be freed. This parameter
			/// should be NULL if the hot key is not associated with a window
			/// </param>
			/// <param name="id">
			/// Specifies the identifier of the hot key to be freed. 
			/// </param>
			/// <returns>
			/// If the function succeeds, the return value is nonzero.  If the function fails,
			/// the return value is zero. To get extended error information, call GetLastError
			/// </returns>

			[DllImport("user32.dll", SetLastError = true)]
			public static extern bool UnregisterHotKey (IntPtr hWnd, int id);
		}

		#endregion KeyInterops


		private const int MaxHotID = 0xBFFF;

		private DriverWindow window;
		private Dictionary<HotKeyAction, HotKey> keys;

		private string dataPath;
		private bool isDisposed;


		//========================================================================================
		// Constructor
		//========================================================================================

		/// <summary>
		/// Initialize a new instance, creating an internal driver window and loading
		/// initial hot key definitions.
		/// </summary>

		public KeyManager ()
		{
			this.IsEnabled = true;
			this.keys = new Dictionary<HotKeyAction, HotKey>();

			this.window = new DriverWindow();
			this.window.KeyPressed += new HKEventHandler(DoKeyPressed);

			this.dataPath = Path.Combine(
				PathHelper.ApplicationDataPath, Resx.FilenameHotkeys);

			Load();
		}


		/// <summary>
		/// Must unregister registered hot keys before destroying the native driver window.
		/// </summary>

		public void Dispose ()
		{
			if (!isDisposed)
			{
				if (window != null)
				{
					foreach (HotKey key in keys.Values)
					{
						KeyInterops.UnregisterHotKey(window.Handle, key.HotID);
					}
				}

				if (keys != null)
				{
					keys.Clear();
					keys = null;
				}

				if (window != null)
				{
					window.KeyPressed -= new HKEventHandler(DoKeyPressed);
					window.Dispose();
					window = null;
				}

				isDisposed = true;

				GC.SuppressFinalize(this);
			}
		}


		//========================================================================================
		// Properties
		//========================================================================================

		/// <summary>
		/// This event fires when a registered hot key sequence is pressed.
		/// </summary>

		public event HotKeyHandler KeyPressed;


		/// <summary>
		/// Gets or sets the state of the key manager.  This is <b>true</b> if the
		/// manager monitors and processes hot keys; <b>false</b> if it ignores
		/// hot keys.
		/// </summary>

		public bool IsEnabled
		{
			get;
			set;
		}


		/// <summary>
		/// Gets a collection of the registered hot keys.  Used to by the hot key editor.
		/// </summary>

		public HotKeyCollection KeyMap
		{
			get
			{
				HotKeyCollection map = new HotKeyCollection();
				foreach (HotKey key in keys.Values)
				{
					map.Add(new HotKey(key));
				}

				return map;
			}
		}


		//========================================================================================
		// Methods
		//========================================================================================

		/// <summary>
		/// Register a new hot key sequence, associating it with the specified action.
		/// </summary>
		/// <param name="code">The primary keyboard key-code identifier.</param>
		/// <param name="modifier">The secondary keyboard modifiers bit mask.</param>
		/// <param name="action">The well-known action to associated with the hot key.</param>

		public void RegisterHotKey (Keys code, KeyModifier modifier, HotKeyAction action)
		{
			HotKey key = new HotKey(action, code, modifier);
			bool ok = true;

			if (code != Keys.None)
			{
				try
				{
					ok = KeyInterops.RegisterHotKey(window.Handle, key.HotID, (int)modifier, (int)code);
				}
				catch (Exception exc)
				{
					Logger.WriteLine("RegisterHotKey", exc);
				}
			}

			if (ok)
			{
				keys.Add(key.Action, key);
			}
			else
			{
				MessageWindow.Show(
					String.Format(Resx.HotKeyNotRegistered, key.Action, key.ToString()));

				key.Code = Keys.None;
				key.Modifier = KeyModifier.None;

				if (keys.ContainsKey(action))
				{
					keys[action] = key;
				}
				else
				{
					keys.Add(action, key);
				}
			}
		}


		/// <summary>
		/// Unregisters the given key sequence as a known hot key.
		/// </summary>
		/// <param name="code">The primary keyboard key-code identifier.</param>
		/// <param name="modifier">The secondary keyboard modifiers bit mask.</param>

		public void UnregisterHotKey (Keys code, KeyModifier modifier)
		{
			HotKey key = keys.Values
				.FirstOrDefault(p => (p.Code == code) && (p.Modifier == modifier)) as HotKey;

			if (key != null)
			{
				keys.Remove(key.Action);

				if ((key.Code != Keys.None) && (key.Modifier != KeyModifier.None))
				{
					if (!KeyInterops.UnregisterHotKey(window.Handle, key.HotID))
					{
						MessageWindow.Show(String.Format(
							Resx.HotKeyNotUnregistered, key.Action, key.ToString()));
					}
				}
			}
		}


		/// <summary>
		/// Unregisters the existing hot keys and replaces them with the given
		/// hot key keymap.
		/// </summary>
		/// <param name="map">The map of new hot keys to register.</param>

		public void Update (HotKeyCollection map)
		{
			HotKeyCollection hotkeys = KeyMap;
			foreach (HotKey key in hotkeys)
			{
				UnregisterHotKey(key.Code, key.Modifier);
			}

			foreach (HotKey key in map)
			{
				RegisterHotKey(key.Code, key.Modifier, key.Action);
			}
		}


		/// <summary>
		/// Handles a hot key sequence keypress and fires the public KeyPressed event.
		/// </summary>
		/// <param name="e">The hot key properties.</param>

		private void DoKeyPressed (HKEventArgs e)
		{
			if (IsEnabled)
			{
				if (KeyPressed != null)
				{
					HotKey key = keys.Values
						.FirstOrDefault(p => (p.Code == e.Code) && (p.Modifier == e.Modifier));

					if (key != null)
					{
						//Logger.WriteLine(String.Format(
						//    "... KeyManager code {0}, mods {1}", key.Code, key.Modifier));
						
						KeyPressed(key);
					}
				}
			}
		}


		/// <summary>
		/// Loads preserved hot keys or set default hot keys.
		/// </summary>

		private void Load ()
		{
			HotKeyCollection map = null;

			if (File.Exists(dataPath))
			{
				try
				{
					using (XmlReader reader = XmlReader.Create(dataPath))
					{
						DataContractSerializer serializer = new DataContractSerializer(typeof(HotKeyCollection));
						map = (HotKeyCollection)serializer.ReadObject(reader);
						reader.Close();
					}
				}
				catch
				{
					map = null;
				}
			}

			if (map == null)
			{
				map = new HotKeyCollection();
			}

			KeyModifier mod = KeyModifier.Alt | KeyModifier.Win;

			MergeMap(map, HotKeyAction.PlayPause, Keys.Space, mod);
			MergeMap(map, HotKeyAction.NextTrack, Keys.Oemplus, mod);
			MergeMap(map, HotKeyAction.PrevTrack, Keys.OemMinus, mod);
			MergeMap(map, HotKeyAction.VolumeDown, Keys.Oemcomma, mod);
			MergeMap(map, HotKeyAction.VolumeUp, Keys.OemPeriod, mod);
			MergeMap(map, HotKeyAction.Mute, Keys.M, mod);
			MergeMap(map, HotKeyAction.ShowLyrics, Keys.L, mod);
			MergeMap(map, HotKeyAction.ShowiTunes, Keys.I, mod);
			MergeMap(map, HotKeyAction.ShowiTuner, Keys.T, mod);

			foreach (HotKey key in map)
			{
				RegisterHotKey(key.Code, key.Modifier, key.Action);
			}
		}


		/// <summary>
		/// Ensure a HotKey exists in the given map for the specified action.
		/// </summary>
		/// <param name="map">The key map to scan.</param>
		/// <param name="action">The action to define.</param>
		/// <param name="keys">The primary key to define.</param>
		/// <param name="modifier">The secondary key modifiers to define.</param>

		private void MergeMap (
			HotKeyCollection map, HotKeyAction action, Keys keys, KeyModifier modifier)
		{
			if (!map.Contains(action))
			{
				if (map.Contains(keys, modifier))
				{
					// can't have two actions with same key sequence so we need to randomize
					// this sequence.  This would only occur when a new version of iTuner
					// introduces a new action whose default sequence matches the user-defined
					// sequence of an existing action.

					keys = Keys.None;
					modifier = KeyModifier.None;
				}

				// Add will add the HotKey in the order prescribed by the HotKeyAction enum
				map.Add(new HotKey(action, keys, modifier));
			}
		}


		/// <summary>
		/// Save all currently registered hot key sequences and their associated actions
		/// to the user's local application data directory.
		/// </summary>

		public void Save ()
		{
			string dirPath = Path.GetDirectoryName(dataPath);
			if (!Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
			}

			HotKeyCollection map = new HotKeyCollection(keys.Values);

			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;

			try
			{
				using (XmlWriter writer = XmlWriter.Create(dataPath, settings))
				{
					DataContractSerializer serializer = new DataContractSerializer(typeof(HotKeyCollection));
					serializer.WriteObject(writer, map);

					writer.Flush();
					writer.Close();
				}
			}
			catch (Exception exc)
			{
				MessageWindow.Show(String.Format(Resx.HotKeysNotSaved, exc.Message));
			}
		}
	}
}
