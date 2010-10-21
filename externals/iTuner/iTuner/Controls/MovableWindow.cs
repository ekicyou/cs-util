﻿//************************************************************************************************
// Copyright © 2010 Steven M. Cohn. All Rights Reserved.
//
//************************************************************************************************

namespace iTuner.Controls
{
	using System;
	using System.Windows;
	using System.Windows.Input;


	//********************************************************************************************
	// class MovableWindow
	//********************************************************************************************

	/// <summary>
	/// Base class for windows that can be moved around the screen by dragging the title bar.
	/// </summary>

	internal class MovableWindow : Window
	{
		private FrameworkElement element = null;
		private MouseButtonEventHandler handler = null;


		public MovableWindow ()
		{
		}


		protected void InitializeDragHandler (FrameworkElement element)
		{
			this.element = element;
			this.handler = new MouseButtonEventHandler(DoMoveWindowBegin);

			element.MouseLeftButtonDown += handler;
		}


		private void DoMoveWindowBegin (object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}


		protected override void OnClosed (EventArgs e)
		{
			if ((element != null) && (handler != null))
			{
				element.MouseLeftButtonDown -= handler;
			}

			base.OnClosed(e);
		}
	}
}
