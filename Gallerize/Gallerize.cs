using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gallerize {
	class Gallerize {
		public void Exec(IList<FileInfo> files, bool recurse) {
			var fileList = string.Join("\n", files.Select(f => f.Path));
			MessageBox.Show(fileList + "\n\n" + (recurse ? "recurse" : "no recurse"));
		}
	}
}
