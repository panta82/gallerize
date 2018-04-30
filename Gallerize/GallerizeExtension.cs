using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace Gallerize {
	[ComVisible(true)]
	[COMServerAssociation(AssociationType.AllFiles)]
	[COMServerAssociation(AssociationType.Directory)]
	class GallerizeExtension : SharpContextMenu {
		protected Gallerize gallerize { get; private set; }

		public GallerizeExtension() {
			this.gallerize = new Gallerize();
		}

		protected override bool CanShowMenu() {
			return true;
		}

		protected override ContextMenuStrip CreateMenu() {
			var menu = new ContextMenuStrip();

			var files = SelectedItemPaths.Select(path => new FileInfo(path, File.GetAttributes(path))).ToList();
			if (files.Count == 0) {
				return menu;
			}

			var dropDown = new ToolStripDropDown();

			Action<string, bool> addItem = (text, recurse) => {
				var item = new ToolStripMenuItem {
					Text = text
				};
				item.Click += (sender, e) => this.gallerize.Exec(files, recurse);
				dropDown.Items.Add(item);
			};

			if (files.Count == 1) {
				var file = files[0];
				if (file.IsDirectory) {
					addItem($"Directory \"{file.Name}\"", false);
					addItem($"Directory \"{file.Name}\" (with subdirectories)", false);
				} else {
					addItem($"File \"{file.Name}\"", false);
				}
			} else {
				var hasDirectories = files.Any(f => f.IsDirectory);
				var hasFiles = files.Any(f => !f.IsDirectory);
				if (hasDirectories) {
					var label = hasFiles ? "files and directories" : "directories";
					addItem(files.Count + " " + label, false);
					addItem(files.Count + " " + label + " (with subdirectories)", true);
				} else {
					addItem(files.Count + " files", false);
				}
			}

			var mainItem = new ToolStripMenuItem {
				Text = "Gallerize",
			};
			mainItem.DropDown = dropDown;
			menu.Items.Add(mainItem);
			return menu;
		}
	}
}
