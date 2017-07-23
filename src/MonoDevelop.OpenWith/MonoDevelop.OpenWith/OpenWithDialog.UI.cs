//
// OpenWithDialog.UI.cs
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

using Xwt;
using MonoDevelop.Core;

namespace MonoDevelop.OpenWith
{
	partial class OpenWithDialog : Dialog
	{
		ListBox openWithItemsListBox;
		Button addButton;
		Button removeButton;
		Button setAsDefaultButton;
		DialogButton okButton;
		DialogButton cancelButton;

		void Build ()
		{
			Title = viewModel.GetTitle ();
			Height = 480;
			Width = 640;

			var mainVBox = new VBox ();
			Content = mainVBox;

			var topLabel = new Label ();
			topLabel.Text = GettextCatalog.GetString ("Choose the program you want to use to open this file:");
			mainVBox.PackStart (topLabel);

			var mainHBox = new HBox ();
			mainVBox.PackStart (mainHBox, true, true);

			openWithItemsListBox = new ListBox ();
			mainHBox.PackStart (openWithItemsListBox, true, true);

			var rightHandVBox = new VBox ();
			mainHBox.PackStart (rightHandVBox);

			addButton = new Button ();
			addButton.Label = GettextCatalog.GetString ("Add...");
			rightHandVBox.PackStart (addButton);

			removeButton = new Button ();
			removeButton.Label = GettextCatalog.GetString ("Remove...");
			rightHandVBox.PackStart (removeButton);

			setAsDefaultButton = new Button ();
			setAsDefaultButton.Label = GettextCatalog.GetString ("Set as Default");
			rightHandVBox.PackStart (setAsDefaultButton);

			cancelButton = new DialogButton (Command.Cancel);
			cancelButton.Clicked += (sender, e) => Close ();

			Buttons.Add (cancelButton);

			okButton = new DialogButton (Command.Ok);
			Buttons.Add (okButton);
		}
	}
}
