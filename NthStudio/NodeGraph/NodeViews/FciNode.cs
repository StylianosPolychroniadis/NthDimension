
using NthDimension.Forms.Events;
using NthStudio.NodeGraph;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NthStudio.Nodes
{
	public class FciNode : NodeGraphNode
	{
		public FciNode(int p_X, int p_Y, NodeViewState p_View, bool p_CanBeSelected) : base(p_X, p_Y, p_View, p_CanBeSelected)
		{
			this.m_sName = "Fuel Consumption Indeces";
			base.Width = 400;
			base.Height = 300;
		}
		public override void Draw(PaintEventArgs e, bool useCustomPoints, bool showHeader)
		{
			base.Draw(e, useCustomPoints, true);
			//this.DrawChart(e.GC);
		}
		private void DrawChart(Graphics objGraphics)
		{
			Color[] array = new Color[]
			{
				Color.Blue,
				Color.Blue,
				Color.Blue,
				Color.Blue,
				Color.Blue,
				Color.Blue,
				Color.Blue,
				Color.Blue,
				Color.Blue,
				Color.Green
			};
            System.Random random = new System.Random();
			Point point = new Point(90, 250);
			Point point2 = new Point(110, 250);
			Point point3 = new Point(115, 260);
			Point point4 = new Point(95, 260);
			Point[] array2 = new Point[]
			{
				point,
				point2,
				point3,
				point4
			};
			objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
			for (int i = 90; i < 390; i += 30)
			{
				array2[0].X = i + base.Width + this.X;
				array2[1].X = i + 20 + base.Width + this.X;
				array2[2].X = i + 25 + base.Width + this.X;
				array2[3].X = i + 5 + base.Width + this.X;
				array2[0].Y = 250 + base.Height + this.Y;
				array2[1].Y = 250 + base.Height + this.Y;
				array2[2].Y = 260 + base.Height + this.Y;
				array2[3].Y = 260 + base.Height + this.Y;
				SolidBrush solidBrush = new SolidBrush(array[(i - 90) / 30]);
				int num = random.Next() % 250;
				for (int j = 0; j < num; j++)
				{
					if (j == num - 1)
					{
						objGraphics.FillPolygon(solidBrush, array2);
					}
					else
					{
						objGraphics.FillPolygon(new HatchBrush(HatchStyle.Percent50, solidBrush.Color), array2);
					}
					for (int k = 0; k < array2.Length; k++)
					{
						Point[] expr_2D0_cp_0 = array2;
						int expr_2D0_cp_1 = k;
						expr_2D0_cp_0[expr_2D0_cp_1].Y = expr_2D0_cp_0[expr_2D0_cp_1].Y - 1;
					}
				}
			}
		}
	}
}
