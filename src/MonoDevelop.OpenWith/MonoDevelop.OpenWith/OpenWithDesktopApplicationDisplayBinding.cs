//
// OpenWithDesktopApplicationDisplayBinding.cs
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
using MonoDevelop.Ide.Desktop;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;

namespace MonoDevelop.OpenWith
{
	class OpenWithDesktopApplicationDisplayBinding : IExternalDisplayBinding, IOpenWithDisplayBinding
	{
		DesktopApplication application;
		DefaultDesktopApplication defaultApplication;
		string fileExtension;
		string mimeType;

		public OpenWithDesktopApplicationDisplayBinding (
			FilePath fileName,
			string mimeType,
			DesktopApplication application,
			bool canUseAsDefault = true)
		{
			fileExtension = fileName.Extension;
			this.mimeType = mimeType;
			this.application = application;

			defaultApplication = new DefaultDesktopApplication (application);

			CanUseAsDefault = canUseAsDefault;
		}

		public bool CanUseAsDefault { get; set; }

		public bool CanHandle (FilePath fileName, string mimeType, Project ownerProject)
		{
			if (StringComparer.OrdinalIgnoreCase.Equals (mimeType, this.mimeType))
				return true;

			return StringComparer.OrdinalIgnoreCase.Equals (fileName.Extension, fileExtension);
		}

		public DesktopApplication GetApplication (FilePath fileName, string mimeType, Project ownerProject)
		{
			if (CanUseAsDefault)
				return defaultApplication;
			return application;
		}

		public override string ToString ()
		{
			return string.Format (
				"Application.Id={0}, CanUseAsDefault={1}]",
				application.Id,
				CanUseAsDefault);
		}
	}
}
