/*
 * Copyright 2018 Coati Software KG
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Windows.Forms;

namespace CoatiSoftware.SourcetrailExtension.Wizard
{
	public partial class WindowMessage : Form
	{
		private string _title = "Title";
		private string _message = "Message";

		public delegate void Callback();

		private Callback _onOK = null;
		private Callback _onCancel = null;

		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		public Callback OnOK
		{
			get { return _onOK; }
			set { _onOK = value; }
		}

		public Callback OnCancel
		{
			get { return _onCancel; }
			set { _onCancel = value; }
		}

		public WindowMessage()
		{
			InitializeComponent();
		}

		public void RefreshWindow()
		{
			Text = _title;
			labelContent.Text = _message;

			if(_onCancel != null)
			{
				buttonCancel.Show();
			}
			else
			{
				buttonCancel.Hide();
			}
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			if(_onCancel != null)
			{
				_onCancel();
			}

			Close();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if(_onOK != null)
			{
				_onOK();
			}

			Close();
		}
	}
}
