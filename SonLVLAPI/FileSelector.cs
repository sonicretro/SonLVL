//------------------------------------------------------------------------------
// <copyright file="FileSelector.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonLVL.API
{
	using System;
	using System.ComponentModel;
	using System.IO;
	using System.Windows.Forms;
	using System.Windows.Forms.Design;
	using System.Collections;
	using System.Windows.Forms.Design.Behavior;

	/// <summary>
	/// Provides a control for selecting a file from an <see cref="OpenFileDialog"/>, by drag and drop or manually typing the
	/// path in a <see cref="TextBox"/>.
	/// </summary>
	[Designer(typeof(FileSelectorDesigner))]
	[DefaultEvent("FileNameChanged")]
	public partial class FileSelector : UserControl, ISupportInitialize
	{
		private static readonly object EventFileNameChanged = new object();

		private FileDialog fileDialog;
		private InitializationValues init;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileSelector"/> class.
		/// </summary>
		public FileSelector()
		{
			InitializeComponent();
			fileNameTextBox.TextChanged += fileNameTextBox_TextChanged;
			fileDialog = new OpenFileDialog();
		}

		/// <summary>
		/// Occurs when the <see cref="FileName"/> property value changes.
		/// </summary>
		public event EventHandler FileNameChanged
		{
			add
			{
				Events.AddHandler(EventFileNameChanged, value);
			}

			remove
			{
				Events.RemoveHandler(EventFileNameChanged, value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the control can accept data that the user drags onto it.
		/// </summary>
		/// <value>
		/// <c>true</c> if drag-and-drop operations are allowed in the control; otherwise, <c>false</c>. The default is
		/// <c>true</c>.
		/// </value>
		[DefaultValue(true)]
		public override bool AllowDrop
		{
			get
			{
				return base.AllowDrop;
			}
			set
			{
				base.AllowDrop = value;
			}
		}

		/// <summary>
		/// Gets or sets the default file name extension.
		/// </summary>
		/// <value>
		/// The default file name extension. The returned string does not include the period. The default value is an empty
		/// string ("").
		/// </value>
		public string DefaultExt
		{
			get
			{
				if (init == null)
				{
					return fileDialog.DefaultExt;
				}

				return init.DefaultExt;
			}

			set
			{
				if (init == null)
				{
					fileDialog.DefaultExt = value;
				}
				else
				{
					init.DefaultExt = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets a string containing the file name selected in the file selector.
		/// </summary>
		/// <value>The file name selected in the file dialog box. The default value is an empty string ("").</value>
		public string FileName
		{
			get
			{
				return fileNameTextBox.Text;
			}

			set
			{
				if (fileNameTextBox.Text != value)
				{
					fileNameTextBox.Text = value;

					if (init == null)
					{
						fileDialog.FileName = value;
					}

					OnFileNameChanged(new EventArgs());
				}
			}
		}

		/// <summary>
		/// Gets or sets the current file name filter string, which determines the choices that appear in the "Save as file
		/// type" or "Files of type" box in the dialog box.
		/// </summary>
		/// <value>The file filtering options available in the dialog box.</value>
		/// <exception cref="ArgumentException">Filter format is invalid.</exception>
		public string Filter
		{
			get
			{
				if (init == null)
				{
					return fileDialog.Filter;
				}

				return init.Filter;
			}

			set
			{
				if (init == null)
				{
					fileDialog.Filter = value;
				}
				else
				{
					init.Filter = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the browsing mode of the file selector.
		/// </summary>
		/// <value>
		/// <see cref="FileSelectorMode"/> indicating whether the Browse button will use an <see cref="OpenFileDialog"/> or a
		/// <see cref="SaveFileDialog"/>.
		/// </value>
		[DefaultValue(typeof(FileSelectorMode), "Open")]
		public FileSelectorMode Mode
		{
			get
			{
				if (init == null)
				{
					return fileDialog is OpenFileDialog ? FileSelectorMode.Open : FileSelectorMode.Save;
				}

				return init.Mode;
			}

			set
			{
				if (value < FileSelectorMode.Open || value > FileSelectorMode.Save)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				if (init == null)
				{
					if (value != Mode)
					{
						// Store the values from the previous file dialog
						string defaultExt = fileDialog.DefaultExt;
						string fileName = fileDialog.FileName;
						string filter = fileDialog.Filter;

						// Create the new file dialog
						fileDialog = value == FileSelectorMode.Open ?
							(FileDialog)new OpenFileDialog() :
							(FileDialog)new SaveFileDialog();

						// Restore the values in the new file dialog
						fileDialog.DefaultExt = defaultExt;
						fileDialog.FileName = fileName;
						fileDialog.Filter = filter;
					}
				}
				else
				{
					init.Mode = value;
				}
			}
		}

		/// <summary>
		/// Signals the <see cref="FileSelector"/> that initialization is starting.
		/// </summary>
		public void BeginInit()
		{
			if (init != null)
			{
				throw new InvalidOperationException(Properties.Resources.FileSelectorAlreadyBeingInitialized);
			}

			init = new InitializationValues();
		}

		/// <summary>
		/// Signals the <see cref="FileSelector"/> that initialization is complete.
		/// </summary>
		public void EndInit()
		{
			if (init == null)
			{
				throw new InvalidOperationException(Properties.Resources.FileSelectorNotBeingInitialized);
			}

			fileDialog = init.Mode == FileSelectorMode.Open ?
				(FileDialog)new OpenFileDialog() :
				(FileDialog)new SaveFileDialog();
			fileDialog.DefaultExt = init.DefaultExt;
			fileDialog.FileName = fileNameTextBox.Text;
			fileDialog.Filter = init.Filter;
		}

		/// <summary>
		/// Sets the target drop effect according to whether the drag and drop data contains acceptable data, then raises the
		/// <see cref="Control.DragEnter"/> event.
		/// </summary>
		/// <param name="drgevent">A <see cref="DragEventArgs"/> that contains the event data.</param>
		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
			{
				object data = drgevent.Data.GetData(DataFormats.FileDrop);
				string[] dataAsStringArray = data as string[];
				if (dataAsStringArray != null && dataAsStringArray.Length > 0 && File.Exists(dataAsStringArray[0]))
				{
					drgevent.Effect = DragDropEffects.Copy;
				}
				else
				{
					drgevent.Effect = DragDropEffects.None;
				}
			}
			else
			{
				drgevent.Effect = DragDropEffects.None;
			}

			base.OnDragEnter(drgevent);
		}

		/// <summary>
		/// Sets the selected path of the current instance to the first file in the file list if the drag and drop data is
		/// acceptable, then raises the <see cref="Control.DragDrop"/> event.
		/// </summary>
		/// <param name="drgevent">A <see cref="DragEventArgs"/> that contains the event data.</param>
		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
			{
				object data = drgevent.Data.GetData(DataFormats.FileDrop);
				string[] dataAsStringArray = data as string[];
				if (dataAsStringArray != null && dataAsStringArray.Length > 0 && File.Exists(dataAsStringArray[0]))
				{
					fileNameTextBox.Text = dataAsStringArray[0];
					fileDialog.FileName = dataAsStringArray[0];
					OnFileNameChanged(new EventArgs());
				}
			}

			base.OnDragDrop(drgevent);
		}

		/// <summary>
		/// Raises the <see cref="FileNameChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnFileNameChanged(EventArgs e)
		{
			(Events[EventFileNameChanged] as EventHandler)?.Invoke(this, e);
		}

		private void fileNameTextBox_TextChanged(object sender, EventArgs e)
		{
			OnFileNameChanged(new EventArgs());
		}

		private void browseButton_Click(object sender, EventArgs e)
		{
			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				fileNameTextBox.Text = fileDialog.FileName;
			}
		}

		private class InitializationValues
		{
			public FileSelectorMode Mode { get; set; }

			public string DefaultExt { get; set; }

			public string Filter { get; set; }
		}
	}

	public class FileSelectorDesigner : ControlDesigner
	{
		public override IList SnapLines
		{
			get
			{
				return new ArrayList(base.SnapLines)
				{
					new SnapLine(SnapLineType.Baseline, 17)
				};
			}
		}
	}
}
