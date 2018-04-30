using System.IO;

namespace Gallerize {
	class FileInfo {
		public string Path { get; set; }
		public string Name { get; set; }
		public bool IsDirectory { get; set; }

		public FileInfo(string path, FileAttributes attributes) {
			this.Path = path;
			this.Name = System.IO.Path.GetFileName(path);
			this.IsDirectory = attributes.HasFlag(FileAttributes.Directory);
		}
	}
}
