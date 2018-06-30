using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallerize.Models {
	public class ViewData {
		public string Title { get; set; }
		public IList<GalleryGroup> Groups { get; set; }
	}
}
