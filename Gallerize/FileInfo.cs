using System.Collections.Generic;
using System.IO;

namespace Gallerize {
	class FileInfo {
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

		public FileInfo(string path, FileAttributes attributes) {
			this.Path = path;
			this.Name = System.IO.Path.GetFileName(path);
			this.IsDirectory = attributes.HasFlag(FileAttributes.Directory);

			var extension = System.IO.Path.GetExtension(path);
			if (extension.Length > 1) {
				extension = extension.Substring(1);
			}
			this.IsImage = !this.IsDirectory && ImageFormats.Contains(extension);
		}
	}
}
