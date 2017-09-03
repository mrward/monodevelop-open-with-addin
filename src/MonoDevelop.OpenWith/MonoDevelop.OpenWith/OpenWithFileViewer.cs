//
// OpenWithFileViewer.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (C) 2009 Novell, Inc (http://www.novell.com)
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
//
// Based on MonoDevelop's DisplayBinding and FileViewer classes.

using System;
using System.Collections.Generic;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Desktop;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;

namespace MonoDevelop.OpenWith
{
	class OpenWithFileViewer
	{
		/// <summary>
		/// Returns the pre-defined file viewers.
		/// </summary>
		public static IEnumerable<OpenWithFileViewer> GetFileViewers (
			FilePath filePath,
			string mimeType,
			Project ownerProject)
		{
			var viewerIds = new HashSet<string> ();

			foreach (var b in GetDisplayBindings (filePath, mimeType, ownerProject)) {
				if (b is PreferencesDisplayBinding)
					continue;
				if (b is IOpenWithDisplayBinding)
					continue;
				var vb = b as IViewDisplayBinding;
				if (vb != null) {
					yield return new OpenWithFileViewer (vb);
				} else {
					var eb = (IExternalDisplayBinding) b;
					var app = eb.GetApplication (filePath, mimeType, ownerProject);
					if (viewerIds.Add (app.Id))
						yield return new OpenWithFileViewer (app);
				}
			}

			if (filePath.IsNullOrEmpty)
				yield break;

			foreach (var app in DesktopService.GetApplications (filePath))
				if (viewerIds.Add (app.Id))
					yield return new OpenWithFileViewer (app);
		}

		static IEnumerable<IDisplayBinding> GetDisplayBindings (FilePath filePath, string mimeType, Project ownerProject)
		{
			if (mimeType == null && !filePath.IsNullOrEmpty)
				mimeType = DesktopService.GetMimeTypeForUri (filePath);

			foreach (var b in DisplayBindingService.GetBindings<IDisplayBinding> ()) {
				bool canHandle = false;
				try {
					canHandle = b.CanHandle (filePath, mimeType, ownerProject);
				} catch (Exception ex) {
					LoggingService.LogError ("Error while getting display bindings", ex);
				}
				if (canHandle)
					yield return b;
			}
		}

		IViewDisplayBinding binding;
		DesktopApplication app;

		public OpenWithFileViewer (DesktopApplication app)
		{
			this.app = app;
		}

		OpenWithFileViewer (IViewDisplayBinding binding)
		{
			this.binding = binding;
		}

		public string GetMappingKey ()
		{
			if (binding != null)
				return binding.GetType ().FullName;

			return app.Id;
		}

		public string Title {
			get { return binding != null ? binding.Name : app.DisplayName; }
		}

		public bool CanUseAsDefault {
			get {
				if (binding != null)
					return binding.CanUseAsDefault;
				else
					return app.IsDefault;
			}
		}

		public bool IsDisplayBinding {
			get { return binding != null; }
		}

		public bool IsApplication {
			get { return app != null; }
		}

		public DesktopApplication GetApplication ()
		{
			return app;
		}
	}
}
