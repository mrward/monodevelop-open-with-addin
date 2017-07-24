﻿//
// ExternalProcessDesktopApplication.cs
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

using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Ide.Desktop;

namespace MonoDevelop.OpenWith
{
	class ExternalProcessDesktopApplication : DesktopApplication
	{
		ProcessExecutionCommand command;

		public ExternalProcessDesktopApplication (
			ProcessExecutionCommand command,
			string displayName,
			bool isDefault)
			: base (command.Command, displayName, isDefault)
		{
			this.command = command;
		}

		public override void Launch (params string[] files)
		{
			var console = ExternalConsoleFactory.Instance.CreateConsole (true);
			var handler = Runtime.ProcessService.GetDefaultExecutionHandler (command);
			handler.Execute (command, console);
		}
	}
}