using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallerize.Models {
	public class GalleryGroup {
		public string Name { get; set; }
		public string Path { get; set; }
		public IList<GalleryItem> Items { get; set; }

		public bool HasItems {
			get {
				return this.Items?.Count > 0;
			}
		}
		
		public void AddItem(GalleryItem item) {
			if (this.Items == null) {
				this.Items = new List<GalleryItem>();
			}
			this.Items.Add(item);
		}
	}
}
