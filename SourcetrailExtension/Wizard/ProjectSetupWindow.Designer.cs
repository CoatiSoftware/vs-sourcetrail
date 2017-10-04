﻿namespace CoatiSoftware.SourcetrailExtension.Wizard
{
	partial class ProjectSetupWindow
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectSetupWindow));
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonCreate = new System.Windows.Forms.Button();
			this.comboBoxConfiguration = new System.Windows.Forms.ComboBox();
			this.comboBoxPlatform = new System.Windows.Forms.ComboBox();
			this.labelConfiguration = new System.Windows.Forms.Label();
			this.labelPlatform = new System.Windows.Forms.Label();
			this.buttonSelectAll = new System.Windows.Forms.Button();
			this.labelSelectProject = new System.Windows.Forms.Label();
			this.folderBrowserTargetDirectory = new System.Windows.Forms.FolderBrowserDialog();
			this.textBoxTargetDirectory = new System.Windows.Forms.TextBox();
			this.buttonSelect = new System.Windows.Forms.Button();
			this.textBoxFileName = new System.Windows.Forms.TextBox();
			this.labelFileName = new System.Windows.Forms.Label();
			this.labelFileNameExtension = new System.Windows.Forms.Label();
			this.helpProvider1 = new System.Windows.Forms.HelpProvider();
			this.comboBoxCStandard = new System.Windows.Forms.ComboBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.labelCStandard = new System.Windows.Forms.Label();
			this.treeViewProjects = new System.Windows.Forms.TreeView();
			this.labelAdditionalClangOptions = new System.Windows.Forms.Label();
			this.textBoxAdditionalClangOptions = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.helpProvider1.SetHelpString(this.buttonCancel, "Abort creation of the Compilation Database");
			this.buttonCancel.Location = new System.Drawing.Point(12, 441);
			this.buttonCancel.Name = "buttonCancel";
			this.helpProvider1.SetShowHelp(this.buttonCancel, true);
			this.buttonCancel.Size = new System.Drawing.Size(92, 23);
			this.buttonCancel.TabIndex = 9;
			this.buttonCancel.Text = "Cancel";
			this.toolTip1.SetToolTip(this.buttonCancel, "Abort creation of the Compilation Database");
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonCreate
			// 
			this.helpProvider1.SetHelpString(this.buttonCreate, "Start creation of the Compilation Database");
			this.buttonCreate.Location = new System.Drawing.Point(178, 441);
			this.buttonCreate.Name = "buttonCreate";
			this.helpProvider1.SetShowHelp(this.buttonCreate, true);
			this.buttonCreate.Size = new System.Drawing.Size(92, 23);
			this.buttonCreate.TabIndex = 8;
			this.buttonCreate.Text = "Create";
			this.toolTip1.SetToolTip(this.buttonCreate, "Start creation of the Compilation Database");
			this.buttonCreate.UseVisualStyleBackColor = true;
			this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
			// 
			// comboBoxConfiguration
			// 
			this.comboBoxConfiguration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxConfiguration.FormattingEnabled = true;
			this.helpProvider1.SetHelpKeyword(this.comboBoxConfiguration, "Build configuration for Compilation Database");
			this.helpProvider1.SetHelpString(this.comboBoxConfiguration, "The selected build configuration determines the compile flags for the Compilation" +
		" Database");
			this.comboBoxConfiguration.Location = new System.Drawing.Point(87, 277);
			this.comboBoxConfiguration.Name = "comboBoxConfiguration";
			this.helpProvider1.SetShowHelp(this.comboBoxConfiguration, true);
			this.comboBoxConfiguration.Size = new System.Drawing.Size(183, 21);
			this.comboBoxConfiguration.TabIndex = 3;
			this.toolTip1.SetToolTip(this.comboBoxConfiguration, "The selected build configuration determines the compile flags for the Compilation" +
		" Database");
			// 
			// comboBoxPlatform
			// 
			this.comboBoxPlatform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxPlatform.FormattingEnabled = true;
			this.helpProvider1.SetHelpString(this.comboBoxPlatform, "The target platform determines some compiler flags included in the Compilation Da" +
		"tabase");
			this.comboBoxPlatform.Location = new System.Drawing.Point(87, 304);
			this.comboBoxPlatform.Name = "comboBoxPlatform";
			this.helpProvider1.SetShowHelp(this.comboBoxPlatform, true);
			this.comboBoxPlatform.Size = new System.Drawing.Size(183, 21);
			this.comboBoxPlatform.TabIndex = 4;
			this.toolTip1.SetToolTip(this.comboBoxPlatform, "The target platform determines some compiler flags included in the Compilation Da" +
		"tabase");
			// 
			// labelConfiguration
			// 
			this.labelConfiguration.AutoSize = true;
			this.labelConfiguration.Location = new System.Drawing.Point(12, 280);
			this.labelConfiguration.Name = "labelConfiguration";
			this.labelConfiguration.Size = new System.Drawing.Size(69, 13);
			this.labelConfiguration.TabIndex = 6;
			this.labelConfiguration.Text = "Configuration";
			// 
			// labelPlatform
			// 
			this.labelPlatform.AutoSize = true;
			this.labelPlatform.Location = new System.Drawing.Point(12, 307);
			this.labelPlatform.Name = "labelPlatform";
			this.labelPlatform.Size = new System.Drawing.Size(45, 13);
			this.labelPlatform.TabIndex = 7;
			this.labelPlatform.Text = "Platform";
			// 
			// buttonSelectAll
			// 
			this.helpProvider1.SetHelpString(this.buttonSelectAll, "Select all projects if not all are ticked. Deselect all otherwise");
			this.buttonSelectAll.Location = new System.Drawing.Point(178, 248);
			this.buttonSelectAll.Name = "buttonSelectAll";
			this.helpProvider1.SetShowHelp(this.buttonSelectAll, true);
			this.buttonSelectAll.Size = new System.Drawing.Size(92, 23);
			this.buttonSelectAll.TabIndex = 2;
			this.buttonSelectAll.Text = "De/Select All";
			this.toolTip1.SetToolTip(this.buttonSelectAll, "Select all projects if not all are ticked. Deselect all otherwise");
			this.buttonSelectAll.UseVisualStyleBackColor = true;
			this.buttonSelectAll.Click += new System.EventHandler(this.buttonSelectAll_Click);
			// 
			// labelSelectProject
			// 
			this.labelSelectProject.AutoSize = true;
			this.labelSelectProject.Location = new System.Drawing.Point(9, 9);
			this.labelSelectProject.Name = "labelSelectProject";
			this.labelSelectProject.Size = new System.Drawing.Size(78, 13);
			this.labelSelectProject.TabIndex = 9;
			this.labelSelectProject.Text = "Select Projects";
			this.labelSelectProject.Click += new System.EventHandler(this.label1_Click);
			// 
			// textBoxTargetDirectory
			// 
			this.helpProvider1.SetHelpString(this.textBoxTargetDirectory, "Target directory where the Compilation Database will be stored");
			this.textBoxTargetDirectory.Location = new System.Drawing.Point(12, 333);
			this.textBoxTargetDirectory.Name = "textBoxTargetDirectory";
			this.helpProvider1.SetShowHelp(this.textBoxTargetDirectory, true);
			this.textBoxTargetDirectory.Size = new System.Drawing.Size(177, 20);
			this.textBoxTargetDirectory.TabIndex = 10;
			this.toolTip1.SetToolTip(this.textBoxTargetDirectory, "Target directory where the Compilation Database will be stored");
			// 
			// buttonSelect
			// 
			this.helpProvider1.SetHelpString(this.buttonSelect, "Pick a target directory via folder browser");
			this.buttonSelect.Location = new System.Drawing.Point(195, 331);
			this.buttonSelect.Name = "buttonSelect";
			this.helpProvider1.SetShowHelp(this.buttonSelect, true);
			this.buttonSelect.Size = new System.Drawing.Size(75, 23);
			this.buttonSelect.TabIndex = 5;
			this.buttonSelect.Text = "Browse";
			this.toolTip1.SetToolTip(this.buttonSelect, "Pick a target directory via folder browser");
			this.buttonSelect.UseVisualStyleBackColor = true;
			this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
			// 
			// textBoxFileName
			// 
			this.textBoxFileName.Location = new System.Drawing.Point(87, 360);
			this.textBoxFileName.Name = "textBoxFileName";
			this.textBoxFileName.Size = new System.Drawing.Size(148, 20);
			this.textBoxFileName.TabIndex = 6;
			this.textBoxFileName.TextChanged += new System.EventHandler(this.textBoxFileName_TextChanged);
			this.textBoxFileName.Leave += new System.EventHandler(this.textBoxFileName_Leave);
			// 
			// labelFileName
			// 
			this.labelFileName.AutoSize = true;
			this.labelFileName.Location = new System.Drawing.Point(12, 363);
			this.labelFileName.Name = "labelFileName";
			this.labelFileName.Size = new System.Drawing.Size(49, 13);
			this.labelFileName.TabIndex = 13;
			this.labelFileName.Text = "Filename";
			// 
			// labelFileNameExtension
			// 
			this.labelFileNameExtension.AutoSize = true;
			this.labelFileNameExtension.Location = new System.Drawing.Point(241, 363);
			this.labelFileNameExtension.Name = "labelFileNameExtension";
			this.labelFileNameExtension.Size = new System.Drawing.Size(29, 13);
			this.labelFileNameExtension.TabIndex = 14;
			this.labelFileNameExtension.Text = ".json";
			// 
			// comboBoxCStandard
			// 
			this.comboBoxCStandard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCStandard.FormattingEnabled = true;
			this.helpProvider1.SetHelpString(this.comboBoxCStandard, "In case your solution contains C files, please specify which C standard is to be " +
		"used for building.");
			this.comboBoxCStandard.Location = new System.Drawing.Point(87, 386);
			this.comboBoxCStandard.Name = "comboBoxCStandard";
			this.helpProvider1.SetShowHelp(this.comboBoxCStandard, true);
			this.comboBoxCStandard.Size = new System.Drawing.Size(183, 21);
			this.comboBoxCStandard.TabIndex = 7;
			this.toolTip1.SetToolTip(this.comboBoxCStandard, "In case your solution contains C files, please specify which C standard is to be " +
		"used for building.");
			// 
			// labelCStandard
			// 
			this.labelCStandard.AutoSize = true;
			this.labelCStandard.Location = new System.Drawing.Point(12, 389);
			this.labelCStandard.Name = "labelCStandard";
			this.labelCStandard.Size = new System.Drawing.Size(60, 13);
			this.labelCStandard.TabIndex = 16;
			this.labelCStandard.Text = "C Standard";
			// 
			// treeViewProjects
			// 
			this.treeViewProjects.CheckBoxes = true;
			this.treeViewProjects.Location = new System.Drawing.Point(12, 26);
			this.treeViewProjects.Name = "treeViewProjects";
			this.treeViewProjects.Size = new System.Drawing.Size(258, 216);
			this.treeViewProjects.TabIndex = 17;
			this.treeViewProjects.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewProjects_NodeCheckChanged);
			this.treeViewProjects.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ProjectCheckList_MouseUp);
			// 
			// labelAdditionalClangOptions
			// 
			this.labelAdditionalClangOptions.AutoSize = true;
			this.labelAdditionalClangOptions.Location = new System.Drawing.Point(12, 416);
			this.labelAdditionalClangOptions.Name = "labelAdditionalClangOptions";
			this.labelAdditionalClangOptions.Size = new System.Drawing.Size(73, 13);
			this.labelAdditionalClangOptions.TabIndex = 18;
			this.labelAdditionalClangOptions.Text = "Clang Options";
			// 
			// textBoxAdditionalClangOptions
			// 
			this.textBoxAdditionalClangOptions.Location = new System.Drawing.Point(87, 413);
			this.textBoxAdditionalClangOptions.Name = "textBoxAdditionalClangOptions";
			this.textBoxAdditionalClangOptions.Size = new System.Drawing.Size(183, 20);
			this.textBoxAdditionalClangOptions.TabIndex = 19;
			this.textBoxAdditionalClangOptions.WordWrap = false;
			// 
			// ProjectSetupWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(282, 475);
			this.Controls.Add(this.textBoxAdditionalClangOptions);
			this.Controls.Add(this.labelAdditionalClangOptions);
			this.Controls.Add(this.treeViewProjects);
			this.Controls.Add(this.labelCStandard);
			this.Controls.Add(this.comboBoxCStandard);
			this.Controls.Add(this.labelFileNameExtension);
			this.Controls.Add(this.labelFileName);
			this.Controls.Add(this.textBoxFileName);
			this.Controls.Add(this.buttonSelect);
			this.Controls.Add(this.textBoxTargetDirectory);
			this.Controls.Add(this.labelSelectProject);
			this.Controls.Add(this.buttonSelectAll);
			this.Controls.Add(this.labelPlatform);
			this.Controls.Add(this.labelConfiguration);
			this.Controls.Add(this.comboBoxPlatform);
			this.Controls.Add(this.comboBoxConfiguration);
			this.Controls.Add(this.buttonCreate);
			this.Controls.Add(this.buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.helpProvider1.SetHelpString(this, "Create a Compilation Database from the current C/C++ solution.");
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProjectSetupWindow";
			this.helpProvider1.SetShowHelp(this, true);
			this.Text = "Create Compilation Database";
			this.toolTip1.SetToolTip(this, "Create a Compilation Database from the current C/C++ solution.");
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonCreate;
		private System.Windows.Forms.ComboBox comboBoxConfiguration;
		private System.Windows.Forms.ComboBox comboBoxPlatform;
		private System.Windows.Forms.Label labelConfiguration;
		private System.Windows.Forms.Label labelPlatform;
		private System.Windows.Forms.Button buttonSelectAll;
		private System.Windows.Forms.Label labelSelectProject;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserTargetDirectory;
		private System.Windows.Forms.TextBox textBoxTargetDirectory;
		private System.Windows.Forms.Button buttonSelect;
		private System.Windows.Forms.TextBox textBoxFileName;
		private System.Windows.Forms.Label labelFileName;
		private System.Windows.Forms.Label labelFileNameExtension;
		private System.Windows.Forms.HelpProvider helpProvider1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.ComboBox comboBoxCStandard;
		private System.Windows.Forms.Label labelCStandard;
		private System.Windows.Forms.TreeView treeViewProjects;
		private System.Windows.Forms.Label labelAdditionalClangOptions;
		private System.Windows.Forms.TextBox textBoxAdditionalClangOptions;
	}
}