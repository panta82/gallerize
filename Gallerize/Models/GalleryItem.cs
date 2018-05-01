using System.Collections.Generic;
using System.IO;

namespace Gallerize.Models {
	public class GalleryItem {
		static readonly HashSet<string> ImageFormats = new HashSet<string> {
			"jpg",
			"jpeg",
			"png",
			"bmp",
			"tiff",
			"gif"
		};

		public string Path { get; set; }
		public string Name { get; set; }
		public bool IsDirectory { get; set; }
		public bool IsImage { get; set; }

		public static GalleryItem FromFilePath(string filePath) {
			var attributes = File.GetAttributes(filePath);
			var item = new GalleryItem {
				Path = filePath,
				Name = System.IO.Path.GetFileName(filePath),
				IsDirectory = attributes.HasFlag(FileAttributes.Directory)
			};
			
			var extension = System.IO.Path.GetExtension(filePath);
			if (extension.Length > 1) {
				extension = extension.Substring(1);
			}
			item.IsImage = !item.IsDirectory && ImageFormats.Contains(extension);
			return item;
		}
	}
}
