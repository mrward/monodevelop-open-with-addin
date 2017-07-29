//
// OpenWithSettings.cs
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
using System.Text;
using System.Xml;
using MonoDevelop.Core;

namespace MonoDevelop.OpenWith
{
	class OpenWithSettings
	{
		Dictionary<DisplayBindingMappingKey, string> mappings;
		Dictionary<DisplayBindingMappingKey, List<UserDefinedOpenWithFileViewer>> userDefinedFileViewers;

		static readonly FilePath fileName = UserProfile.Current.ConfigDir.Combine ("OpenWithSettings.xml");

		OpenWithSettings ()
			: this (
				new Dictionary<DisplayBindingMappingKey, string> (),
				new Dictionary<DisplayBindingMappingKey, List<UserDefinedOpenWithFileViewer>> ())
		{
		}

		OpenWithSettings (
			Dictionary<DisplayBindingMappingKey, string> mappings,
			Dictionary<DisplayBindingMappingKey, List<UserDefinedOpenWithFileViewer>> userDefinedFileViewers)
		{
			this.mappings = mappings;
			this.userDefinedFileViewers = userDefinedFileViewers;
		}

		public Dictionary<DisplayBindingMappingKey, string> Mappings {
			get { return mappings; }
		}

		public Dictionary<DisplayBindingMappingKey, List<UserDefinedOpenWithFileViewer>> UserDefinedFileViewers {
			get { return userDefinedFileViewers; }
		}

		public static void Save (
			Dictionary<DisplayBindingMappingKey, string> mappings,
			Dictionary<DisplayBindingMappingKey, List<UserDefinedOpenWithFileViewer>> userDefinedFileViewers)
		{
			try {
				var settings = new OpenWithSettings (mappings, userDefinedFileViewers);
				settings.Save ();
			} catch (Exception ex) {
				LoggingService.LogError ("Unable to save Open With configuration.", ex);
			}
		}

		void Save ()
		{
			using (var writer = new XmlTextWriter (fileName, Encoding.UTF8)) {
				writer.Formatting = Formatting.Indented;
				writer.WriteStartDocument ();
				writer.WriteStartElement ("OpenWithSettings");

				WriteUserDefinedFileViewers (writer);
				WriteDefaultMappings (writer);

				writer.WriteEndElement ();
			}
		}

		void WriteUserDefinedFileViewers (XmlTextWriter writer)
		{
			foreach (var keyValuePair in userDefinedFileViewers) {
				writer.WriteStartElement ("UserDefinedFileViewerGroup");

				writer.WriteAttributeString ("extension", keyValuePair.Key.FileExtension);
				writer.WriteAttributeString ("mimeType", keyValuePair.Key.MimeType);

				foreach (var fileViewer in keyValuePair.Value) {
					WriteUserDefinedFileViewer (writer, fileViewer);
				}

				writer.WriteEndElement ();
			}
		}

		void WriteDefaultMappings (XmlTextWriter writer)
		{
			foreach (var keyValuePair in mappings) {
				writer.WriteStartElement ("DefaultMapping");

				writer.WriteAttributeString ("extension", keyValuePair.Key.FileExtension);
				writer.WriteAttributeString ("mimeType", keyValuePair.Key.MimeType);

				writer.WriteElementString ("DisplayBinding", keyValuePair.Value);

				writer.WriteEndElement ();
			}
		}

		void WriteUserDefinedFileViewer (XmlTextWriter writer, UserDefinedOpenWithFileViewer fileViewer)
		{
			writer.WriteStartElement ("UserDefinedFileViewer");

			var app = fileViewer.GetApplication ();

			writer.WriteElementString ("Application", app.Id);
			writer.WriteElementString ("DisplayName", app.DisplayName);

			var externalApp = app as ExternalProcessDesktopApplication;
			writer.WriteElementString ("Arguments", externalApp?.Arguments ?? string.Empty);

			writer.WriteElementString ("IsDefault", fileViewer.IsDefault.ToString ());

			writer.WriteEndElement ();
		}

		public static OpenWithSettings Read ()
		{
			var settings = new OpenWithSettings ();
			
			try {
				settings.ReadSettings ();
			} catch (Exception ex) {
				LoggingService.LogError ("Unable to read Open With configuration.", ex);
			}

			return settings;
		}

