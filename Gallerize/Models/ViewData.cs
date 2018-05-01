using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallerize.Models {
	public class ViewData {
		public IList<GalleryItem> Items { get; set; }
		public bool Recurse { get; set; }
	}
}
