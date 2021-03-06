﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsParser.Core.Code;
using EnvDTE80;
using EnvDTE;

namespace JsParser.AddIn
{
	public class VS2008CodeProvider : ICodeProvider
	{
		private DTE2 _applicationObject;
		private Document _activeDocument;

		public VS2008CodeProvider(DTE2 applicationObject, Document activeDocument)
		{
			_applicationObject = applicationObject;
			_activeDocument = activeDocument ?? _applicationObject.ActiveDocument;
			ContainerName = "Visual Studio " + _applicationObject.DTE.Version;
		}

		private Document Doc
		{
			get
			{
				return _activeDocument;
			}
		}

		#region ICodeProvider Members

		public string LoadCode()
		{
			try
			{
				var textDocument = (TextDocument)Doc.Object("TextDocument");
				var docContent = textDocument.CreateEditPoint(textDocument.StartPoint).GetText(textDocument.EndPoint);
				return docContent;
			}
			catch
			{
				return "function Error_Loading_Document(){}";
			}
		}

		public string FullName
		{
			get
			{
				return System.IO.Path.Combine(Path, Name);
			}
		}

		public string Path
		{
			get { return Doc != null ? Doc.Path : string.Empty; }
		}

		public string Name
		{
			get { return Doc != null ? Doc.Name : string.Empty; }
		}

		public void SelectionMoveToLineAndOffset(int StartLine, int StartColumn)
		{
			try
			{
				var textDocument = (TextDocument)Doc.Object("TextDocument");
				textDocument.Selection.MoveToLineAndOffset(StartLine, StartColumn, false);
			}
			catch
			{
			}
		}

		public void SetFocus()
		{
			if (Doc == null)
			{
				return;
			}

			Doc.Activate();
			_applicationObject.ActiveWindow.SetFocus();
		}

		public void GetCursorPos(out int line, out int column)
		{
			line = -1;
			column = -1;

			try
			{
				if (Doc != null)
				{
					var textDocument = (TextDocument)Doc.Object("TextDocument");
					line = textDocument.Selection.ActivePoint.Line;
					column = textDocument.Selection.ActivePoint.DisplayColumn;
				}
			}
			catch
			{
			}
		}

		#endregion


		public string ContainerName {get; set;}
	}
}
