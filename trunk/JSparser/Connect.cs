using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using JsParserCore.UI;

namespace JSparser
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
		/// <summary>
		/// The navigation tree view.
		/// </summary>
		private NavigationTreeView _navigationTreeView;

		private Window _toolWindow;

		/// <summary>
		/// The document events.
		/// </summary>
		private DocumentEvents _documentEvents;

		private DTE2 _applicationObject;
		private AddIn _addInInstance;

		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;
			if (connectMode == ext_ConnectMode.ext_cm_UISetup || _documentEvents == null)
			{
				object []contextGUIDS = new object[] { };
				Commands2 commands = (Commands2)_applicationObject.Commands;
				string toolsMenuName = "Tools";

				//Place the command on the tools menu.
				//Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
				Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];

				//Find the Tools command bar on the MenuBar command bar:
				CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
				CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

				//This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
				//  just make sure you also update the QueryStatus/Exec method to include the new command names.
				for (int i = 629; i < 630; ++i) //this is for scan of required icon. works on 1000 range quite well
				{
					try
					{
						//Add a command to the Commands collection:
						Command command = commands.AddNamedCommand2(_addInInstance, "JSparser", "Javascript parser", "Javascript parser addin", true,
							i,
							ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

						//Add a control for the command to the tools menu:
						if ((command != null) && (toolsPopup != null))
						{
							command.AddControl(toolsPopup.CommandBar, 1);
						}
					}
					catch (System.ArgumentException)
					{
						//If we are here, then the exception is probably because a command with that name
						//  already exists. If so there is no need to recreate the command and we can 
						//  safely ignore the exception.
					}
				}
				Events events = _applicationObject.Events;
				_documentEvents = events.get_DocumentEvents(null);
				_documentEvents.DocumentClosing += documentEvents_DocumentClosing;
				_documentEvents.DocumentSaved += documentEvents_DocumentSaved;

				events.SolutionEvents.Opened += solutionEvents_Opened;

				var windowEvents = events.get_WindowEvents(null);
				windowEvents.WindowActivated += windowEvents_WindowActivated;
			}
		}

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
				if(commandName == "JSparser.Connect.JSparser")
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
					return;
				}
			}
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
				if(commandName == "JSparser.Connect.JSparser")
				{
					ShowWindow();
					handled = true;
					return;
				}
			}
		}

		/// <summary>
		/// Show control.
		/// </summary>
		/// <returns>
		/// The show window.
		/// </returns>
		private bool ShowWindow()
		{
			if (EnsureWindowCreated())
			{
				if (!_toolWindow.Linkable)
					_toolWindow.Linkable = true;
				if (_toolWindow.IsFloating)
					_toolWindow.IsFloating = false;
				if (!_toolWindow.Visible)
					_toolWindow.Visible = true;
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Creates control.
		/// </summary>
		/// <returns>
		/// The ensure window created.
		/// </returns>
		private bool EnsureWindowCreated()
		{
			if (_navigationTreeView != null) return true;
			try
			{
				Cursor.Current = Cursors.WaitCursor;
				object obj = null;
				string guid = "{119157a3-dce1-4cb2-99c2-13d59c269bcc}";
				var windows2 = (Windows2) _applicationObject.Windows;
				Assembly asm = Assembly.GetExecutingAssembly();

				try
				{
					_toolWindow = windows2.CreateToolWindow2(_addInInstance, asm.Location, typeof(NavigationTreeView).ToString(),
															  "JavaScript Parser", guid, ref obj);
				}
				catch
				{
					return false;
				}

				_navigationTreeView = obj as NavigationTreeView;
				if (_navigationTreeView == null || _toolWindow == null) return false;

				try
				{
					var codeProvider = new VS2008CodeProvider(_applicationObject, null);
					_navigationTreeView.Init(codeProvider);
					_navigationTreeView.LoadFunctionList();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message + Environment.NewLine + ex.Source);
				}

				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		/// <summary>
		/// The document events_ document closing.
		/// </summary>
		/// <param name="doc">
		/// The doc.
		/// </param>
		private void documentEvents_DocumentClosing(Document doc)
		{
			if (_navigationTreeView != null)
				_navigationTreeView.Clear();
		}

		/// <summary>
		/// The document events_ document saved.
		/// </summary>
		/// <param name="doc">
		/// The doc.
		/// </param>
		private void documentEvents_DocumentSaved(Document doc)
		{
			if (_navigationTreeView != null)
				_navigationTreeView.LoadFunctionList();
		}

		/// <summary>
		/// The solution events_ opened.
		/// </summary>
		private void solutionEvents_Opened()
		{
			ShowWindow();
		}

		/// <summary>
		/// The window events_ window activated.
		/// </summary>
		/// <param name="GotFocus">
		/// The got focus.
		/// </param>
		/// <param name="LostFocus">
		/// The lost focus.
		/// </param>
		private void windowEvents_WindowActivated(Window GotFocus, Window LostFocus)
		{
			if (GotFocus == null || GotFocus.Kind != "Document") return;
			if (_navigationTreeView == null || GotFocus.Document == null) return;

			try
			{
				var codeProvider = new VS2008CodeProvider(_applicationObject, GotFocus.Document);
				_navigationTreeView.Init(codeProvider);
				_navigationTreeView.LoadFunctionList();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + Environment.NewLine + ex.Source);
			}
		}
	}
}