		void ReadSettings ()
		{
			if (!File.Exists (fileName))
				return;

			using (var reader = new XmlTextReader (fileName)) {
				while (reader.Read ()) {
					switch (reader.NodeType) {
						case XmlNodeType.Element:
							switch (reader.LocalName) {
								case "DefaultMapping":
									ReadDefaultMapping (reader);
									break;
								case "UserDefinedFileViewerGroup":
									ReadUserDefinedFileViewerGroup (reader);
									break;
							}
							break;
					}
				}
			}
		}

		void ReadDefaultMapping (XmlTextReader reader)
		{
			var key = GetKey (reader);

			while (reader.Read ()) {
				switch (reader.NodeType) {
					case XmlNodeType.Element:
						switch (reader.LocalName) {
							case "DisplayBinding":
								AddDefaultMapping (key, reader.ReadElementContentAsString ());
								return;
							default:
								throw new ApplicationException ("Unsupported element found when looking for DefaultMapping/DisplayBinding");
						}
				}
			}
		}

		void AddDefaultMapping (DisplayBindingMappingKey key, string displayBinding)
		{
			mappings [key] = displayBinding;
		}

		void ReadUserDefinedFileViewerGroup (XmlTextReader reader)
		{
			var key = GetKey (reader);

			var userDefinedFileViewersList = new List<UserDefinedOpenWithFileViewer> ();

			while (reader.Read ()) {
				switch (reader.NodeType) {
					case XmlNodeType.Element:
						if (reader.LocalName == "UserDefinedFileViewer") {
							var fileViewer = ReadUserDefinedFileViewer (key, reader);
							userDefinedFileViewersList.Add (fileViewer);
						}
						break;
					case XmlNodeType.EndElement:
						if (reader.LocalName == "UserDefinedFileViewerGroup") {
							userDefinedFileViewers [key] = userDefinedFileViewersList;
							return;
						}
						break;
				}
			}

			throw new ApplicationException ("Unable to read UserDefinedFileViewerGroup");
		}

		UserDefinedOpenWithFileViewer ReadUserDefinedFileViewer (DisplayBindingMappingKey key, XmlTextReader reader)
		{
			string application = null;
			string displayName = null;
			string arguments = null;
			bool? isDefault = null;

			while (reader.Read ()) {
				switch (reader.NodeType) {
					case XmlNodeType.Element:
						switch (reader.LocalName) {
							case "Application":
								application = reader.ReadElementContentAsString ();
								if (CanCreateUserDefinedFileViewer (application, displayName, arguments, isDefault))
									return CreateUserDefinedFileViewer (key, application, displayName, arguments, isDefault.Value);
								break;
							case "Arguments":
								arguments = reader.ReadElementContentAsString ();
								if (CanCreateUserDefinedFileViewer (application, displayName, arguments, isDefault))
									return CreateUserDefinedFileViewer (key, application, displayName, arguments, isDefault.Value);
								break;
							case "DisplayName":
								displayName = reader.ReadElementContentAsString ();
								if (CanCreateUserDefinedFileViewer (application, displayName, arguments, isDefault))
									return CreateUserDefinedFileViewer (key, application, displayName, arguments, isDefault.Value);
								break;
							case "IsDefault":
								string isDefaultText = reader.ReadElementContentAsString ();
								isDefault = bool.Parse (isDefaultText);
								if (CanCreateUserDefinedFileViewer (application, displayName, arguments, isDefault))
									return CreateUserDefinedFileViewer (key, application, displayName, arguments, isDefault.Value);
								break;
							default:
								throw new ApplicationException ("Unsupported element found when looking for UserDefinedFileViewer");
						}
						break;
				}
			}

			throw new ApplicationException ("UserDefinedFileViewer not found.");
		}

		DisplayBindingMappingKey GetKey (XmlTextReader reader)
		{
			string extension = reader.GetAttribute ("extension");
			string mimeType = reader.GetAttribute ("mimeType");

			return new DisplayBindingMappingKey (extension, mimeType);
		}

		bool CanCreateUserDefinedFileViewer (
			string application,
			string displayName,
			string arguments,
			bool? isDefault)
		{
			return application != null && displayName != null && arguments != null && isDefault.HasValue;
		}

		UserDefinedOpenWithFileViewer CreateUserDefinedFileViewer (
			DisplayBindingMappingKey key,
			string application,
			string displayName,
			string arguments,
			bool isDefault)
		{
			var app = DesktopApplicationFactory.CreateApplication (application, arguments, displayName);
			var fileViewer = new UserDefinedOpenWithFileViewer (app);
			if (isDefault)
				fileViewer.SetAsDefault ();

			return fileViewer;
		}
	}
}
