using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Gallerize.Models;
using RazorEngine;
using RazorEngine.Templating;

namespace Gallerize {
	public class Gallerize {
		public IList<GalleryItem> Items { get; private set; }
		public bool Recurse { get; private set; }

		public Gallerize(IList<GalleryItem> items, bool recurse) {
			this.Items = items;
			this.Recurse = recurse;
		}

		private string GetTargetPath() {
			return System.IO.Path.GetTempFileName();
		}

		public void Execute() {
			var html = this.GenerateHTML();
			var fileList = string.Join("\n", this.Items.Select(f => f.Path));
			MessageBox.Show(fileList + "\n\n" + (this.Recurse ? "recurse" : "no recurse"));
		}

		public string GenerateHTML() {
			var templateFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views\\index.cshtml");
			string template = File.ReadAllText(templateFile);
			var data = new ViewData {
				Items = this.Items,
				Recurse = this.Recurse
			};
			var result = Engine.Razor.RunCompile(new LoadedTemplateSource(template, templateFile), "index", null, data);
			return result;
		}
	}
}
