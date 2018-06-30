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

		private void GallerizeItems(IList<GalleryItem> items, bool recurse) {
			var gallerize = new Gallerize();
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

			var canRecurse = items.Count > 1 && items.Any(i => i.Type == GalleryItemType.Directory);
			if (!canRecurse) {
				// Immediately execute, no need for submenu
				mainItem.Click += (sender, e) => {
					this.GallerizeItems(items, false);
				};
			} else {
				// Show submenu with shallow and recursive options
				var dropDown = new ToolStripDropDown();
				{
					var item = new ToolStripMenuItem {
						Text = items.Count + " items",
					};
					item.Click += (sender, e) => {
						this.GallerizeItems(items, false);
					};
					dropDown.Items.Add(item);
				}

				{
					var item = new ToolStripMenuItem {
						Text = items.Count + " items (with subdirectories)",
					};
					item.Click += (sender, e) => {
						this.GallerizeItems(items, true);
					};
					dropDown.Items.Add(item);
				}
				mainItem.DropDown = dropDown;
			}

			return menu;
		}
	}
}
