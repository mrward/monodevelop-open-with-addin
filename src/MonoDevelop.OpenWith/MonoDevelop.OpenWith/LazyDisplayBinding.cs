//
// LazyDisplayBinding.cs
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
using System.Linq;
using MonoDevelop.Core;
using MonoDevelop.Ide.Desktop;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;

namespace MonoDevelop.OpenWith
{
	class LazyDisplayBinding : IExternalDisplayBinding, IOpenWithDisplayBinding
	{
		string fileExtension;
		string mimeType;
		LazyOpenWithFileViewer lazyFileViewer;
		IExternalDisplayBinding displayBinding;
		bool findingDisplayBinding;

		public LazyDisplayBinding (
			FilePath fileName,
			string mimeType,
			LazyOpenWithFileViewer lazyFileViewer,
			bool canUseAsDefault = true)
		{
			fileExtension = fileName.Extension;
			this.mimeType = mimeType;
			this.lazyFileViewer = lazyFileViewer;

			CanUseAsDefault = canUseAsDefault;
		}

		public bool CanUseAsDefault { get; set; }

		public bool CanHandle (FilePath fileName, string mimeType, Project ownerProject)
		{
			if (displayBinding == null) {
				if (IsSupported (fileName)) {
					if (findingDisplayBinding)
						return false;

					FindDisplayBinding (fileName, mimeType, ownerProject);
				}
			}

			if (displayBinding != null)
				return displayBinding.CanHandle (fileName, mimeType, ownerProject);

			return false;
		}

		public DesktopApplication GetApplication (FilePath fileName, string mimeType, Project ownerProject)
		{
			if (displayBinding != null)
				return displayBinding.GetApplication (fileName, mimeType, ownerProject);

			return null;
		}

		public override string ToString ()
		{
			return string.Format (
				"LazyDisplayBinding Application={0}, CanUseAsDefault={1}]",
				lazyFileViewer.Title,
				CanUseAsDefault);
		}

		bool IsSupported (FilePath fileName)
		{
			return StringComparer.OrdinalIgnoreCase.Equals (fileName.Extension, fileExtension);
		}

		void FindDisplayBinding (FilePath fileName, string mimeType, Project ownerProject)
		{
			findingDisplayBinding = true;

			try {
				string mappingKey = lazyFileViewer.GetMappingKey ();

				var fileViewer = OpenWithFileViewer.GetFileViewers (
					fileName,
					mimeType,
					ownerProject).FirstOrDefault (item => item.GetMappingKey () == mappingKey);

				if (fileViewer != null) {
					displayBinding = DisplayBindingFactory.CreateDisplayBinding (
						fileName,
						mimeType,
						fileViewer) as IExternalDisplayBinding;
				}
			} finally {
				findingDisplayBinding = false;
			}
		}
	}
}
