using Microsoft.Deployment.WindowsInstaller;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace WixSharp.UI.Forms
{
    public static class Extensions
    {
        public static bool IsViewChecked(this FeatureItem feature)
        {
            if (feature.View is TreeNode)
                return (feature.View as TreeNode).Checked;
            return false;
        }

        public static FeatureItem FeatureItem(this TreeNode node)
        {
            return node.Tag as FeatureItem;
        }

        public static TreeNode[] AllNodes(this TreeView treeView)
        {
            var result = new List<TreeNode>();
            var queue = new Queue<TreeNode>(treeView.Nodes.Cast<TreeNode>());

            while (queue.Any())
            {
                TreeNode node = queue.Dequeue();
                result.Add(node);
                foreach (TreeNode child in node.Nodes)
                    queue.Enqueue(child);
            }
            return result.ToArray();
        }
    }
    internal class ReadOnlyTreeNode : TreeNode
    {
        public bool IsReadOnly { get; set; }

        public class Behavior
        {
            public static void AttachTo(TreeView treeView)
            {
                treeView.DrawMode = TreeViewDrawMode.OwnerDrawAll;
                treeView.DrawNode += treeView_DrawNode;
                treeView.BeforeCheck += treeView_BeforeCheck;
                treeView.MouseDown += (s, e) =>
                {
                    TreeNode clickedNode = treeView.GetNodeAt(e.X, e.Y);

                    Rectangle selectionClickableArea = clickedNode.Bounds;
                    selectionClickableArea.Offset(-15, 0);
                    if (treeView.CheckBoxes)
                        selectionClickableArea.Offset(13, 0);

                    if (selectionClickableArea.Contains(e.X, e.Y))
                        treeView.SelectedNode = clickedNode;
                }; ;
            }

            static void treeView_BeforeCheck(object sender, TreeViewCancelEventArgs e)
            {
                if (IsReadOnly(e.Node))
                    e.Cancel = true;
            }

            static Pen dotPen = new Pen(Color.FromArgb(128, 128, 128)) { DashStyle = DashStyle.Dot };
            static Brush selectionModeBrush = new SolidBrush(Color.FromArgb(51, 153, 255));
            static int cIndentBy = 19;		            // TODO - support Indent value
            static int cMargin = 6;		                // TODO - this is a bit random, it's slaved off the Indent in some way

            static bool IsReadOnly(TreeNode node)
            {
                return (node is ReadOnlyTreeNode) && (node as ReadOnlyTreeNode).IsReadOnly;
            }

            static void treeView_DrawNode(object sender, DrawTreeNodeEventArgs e)
            {
                //based on Jason Williams solution (http://stackoverflow.com/questions/1003459/c-treeview-owner-drawing-with-ownerdrawtext-and-the-weird-black-highlighting-w)
                if (e.Bounds.Height < 1 || e.Bounds.Width < 1)
                    return;

                var treeView = (TreeView)sender;

                Rectangle itemRect = e.Bounds;
                e.Graphics.FillRectangle(Brushes.White, itemRect);

                int cTwoMargins = cMargin * 2;

                int indent = (e.Node.Level * cIndentBy) + cMargin;
                int iconLeft = indent;						// Where to draw parentage lines & icon/checkbox
                int textLeft = iconLeft + 16;				// Where to draw text

                int midY = (itemRect.Top + itemRect.Bottom) / 2;

                // Draw parentage lines
                if (treeView.ShowLines)
                {
                    int x = cMargin * 2;

                    if (e.Node.Level == 0 && e.Node.PrevNode == null)
                    {
                        // The very first node in the tree has a half-height line
                        e.Graphics.DrawLine(dotPen, x, midY, x, itemRect.Bottom);
                    }
                    else
                    {
                        TreeNode testNode = e.Node;			// Used to only draw lines to nodes with Next Siblings, as in normal TreeViews
                        for (int iLine = e.Node.Level; iLine >= 0; iLine--)
                        {
                            if (testNode.NextNode != null)
                            {
                                x = (iLine * cIndentBy) + (cMargin * 2);
                                e.Graphics.DrawLine(dotPen, x, itemRect.Top, x, itemRect.Bottom);
                            }

                            testNode = testNode.Parent;
                        }

                        x = (e.Node.Level * cIndentBy) + cTwoMargins;
                        e.Graphics.DrawLine(dotPen, x, itemRect.Top, x, midY);
                    }

                    e.Graphics.DrawLine(dotPen, iconLeft + cMargin, midY, iconLeft + cMargin + 10, midY);
                }

                // Draw (plus/minus) icon if required
                if (e.Node.Nodes.Count > 0)
                {
                    // Use the VisualStyles renderer to use the proper OS-defined glyphs
                    Rectangle expandRect = new Rectangle(iconLeft - 1, midY - 7, 16, 16);

                    VisualStyleElement element = e.Node.IsExpanded ? VisualStyleElement.TreeView.Glyph.Opened : VisualStyleElement.TreeView.Glyph.Closed;
                    new VisualStyleRenderer(element).DrawBackground(e.Graphics, expandRect);
                }

                //Checkbox
                int checkBoxExtra = 0;
                if (treeView.CheckBoxes)
                {
                    Rectangle checkBoxRect = new Rectangle(itemRect.Left + iconLeft + 8, itemRect.Top - 6, 30, 30);

                    var el = e.Node.Checked ? VisualStyleElement.Button.CheckBox.CheckedNormal : VisualStyleElement.Button.CheckBox.UncheckedNormal;
                    if (IsReadOnly(e.Node))
                        el = e.Node.Checked ? VisualStyleElement.Button.CheckBox.CheckedDisabled : VisualStyleElement.Button.CheckBox.UncheckedDisabled;

                    new VisualStyleRenderer(el).DrawBackground(e.Graphics, checkBoxRect);
                    checkBoxExtra = 14;
                }

                //Text
                if (!string.IsNullOrEmpty(e.Node.Text))
                {
                    Point textStartPos = new Point(itemRect.Left + textLeft, itemRect.Top);
                    Point textPos = new Point(textStartPos.X + checkBoxExtra, textStartPos.Y);
                    SizeF textSize = e.Graphics.MeasureString(e.Node.Text, treeView.Font);

                    var drawFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        FormatFlags = StringFormatFlags.NoWrap,
                    };

                    var textRect = new Rectangle(textPos.X, textPos.Y, (int)textSize.Width + checkBoxExtra, itemRect.Bottom - textPos.Y + 2);

                    if (e.Node.IsSelected)
                    {
                        e.Graphics.FillRectangle(selectionModeBrush, textRect);
                        e.Graphics.DrawString(e.Node.Text, treeView.Font, Brushes.White, textRect, drawFormat);
                    }
                    else
                    {
                        e.Graphics.DrawString(e.Node.Text, treeView.Font, Brushes.Black, textRect, drawFormat);
                    }

                }

                // Focus rectangle around the text
                if (e.State == TreeNodeStates.Focused)
                {
                    var r = itemRect;
                    r.Width -= 2;
                    r.Height -= 2;
                    r.Offset(indent, 0);
                    r.Width -= indent;
                    e.Graphics.DrawRectangle(dotPen, r);
                }
            }
        }
    }
}