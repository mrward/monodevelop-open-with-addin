//
// OpenWithPreferencesDesktopApplication.cs
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

using MonoDevelop.Ide.Desktop;
using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace MonoDevelop.OpenWith
{
	class OpenWithPreferencesDesktopApplication : DesktopApplication
	{
		FilePath fileName;
		string mimeType;
		Project project;

		public OpenWithPreferencesDesktopApplication (
			FilePath fileName,
			string mimeType,
			Project project)
			: base (
				"OpenPreferencesDesktopApplication",
				GettextCatalog.GetString ("Preferences..."),
				false)
		{
			this.fileName = fileName;
			this.mimeType = mimeType;
			this.project = project;
		}

		public override void Launch (params string[] files)
		{
			var viewModel = new OpenWithConfigurationViewModel (fileName, mimeType, project);

			var dialog = new OpenWithDialog (viewModel);
			dialog.ShowWithParent ();
		}
	}
}
