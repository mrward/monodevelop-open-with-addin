//
// OpenWithDialog.cs
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
using Xwt;

namespace MonoDevelop.OpenWith
{
	partial class OpenWithDialog
	{
		OpenWithConfigurationViewModel viewModel;

		public OpenWithDialog (OpenWithConfigurationViewModel viewModel)
		{
			this.viewModel = viewModel;

			Build ();

			addButton.Clicked += AddButtonClicked;
			okButton.Clicked += OkButtonClicked;
			setAsDefaultButton.Clicked += SetAsDefaultButtonClicked;
			openWithItemsListBox.SelectionChanged += OpenWithItemsListBoxSelectionChanged;

			AddFileViewers ();
		}

		void AddFileViewers ()
		{
			foreach (var fileViewer in viewModel.GetFileViewers ()) {
				openWithItemsListBox.Items.Add (fileViewer, viewModel.GetTitle (fileViewer));
			}
		}

		void OpenWithItemsListBoxSelectionChanged (object sender, EventArgs e)
		{
			viewModel.SelectedItem = openWithItemsListBox.SelectedItem as OpenWithFileViewer;
			UpdateButtons ();
		}

		void UpdateButtons ()
		{
			removeButton.Sensitive = viewModel.CanRemove;
			setAsDefaultButton.Sensitive = viewModel.CanSetAsDefault;
		}

		void OkButtonClicked (object sender, EventArgs e)
		{
			viewModel.SaveChanges ();
			Close ();
		}

		void SetAsDefaultButtonClicked (object sender, EventArgs e)
		{
			viewModel.SetSelectedItemAsDefault ();
			UpdateTitles ();
			UpdateButtons ();
		}

		void UpdateTitles ()
		{
			var selectedItem = openWithItemsListBox.SelectedItem;
			openWithItemsListBox.Items.Clear ();
			AddFileViewers ();
			openWithItemsListBox.SelectedItem = selectedItem;
		}

		void AddButtonClicked (object sender, EventArgs e)
		{
			using (var dialog = new AddApplicationDialog ()) {
				if (dialog.ShowWithParent () == Command.Ok) {
				}
			}
		}
	}
}
