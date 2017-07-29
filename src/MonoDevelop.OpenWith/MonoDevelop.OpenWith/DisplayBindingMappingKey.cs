//
// DisplayBindingMappingKey.cs
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

namespace MonoDevelop.OpenWith
{
	class DisplayBindingMappingKey : IEquatable<DisplayBindingMappingKey>
	{
		public DisplayBindingMappingKey (FilePath fileName, string mimeType)
		{
			FileExtension = fileName.Extension;
			MimeType = mimeType;
		}

		public DisplayBindingMappingKey (string fileExtension, string mimeType)
		{
			FileExtension = fileExtension;
			MimeType = mimeType;
		}

		public string FileExtension { get; private set; }
		public string MimeType { get; private set; }

		string GetKey ()
		{
			return $"{FileExtension}-{MimeType}";
		}

		public override int GetHashCode()
		{
			return StringComparer.OrdinalIgnoreCase.GetHashCode (GetKey ());
		}

		public override bool Equals (object obj)
		{
			return Equals (obj as DisplayBindingMappingKey);
		}

		public bool Equals (DisplayBindingMappingKey other)
		{
			if (other != null)
				return StringComparer.OrdinalIgnoreCase.Equals (other.GetKey (), GetKey ());

			return false;
		}
	}
}
