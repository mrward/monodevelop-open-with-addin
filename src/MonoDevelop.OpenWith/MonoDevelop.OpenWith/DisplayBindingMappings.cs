//
// ViewDisplayBindingMappings.cs
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
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;

namespace MonoDevelop.OpenWith
{
	class DisplayBindingMappings
	{
		Dictionary<string, string> mappings =
			new Dictionary<string, string> (StringComparer.OrdinalIgnoreCase);

		Dictionary<string, IDisplayBinding> defaultDisplayBindings =
			new Dictionary<string, IDisplayBinding> (StringComparer.OrdinalIgnoreCase);

		public IDisplayBinding GetDisplayBinding (FilePath fileName, string mimeType, Project ownerProject)
		{
			string mapping = null;
			string key = GetKey (fileName, mimeType);
			if (!mappings.TryGetValue (key, out mapping))
				return null;

			foreach (var displayBinding in DisplayBindingService.GetBindings<IDisplayBinding> ()) {
				if (displayBinding.GetType ().FullName == mapping)
					return displayBinding;
			}

			return null;
		}

		public void SetAsDefault (FilePath fileName, string mimeType, OpenWithFileViewer fileViewer)
		{
			ClearDefault (fileName, mimeType);

			if (fileViewer == null)
				return;

			string key = GetKey (fileName, mimeType);

			mappings [key] = fileViewer.GetMappingKey ();

			var displayBinding = DisplayBindingFactory.CreateDisplayBinding (fileName, mimeType, fileViewer);
			defaultDisplayBindings [key] = displayBinding;
			DisplayBindingService.RegisterRuntimeDisplayBinding (displayBinding);
		}

		public void ClearDefault (FilePath fileName, string mimeType)
		{
			string key = GetKey (fileName, mimeType);
			mappings.Remove (key);

			IDisplayBinding displayBinding = null;
			if (defaultDisplayBindings.TryGetValue (key, out displayBinding)) {
				DisplayBindingService.DeregisterRuntimeDisplayBinding (displayBinding);
				defaultDisplayBindings.Remove (key);
			}
		}

		static string GetKey (FilePath fileName, string mimeType)
		{
			return $"{fileName.Extension}-{mimeType}";
		}
	}
}
