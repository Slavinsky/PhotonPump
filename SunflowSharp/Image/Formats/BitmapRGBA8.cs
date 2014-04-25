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
	public class BitmapRGBA8 : Bitmap {

		private int w, h;
		private byte[] data;
		
		public BitmapRGBA8(int w, int h, byte[] data) {
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
			int index = 4 * (x + y * w);
			float r = (data[index + 0] & 0xFF) * INV255;
			float g = (data[index + 1] & 0xFF) * INV255;
			float b = (data[index + 2] & 0xFF) * INV255;
			return new Color(r, g, b);
		}
		
		public override float readAlpha(int x, int y) {
			return (data[4 * (x + y * w) + 3] & 0xFF) * INV255;
		}
	}
}

