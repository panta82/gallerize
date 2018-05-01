using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gallerize.Models;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace Gallerize {
	[ComVisible(true)]
	[COMServerAssociation(AssociationType.AllFiles)]
	[COMServerAssociation(AssociationType.Directory)]
	class GallerizeExtension : SharpContextMenu {
		protected override bool CanShowMenu() {
			return true;
		}

		protected override ContextMenuStrip CreateMenu() {
			var menu = new ContextMenuStrip();

			var items = SelectedItemPaths
				.Select(path => GalleryItem.FromFilePath(path))
				.Where(file => file.IsDirectory || file.IsImage)
				.ToList();

			var mainItem = new ToolStripMenuItem {
				Text = "Gallerize",
			};
			menu.Items.Add(mainItem);

			if (items.Count == 0) {
				mainItem.Enabled = false;
				return menu;
			}

			var dropDown = new ToolStripDropDown();

			Action<string, bool> addItem = (text, recurse) => {
				var item = new ToolStripMenuItem {
					Text = text
				};
				item.Click += (sender, e) => {
					var gallerize = new Gallerize(items, recurse);
					gallerize.Execute();
				};
				dropDown.Items.Add(item);
			};

			if (items.Count == 1) {
				var file = items[0];
				if (file.IsDirectory) {
					addItem($"Directory \"{file.Name}\"", false);
					addItem($"Directory \"{file.Name}\" (with subdirectories)", false);
				} else {
					addItem($"Image \"{file.Name}\"", false);
				}
			} else {
				var hasDirectories = items.Any(f => f.IsDirectory);
				var hasFiles = items.Any(f => !f.IsDirectory);
				if (hasDirectories) {
					var label = hasFiles ? "images and directories" : "directories";
					addItem(items.Count + " " + label, false);
					addItem(items.Count + " " + label + " (with subdirectories)", true);
				} else {
					addItem(items.Count + " images", false);
				}
			}

			mainItem.DropDown = dropDown;
			return menu;
		}
	}
}
