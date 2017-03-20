using System;
using System.Text;
using System.Windows.Forms;
using Fiddler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

[assembly: Fiddler.RequiredVersion("2.2.4.0")]
[assembly: AssemblyVersion("0.0.0.0")]
[assembly: AssemblyTitle("Req2Code")]
[assembly: AssemblyDescription("Automatically generate python code of the selected request")]
[assembly: AssemblyCompany("www.infosec-wiki.com")]
[assembly: AssemblyProduct("Req2Code")]

namespace Req2Code
{
	public class Req2Code : IFiddlerExtension
	{
		private TabPage oPage;
		// private TextBox newTxt;
		private RichTextBox newTxt;

		private void TextBox_KeyPress(object sender, KeyPressEventArgs e){
			TextBox textBox = sender as TextBox;
			if (textBox == null)
				return;
			if (e.KeyChar == (char)1)
			{
				textBox.SelectAll();
				e.Handled = true;
			}
		}
		
		private void MyTextBox_DragEnter(object sender, DragEventArgs e){
			e.Effect = DragDropEffects.Copy;
			// MessageBox.Show("MyTextBox_DragEnter");
		}

		private void MyTextBox_DragDrop(object sender, DragEventArgs e){
			DataObject v_obj = (DataObject)e.Data;
			String output = "";
			
			String[] v_formats = v_obj.GetFormats(false);
			foreach (string v_format in v_formats){
				// MessageBox.Show( v_format );
				// output += v_format + "\r\n";
			}
					
			Session[] session_arr = (Session[]) v_obj.GetData("Fiddler.Session[]");
			Session session = session_arr[0];
			var req = session.oRequest;
			var headers = req.headers;
			
			String host = session.host;
			String url = session.url;
			String fullUrl = session.fullUrl;
			bool isHTTPS = session.isHTTPS;
			String RequestBody = System.Text.Encoding.Default.GetString( session.RequestBody );
			String RequestMethod = session.RequestMethod;
			String requestBodyBytes = System.Text.Encoding.Default.GetString( session.requestBodyBytes );
					
			output =  "#encoding=utf8\r\n";
			output += "import requests\r\n";
			output += "import warnings\r\n";
			output += "warnings.filterwarnings(\"ignore\")\r\n\r\n";
			output += "http_session = requests.Session()\r\n\r\n";
			
			String func = "def my_request():\r\n";
			func += "\turl = \"" + fullUrl + "\"\r\n";			
			func += "\theaders = {\r\n";
			
			var header_arr = headers.ToArray();
			foreach (HTTPHeaderItem header in header_arr){
				String Name = header.Name;
				String Value = header.Value;
				// MessageBox.Show(Name+"="+Value);
				func += "\t\t\"" + Name + "\": \""+ Value + "\",\r\n";
			}
			func += "\t}\r\n";
			func += "\t\r\n";
								
			if(RequestMethod == "POST"){
				func += "\tpost_data = \"" + RequestBody + "\"\r\n";
				func += "\tdata = http_session.post(url, data=post_data, headers=headers, verify=False).content\r\n";
			}
			else if(RequestMethod == "GET"){
				func += "\tdata = http_session.get(url, headers=headers, verify=False).content\r\n";
			}
			func += "\treturn data\r\n\r\n";
			output += func;
					
			// MessageBox.Show(host);
			// MessageBox.Show(url); // without http://
			// MessageBox.Show(fullUrl);
			// MessageBox.Show(RequestBody);
			// MessageBox.Show(RequestMethod);
			// MessageBox.Show(requestBodyBytes);
	
			// MessageBox.Show("MyTextBox_DragDrop");
			// newTxt.ReadOnly = false;
			newTxt.Text = output;
			
		}
		
		public void OnLoad() {
			oPage = new TabPage("Req2Code");

			// newTxt = new TextBox();
			newTxt = new RichTextBox();
			// newTxt.Width = 400;
			// newTxt.Height = 400;
			newTxt.Multiline = true;
			newTxt.AcceptsTab = true;
			newTxt.AutoSize = true;
			newTxt.ReadOnly = true;
			newTxt.Dock = DockStyle.Fill;

			newTxt.AllowDrop = true;
			newTxt.DragDrop += new DragEventHandler(MyTextBox_DragDrop);
			newTxt.DragEnter += new DragEventHandler(MyTextBox_DragEnter);
			
			newTxt.KeyPress += TextBox_KeyPress;
			
			oPage.Controls.Add(newTxt);

			FiddlerApplication.UI.tabsViews.TabPages.Add(oPage);
		}

		public void OnBeforeUnload(){}
	}
}