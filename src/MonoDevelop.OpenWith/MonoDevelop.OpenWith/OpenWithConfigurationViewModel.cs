//
// OpenWithConfigurationViewModel.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace MonoDevelop.OpenWith
{
	class OpenWithConfigurationViewModel
	{
		List<OpenWithFileViewer> fileViewers;
		Project project;
		FilePath fileName;
		string mimeType;
		OpenWithFileViewer selectedItem;
		OpenWithFileViewer defaultFileViewer;
		OpenWithFileViewer originalDefaultFileViewer;
		OpenWithFileViewer nonOverriddenOriginalDefaultFileViewer;

		List<UserDefinedOpenWithFileViewer> removedFileViewers
			= new List<UserDefinedOpenWithFileViewer> ();

		public OpenWithFileViewer SelectedItem {
			get { return selectedItem; }
			set {
				selectedItem = value;
				OnSelectedItemChanged ();
			}
		}

		public OpenWithFileViewer DefaultFileViewer {
			get { return defaultFileViewer; }
		}

		public bool CanRemove { get; private set; }
		public bool CanSetAsDefault { get; private set; }

		public OpenWithConfigurationViewModel (FilePath fileName, string mimeType, Project project)
		{
			this.fileName = fileName;
			this.mimeType = mimeType;
			this.project = project;
		}

		public string GetTitle (OpenWithFileViewer fileViewer)
		{
			if (fileViewer == defaultFileViewer)
				return GettextCatalog.GetString ("{0} (Default)", fileViewer.Title);

			return fileViewer.Title;
		}

		public string GetTitle ()
		{
			return GettextCatalog.GetString ("Open With - {0}", Path.GetFileName (fileName));
		}

		public IEnumerable<OpenWithFileViewer> GetFileViewers ()
		{
			if (fileViewers != null)
				return fileViewers;

			fileViewers = OpenWithFileViewer.GetFileViewers (fileName, mimeType, project).ToList ();

			fileViewers.AddRange (OpenWithServices.Mappings.GetUserDefinedFileViewers (fileName, mimeType));

			ConfigureDefaultFileViewer ();

			return fileViewers;
		}

		void ConfigureDefaultFileViewer ()
		{
			// Find the default file viewer ignoring any customisation.
			nonOverriddenOriginalDefaultFileViewer = fileViewers
				.FirstOrDefault (fileViewer => fileViewer.CanUseAsDefault);

			originalDefaultFileViewer = nonOverriddenOriginalDefaultFileViewer;
			defaultFileViewer = nonOverriddenOriginalDefaultFileViewer;

			// Find custom default file viewer.
			foreach (var fileViewer in fileViewers) {
				if (OpenWithServices.Mappings.IsCustomDefault (fileName, mimeType, fileViewer)) {
					originalDefaultFileViewer = fileViewer;
					defaultFileViewer = fileViewer;
				}
			}
		}

		void OnSelectedItemChanged ()
		{
			if (selectedItem != null) {
				CanSetAsDefault = !(selectedItem == defaultFileViewer);
				CanRemove = (selectedItem is UserDefinedOpenWithFileViewer);
			} else {
				CanRemove = false;
				CanSetAsDefault = false;
			}
		}

		public void SetSelectedItemAsDefault ()
		{
			defaultFileViewer = selectedItem;
			CanSetAsDefault = false;
		}

		public void SaveChanges ()
		{
			foreach (var newFileViewer in fileViewers.Where (IsNewUserDefinedFileViewer)) {
				OpenWithServices.Mappings.AddUserDefinedViewer (
					fileName,
					mimeType,
					(UserDefinedOpenWithFileViewer)newFileViewer);
			}

			if (defaultFileViewer != originalDefaultFileViewer) {
				if (defaultFileViewer != nonOverriddenOriginalDefaultFileViewer) {
					OpenWithServices.Mappings.SetAsDefault (fileName, mimeType, defaultFileViewer);
				} else {
					// The default file viewer has been changed back to the default
					// that the IDE would use anyway so just clear the default mapping.
					OpenWithServices.Mappings.ClearDefault (fileName, mimeType);
				}
			}

			foreach (var removedFileViewer in removedFileViewers) {
				OpenWithServices.Mappings.RemoveUserDefinedViewer (
					fileName,
					mimeType,
					removedFileViewer);
			}

			OpenWithServices.Mappings.Save ();
		}

		public void AddNewApplication (string application, string arguments, string friendlyName)
		{
			var app = DesktopApplicationFactory.CreateApplication (application, arguments, friendlyName);
			var fileViewer = new UserDefinedOpenWithFileViewer (app);
			fileViewer.IsNew = true;

			fileViewers.Add (fileViewer);
		}

		bool IsNewUserDefinedFileViewer (OpenWithFileViewer fileViewer)
		{
			var userDefinedFileViewer = fileViewer as UserDefinedOpenWithFileViewer;
			return userDefinedFileViewer?.IsNew == true;
		}

		public void RemoveSelectedItem ()
		{
			if (selectedItem == null)
				return;

			removedFileViewers.Add ((UserDefinedOpenWithFileViewer)selectedItem);
			fileViewers.Remove (selectedItem);

			if (selectedItem == defaultFileViewer) {
				defaultFileViewer = nonOverriddenOriginalDefaultFileViewer;
			}

			selectedItem = null;

			OnSelectedItemChanged ();
		}
	}
}
