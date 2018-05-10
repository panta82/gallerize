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

			var items = this.SelectedItemPaths
				.Select(path => GalleryItem.FromPath(path))
				.Where(item => item.IsValid)
				.ToList();

			var mainItem = new ToolStripMenuItem {
				Text = "Gallerize",
			};
			menu.Items.Add(mainItem);

			if (items.Count == 0) {
				mainItem.Enabled = false;
				return menu;
			}

			var gallerize = new Gallerize();
			var dropDown = new ToolStripDropDown();

			Action<string, bool> addItem = (text, recurse) => {
				var item = new ToolStripMenuItem {
					Text = text
				};
				item.Click += (sender, e) => {
					this.Log($"Calling Execute with {items.Count} items, recurse = {recurse}");
					try {
						gallerize.Execute(items, recurse);
					}
					catch (Gallerize.GallerizeException ex) {
						MessageBox.Show(ex.Message, "Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					catch (Exception ex) {
						this.LogError("Failed to execute", ex);
					}
				};
				dropDown.Items.Add(item);
			};

			if (items.Count == 1) {
				var item = items[0];
				if (item.Type == GalleryItemType.Directory) {
					addItem($"Directory \"{item.Name}\"", false);
					addItem($"Directory \"{item.Name}\" (with subdirectories)", false);
				}
				else if (item.Type == GalleryItemType.Image) {
					addItem($"Image \"{item.Name}\"", false);
				}
				else if (item.Type == GalleryItemType.Archive) {
					addItem($"Archive \"{item.Name}\"", false);
				}
			} else {
				var hasDirectories = items.Any(i => i.Type == GalleryItemType.Directory);
				var hasFiles = items.Any(i => i.Type == GalleryItemType.Image || i.Type == GalleryItemType.Archive);
				if (hasDirectories) {
					var label = hasFiles ? "files and directories" : "directories";
					addItem(items.Count + " " + label, false);
					addItem(items.Count + " " + label + " (with subdirectories)", true);
				} else {
					addItem(items.Count + " files", false);
				}
			}

			mainItem.DropDown = dropDown;
			return menu;
		}
	}
}
