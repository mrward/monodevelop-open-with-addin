﻿//
// AddApplicationDialog.cs
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
	partial class AddApplicationDialog
	{
		public AddApplicationDialog ()
		{
			Build ();

			okButton.Sensitive = false;

			browseButton.Clicked += BrowseButtonClicked;
			applicationTextEntry.Changed += ApplicationTextEntryChanged;
		}

		void BrowseButtonClicked (object sender, EventArgs e)
		{
			using (var folderDialog = new OpenFileDialog ()) {
				folderDialog.Title = GettextCatalog.GetString ("Select Application");
				folderDialog.Multiselect = false;
				if (folderDialog.Run ()) {
					applicationTextEntry.Text = folderDialog.FileName;
				}
			}
		}

		public string Application {
			get { return applicationTextEntry.Text; }
		}

		public string Arguments {
			get { return argumentsTextEntry.Text; }
		}

		public string FriendlyName {
			get { return friendlyNameTextEntry.Text; }
		}

		void ApplicationTextEntryChanged (object sender, EventArgs e)
		{
			okButton.Sensitive = Application.Length > 0;
		}
	}
}