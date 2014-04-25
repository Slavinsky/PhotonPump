// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
namespace SunflowSharp.Image.Formats
{
	public class BitmapXYZ : Bitmap {

		private int w, h;
		private float[] data;
		
		public BitmapXYZ(int w, int h, float[] data) {
			this.w = w;
			this.h = h;
			this.data = data;
		}
		
		public override int getWidth() {
			return w;
		}
		
		public override int getHeight() {
			return h;
		}
		
		public override Color readColor(int x, int y) {
			int index = 3 * (x + y * w);
			return Color.NATIVE_SPACE.convertXYZtoRGB(new XYZColor(data[index], data[index + 1], data[index + 2])).mul(0.1f);
		}
		
		public override float readAlpha(int x, int y) {
			return 1;
		}
	}
}

