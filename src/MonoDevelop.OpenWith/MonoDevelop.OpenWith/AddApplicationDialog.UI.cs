//
// AddApplicationDialog.UI.cs
//
// Author:
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2017 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using MonoDevelop.Core;
using Xwt;

namespace MonoDevelop.OpenWith
{
	partial class AddApplicationDialog : Dialog
	{
		DialogButton cancelButton;
		DialogButton okButton;
		Button browseButton;
		TextEntry applicationTextEntry;
		TextEntry argumentsTextEntry;
		TextEntry friendlyNameTextEntry;
		Label applicationLabel;
		Label argumentsLabel;
		Label friendlyNameLabel;

		void Build ()
		{
			Title = GettextCatalog.GetString ("Add Application");
			Width = 480;

			var mainVBox = new VBox ();
			Content = mainVBox;

			var applicationHBox = new HBox ();
			mainVBox.PackStart (applicationHBox);

			applicationLabel = new Label ();
			applicationLabel.Text = GettextCatalog.GetString ("Application:");
			applicationHBox.PackStart (applicationLabel);

			applicationTextEntry = new TextEntry ();
			applicationHBox.PackStart (applicationTextEntry, true, true);

			browseButton = new Button ();
			browseButton.Label = GettextCatalog.GetString ("Browse...");
			applicationHBox.PackStart (browseButton);

			var argumentsHBox = new HBox ();
			mainVBox.PackStart (argumentsHBox);

			argumentsLabel = new Label ();
			argumentsLabel.Text = GettextCatalog.GetString ("Arguments:");
			argumentsHBox.PackStart (argumentsLabel);

			argumentsTextEntry = new TextEntry ();
			argumentsHBox.PackStart (argumentsTextEntry, true, true);

			var friendlyNameHBox = new HBox ();
			mainVBox.PackStart (friendlyNameHBox);

			friendlyNameLabel = new Label ();
			friendlyNameLabel.Text = GettextCatalog.GetString ("Friendly Name:");
			friendlyNameHBox.PackStart (friendlyNameLabel);

			friendlyNameTextEntry = new TextEntry ();
			friendlyNameHBox.PackStart (friendlyNameTextEntry, true, true);

			cancelButton = new DialogButton (Command.Cancel);
			cancelButton.Clicked += (sender, e) => Close ();

			Buttons.Add (cancelButton);

			okButton = new DialogButton (Command.Ok);
			Buttons.Add (okButton);
		}

		/// <summary>
		/// Ensure labels have the same width.
		/// </summary>
		protected override void OnBoundsChanged (BoundsChangedEventArgs a)
		{
			double width = Math.Max (friendlyNameLabel.WindowBounds.Width, 0);
			width = Math.Max (applicationLabel.WindowBounds.Width, width);
			width = Math.Max (argumentsLabel.WindowBounds.Width, width);

			if (friendlyNameLabel.WindowBounds.Width < width)
				friendlyNameLabel.WidthRequest = width;

			if (applicationLabel.WindowBounds.Width < width)
				applicationLabel.WidthRequest = width;

			if (argumentsLabel.WindowBounds.Width < width)
				argumentsLabel.WidthRequest = width;

			base.OnBoundsChanged (a);
		}
	}
}
